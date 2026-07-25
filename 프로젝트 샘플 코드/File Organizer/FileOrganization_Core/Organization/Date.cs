using System.Linq.Expressions;

namespace FileOrganization_Core.Organization
{
    public class Date : FileOrganizerBase
    {
        string _path = null;
        object _lock = new object();
        int count = 0;

        HashSet<string> fileList = new HashSet<string>();
        List<String> files = new List<string>();
        List<(string to, string from)> moveLog = new List<(string, string)>();

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
                        var info = new FileInfo(file);
                        string date = info.LastWriteTime.ToString("yyyy-MM");
                        Thread.Sleep(5000);

                        string destFolder = Path.Combine(_path, date);
                        string destPath = Path.Combine(destFolder, Path.GetFileName(file));

                        File.Move(file, destPath, true);
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
                for(int i = moveLog.Count - 1; i >= 0; i--)
                {
                    File.Move(moveLog[i].from, moveLog[i].to, true);
                }
                throw;

            }
        }
    }
}
