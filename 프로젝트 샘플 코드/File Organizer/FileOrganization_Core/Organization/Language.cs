namespace FileOrganization_Core.Organization
{
    public class Language : FileOrganizerBase
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

        public override void MoveFiles()
        {
            foreach (var file in Directory.GetFiles(_path))
            {
                string lang = Path.GetFileNameWithoutExtension(file);

                string dest = "";
                if (lang[0] >= '가' && lang[0] <= '힣') dest = "Korean";
                else if (lang[0] >= 'a' && lang[0] <= 'z') dest = "English";

                string destFolder = Path.Combine(_path, dest);
                string destPath = Path.Combine(destFolder, Path.GetFileName(file));

                File.Move(file, destPath);
            }
        }
    }
}