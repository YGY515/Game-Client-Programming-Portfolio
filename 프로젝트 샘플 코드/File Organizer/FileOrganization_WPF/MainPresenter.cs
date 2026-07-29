using FileOrganization_Core;
using FileOrganization_Core.Organization;
using System.IO;

namespace FileOrganization_WPF;

class MainPresenter
{
    private readonly IMainView _view;
    CancellationTokenSource cts = new CancellationTokenSource();

    public MainPresenter (IMainView view)
    {
        _view = view;
    }

    public async void OnOrganizeClicked()
    {
        cts = new CancellationTokenSource();
        string path = _view.SelectedPath;
        string mode = _view.SelectedMode;

        // 진행률 기능
        var progressWindow = new ProgressWindow();
        progressWindow.CancelRequested += () => cts.Cancel();
        progressWindow.Show();

        // 입력된 주소의 여러 하위 폴더 순회
        var folders = new List<string>() { path };
        folders.AddRange(Directory.GetDirectories(path));
        int totalFiles = folders.Sum(f => Directory.GetFiles(f).Length);
        int totalDone = 0;

        if (path == null || Directory.Exists(path) == false)
        {
            _view.ShowError("올바른 경로를 입력하세요.");
            return;
        }

        try
        {
            var results = new List<string>();
            object resultLock = new object();
            var options = new ParallelOptions { CancellationToken = cts.Token };

            await Task.Run(() =>
            {
                Parallel.ForEach(folders, options, folder =>
                {
                    FileOrganizerBase organizer = mode switch
                    {
                        "확장자" => new Extension(),
                        "날짜" => new Date(),
                        "언어" => new Language(),
                        _ => throw new NotImplementedException()
                    };

                    var progress = new Progress<int>(_ =>
                    {
                        int current = Interlocked.Increment(ref totalDone);
                        int percent = totalFiles == 0 ? 100 : (int)((double)current / totalFiles * 100);

                        if (cts.IsCancellationRequested) return;    // 취소 요청 시 중단
                        progressWindow.Dispatcher.Invoke(() => progressWindow.UpdateProgress(percent));
                    });

                    string result = organizer.Organize(folder, cts.Token, progress);
                    lock (resultLock) { results.Add($"[{folder}] {result}"); }
                });
            });

            _view.ShowResult(string.Join("\n", results));
        }
        catch (OperationCanceledException)
        {
            _view.ShowError("작업을 취소하였습니다.");
        }
        finally
        {
            progressWindow.Close();
        }
    }

    public void OnCancelCliked()
    {
        if (cts != null)
            cts.Cancel();
    }
}