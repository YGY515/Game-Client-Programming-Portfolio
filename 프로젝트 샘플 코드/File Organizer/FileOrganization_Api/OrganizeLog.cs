namespace FileOrganization_Api.Models
{
    public class OrganizeLog
    {
        public int Id { get; set; }          
        public DateTime Date { get; set; }
        public string Path { get; set; }

        public int OrganizeMethod { get; set; }
        public int FileCount { get; set; }
        public int FolderCount { get; set; }
        public bool WasCancelled { get; set; }
    }
}
