namespace FileOrganization_Core.Organization
{
    public class Date : FileOrganizerBase
    {
        string _path = null;
        int count = 0;
        HashSet<string> fileList = new HashSet<string>();
        List<String> files = new List<string>();

        public override string Organize(string path)
        {
            _path = path;
            files = Directory.GetFiles(_path).ToList();

            CollectFiles();
            CreateFolders();
            MoveFiles();
            
            return PrintLog(count, fileList.Count);
        }

        public override void CollectFiles()
        {
            foreach (var file in files)
            {
                count++;
                var info = new FileInfo(file);
                string date = info.LastWriteTime.ToString("yyyy-MM");
                fileList.Add(date); 
            }
        }

        public override void CreateFolders()
        {
            foreach (string file in fileList)
            {
                string folderPath = Path.Combine(_path, file);
                Directory.CreateDirectory(folderPath);
            }
        }

        public override void MoveFiles()
        {
            SemaphoreSlim semaphore = new SemaphoreSlim(4);

            Parallel.ForEach(files, file =>
            {
                semaphore.Wait();
                try
                {
                    var info = new FileInfo(file);
                    string folder = info.LastWriteTime.ToString("yyyy-MM");

                    string destFolder = Path.Combine(_path, folder);
                    string destPath = Path.Combine(destFolder, Path.GetFileName(file));

                    File.Move(file, destPath, true);
                }
                finally
                {
                    semaphore.Release();
                }
            });
        }
    }
}
