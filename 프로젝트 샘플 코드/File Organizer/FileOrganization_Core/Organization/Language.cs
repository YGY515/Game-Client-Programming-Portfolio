namespace FileOrganization_Core.Organization
{
    public class Language : FileOrganizerBase
    {
        string _path = null;
        object _lock = new object();
        int count = 0;

        HashSet<string> fileList = new HashSet<string>();
        List<String> files = new List<string>();
        List<(string from, string to)> moveLog = new List<(string from, string to)>();

        public override string Organize(string path, CancellationToken token)
        {
            _path = path;
            files = Directory.GetFiles(_path).ToList();

            CollectFiles();
            CreateFolders();
            MoveFiles(token);
            
            return PrintLog(count, fileList.Count); 
        }

        public override void CollectFiles()
        {
            foreach (var file in files)
            {
                count++;
                string lang = Path.GetFileNameWithoutExtension(file);

                if (lang[0] >= '가' && lang[0] <= '힣') lang = "Korean";
                else if (lang[0] >= 'a' && lang[0] <= 'z') lang = "English";

                fileList.Add(lang);
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
        public override void MoveFiles(CancellationToken token)
        {
            var options = new ParallelOptions { CancellationToken = token };
            SemaphoreSlim semaphore = new SemaphoreSlim(4);

            try
            {
                Parallel.ForEach(files, options, file =>
                {
                    options.CancellationToken.ThrowIfCancellationRequested();
                    semaphore.Wait();
                    try
                    {
                        string info = Path.GetFileNameWithoutExtension(file);
                        string lang = "";

                        if (lang[0] >= '가' && lang[0] <= '힣') lang = "Korean";
                        else if (lang[0] >= 'a' && lang[0] <= 'z') lang = "English";
                        Thread.Sleep(5000);

                        string destFolder = Path.Combine(_path, lang);
                        string destPath = Path.Combine(destFolder, Path.GetFileName(file));
                        File.Move(file, destPath);

                        lock (_lock)
                        {
                            moveLog.Add((file, destPath));
                        }
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                });
            }
            catch (OperationCanceledException)
            {
                for (int i = moveLog.Count - 1; i >= 0; i--)
                {
                    File.Move(moveLog[i].from, moveLog[i].to, true);
                }
                throw;

            }
        }
    }
}