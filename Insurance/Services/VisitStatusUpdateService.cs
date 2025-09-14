using Insurance.Areas.VMS.Models; 
using Insurance.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
public class VisitStatusUpdateService : BackgroundService
{
    private readonly ILogger<VisitStatusUpdateService> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly TimeSpan _pollInterval;

    public VisitStatusUpdateService(ILogger<VisitStatusUpdateService> logger, IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _pollInterval = TimeSpan.FromMinutes(1); // change to 30s or 5m if you prefer

    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("VisitStatusUpdater started, interval: {interval}", _pollInterval);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                // 'now' in the same kind (UTC/local) as data is stored — adjust as needed
                var now = DateTime.UtcNow;

                // fetch only candidates: CheckedIn and have a non-null LengthOfMeeting
                var candidates = await db.Visits
                    .Where(v => v.Status == VisitStatus.CheckedIn && v.LengthOfMeeting != null && v.CheckInDate != null && v.CheckInTime != null)
                    .ToListAsync(stoppingToken);

                var toUpdate = new List<Visit>();

                foreach (var v in candidates)
                {
                    // Convert CheckInDate + CheckInTime to a DateTime. Use UTC if you store UTC; adjust conversions if you use local time.
                    DateOnly checkInDate = v.CheckInDate!.Value;
                    TimeOnly checkInTime = v.CheckInTime!.Value;

                    // build DateTime in UTC:
                    var checkInDateTime = checkInDate.ToDateTime(checkInTime);

                    // If you store local DateTime, convert checkInDateTime to UTC:
                    // var checkInUtc = TimeZoneInfo.ConvertTimeToUtc(checkInDateTime, TimeZoneInfo.Local);
                    var checkInUtc = checkInDateTime; // assume stored as UTC-compatible

                    var checkoutUtc = checkInUtc + v.LengthOfMeeting!.Value;

                    if (checkoutUtc <= now)
                    {
                        // set checkout time (optional) and status
                        v.CheckOutTime = TimeOnly.FromDateTime(checkoutUtc);
                        v.Status = VisitStatus.CheckedOut;
                        toUpdate.Add(v);
                    }
                }

                if (toUpdate.Count > 0)
                {
                    // persist changes
                    await db.SaveChangesAsync(stoppingToken);
                    _logger.LogInformation("Marked {count} visits as CheckedOut.", toUpdate.Count);
                }
            }
            catch (OperationCanceledException) { /* shutting down */ }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating visit statuses.");
            }

            try
            {
                await Task.Delay(_pollInterval, stoppingToken);
            }
            catch (TaskCanceledException) { /* shutting down */ }
        }

        _logger.LogInformation("VisitStatusUpdater stopping.");
    }
}
