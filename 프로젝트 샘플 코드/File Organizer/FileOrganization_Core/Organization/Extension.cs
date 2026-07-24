namespace FileOrganization_Core.Organization
{
    public class Extension : FileOrganizerBase
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
                string extension = Path.GetExtension(file);
                fileList.Add(extension);
            }
        }

        public override void CreateFolders()
        {
            foreach (string file in fileList)
            {
                string folderPath = Path.Combine(_path, file.Replace(".", ""));
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
                    string extension = Path.GetExtension(file).Replace(".", "");
                    string destFolder = Path.Combine(_path, extension);
                    string destPath = Path.Combine(destFolder, Path.GetFileName(file));

                    File.Move(file, destPath);
                }
                finally
                {
                    semaphore.Release();
                }
            });
        }
    }
}
