using System.Windows;
using FileOrganization_Core;
using FileOrganization_Core.Organization;
using System.IO;
using System.Diagnostics;

namespace FileOrganization_WPF;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    FileOrganizerBase organizer = null;
    string path = null;

    public MainWindow()
    {
        InitializeComponent();
    }

    private void SelectFolder_Click(object sender, RoutedEventArgs e)
    {
        FolderBrowserDialog dialog = new FolderBrowserDialog();

        if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        {
            path = dialog.SelectedPath;
            if (Directory.Exists(path) == false)
            {
                System.Windows.MessageBox.Show("올바른 폴더를 입력하세요");
            }
        }

        PathDisplay.Text = path;
    }

    private void OrganizeButton_Click(object sender, RoutedEventArgs e)
    {
        if (ExtensionRadio.IsChecked == true)
            organizer = new Extension();
        else if (DateRadio.IsChecked == true)
            organizer = new Date();
        else if (LanguageRadio.IsChecked == true)
            organizer = new Language();

        string result = organizer.Organize(path);

        var dialoug = System.Windows.MessageBox.Show(
                result + "\n\n정리된 폴더를 열어보시겠습니까?",
                "정리 완료",
                MessageBoxButton.YesNo
            );

        if (dialoug == MessageBoxResult.Yes)
            Process.Start("explorer.exe", path);
    }
}