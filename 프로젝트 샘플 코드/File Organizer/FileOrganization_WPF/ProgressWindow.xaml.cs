using System.Windows;
namespace FileOrganization_WPF;

public partial class ProgressWindow : Window
{
    public event Action CancelRequested;
    public ProgressWindow() { InitializeComponent(); }

    public void UpdateProgress(int percent)
    {
        PercentText.Text = $"파일 정리 {percent}% 완료...";
        Bar.Value = percent;
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        PercentText.Text = "파일 정리 취소 중...";
        CancelRequested?.Invoke();
    }
}