namespace FileOrganization_Core.Organization
{
    public class Language : FileOrganizerBase
    {
        string _path = null;
        object _lock = new object();
        int _count = 0;

        HashSet<string> fileList = new HashSet<string>();
        List<String> files = new List<string>();
        List<(string from, string to)> moveLog = new List<(string from, string to)>();

        public override string Organize(string path, CancellationToken token, IProgress<int> progress)
        {
            _path = path;
            files = Directory.GetFiles(_path).ToList();

            CollectFiles();
            CreateFolders();
            MoveFiles(token, progress);
            
            return PrintLog(_count, fileList.Count); 
        }

        public override void CollectFiles()
        {
            foreach (var file in files)
            {
                _count++;
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
        public override void MoveFiles(CancellationToken token, IProgress<int> progress)
        {
            var options = new ParallelOptions { CancellationToken = token };
            SemaphoreSlim semaphore = new SemaphoreSlim(4);
            int done = 0;

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
                        Thread.Sleep(2000);  //취소 테스트용

                        string destFolder = Path.Combine(_path, lang);
                        string destPath = Path.Combine(destFolder, Path.GetFileName(file));
                        File.Move(file, destPath);

                        lock (_lock)
                        {
                            moveLog.Add((file, destPath));
                        }

                        int current = Interlocked.Increment(ref done);
                        progress?.Report(1);
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