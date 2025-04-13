namespace Insurance.ViewModels
{
    public class NotificationModel
    {
        public int PrimaryId { get; set; }
        public string Message { get; set; }
        public bool? Status { get; set; }
        public string Type { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
