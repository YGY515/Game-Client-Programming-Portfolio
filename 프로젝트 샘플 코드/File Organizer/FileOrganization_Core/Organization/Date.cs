namespace FileOrganization_Core.Organization
{
    public class Date : FileOrganizerBase
    {
        string _path = null;
        int count = 0;
        HashSet<string> fileList = new HashSet<string>();

        public override string Organize(string path)
        {
            _path = path;

            CollectFiles();
            CreateFolders();
            MoveFiles();
            
            return PrintLog(count, fileList.Count);
        }

        public override void CollectFiles()
        {
            foreach (var file in Directory.GetFiles(_path))
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
            foreach (var file in Directory.GetFiles(_path))
            {
                var info = new FileInfo(file);
                string folder = info.LastWriteTime.ToString("yyyy-MM");

                string destFolder = Path.Combine(_path, folder);
                string destPath = Path.Combine(destFolder, Path.GetFileName(file));

                File.Move(file, destPath);
            }
        }
    }
}