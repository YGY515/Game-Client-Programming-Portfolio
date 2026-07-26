namespace FileOrganization_WPF
{
    public interface IMainView
    {
        string SelectedPath { get; }
        string SelectedMode { get; }
        void ShowResult(string message);
        void ShowError(string message);
    }
}
