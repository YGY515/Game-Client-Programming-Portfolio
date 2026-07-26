using FileOrganization_Core;
using FileOrganization_Core.Organization;
using System.IO;

namespace FileOrganization_WPF
{
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

            if (path == null || Directory.Exists(path) == false)
            {
                _view.ShowError("올바른 경로를 입력하세요.");
                return;
            }

            FileOrganizerBase organizer = _view.SelectedMode switch
            {
                "확장자" => new Extension(),
                "날짜" => new Date(),
                "언어" => new Language(),
                _ => throw new NotImplementedException()
            };

            if (organizer == null)
            {
                _view.ShowError("올바른 정리 기준을 고르세요.");
                return;
            }

            string result = await Task.Run(() => organizer.Organize(path, cts.Token));
            _view.ShowResult(result);
        }

        public void OnCancelCliked()
        {
            if (cts != null)
                cts.Cancel();
        }
    }
}
