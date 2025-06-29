namespace Insurance.Areas.Risk.Models
{
    public class ImportResult
    {
        public bool Success { get; set; }
        public int RowsImported { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }
}
