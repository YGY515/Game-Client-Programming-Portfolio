namespace FileOrganization_Core
{
    public abstract class FileOrganizerBase
    {
        public abstract string Organize(string path);

        public abstract void CollectFiles();

        public abstract void CreateFolders();

        public abstract void MoveFiles();

        public string PrintLog(int fileNum, int folderNum)
        {
            return ($"파일 {fileNum}개를 정리하고 폴더 {folderNum}개를 생성했습니다.");
        }
    }
}