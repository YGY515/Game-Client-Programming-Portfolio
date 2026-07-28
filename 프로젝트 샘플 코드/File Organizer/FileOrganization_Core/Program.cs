using FileOrganization_Core;
using FileOrganization_Core.Organization;
using System.Threading.Channels;

class Program
{
    static void Main(string[] args)
    {
        GetDirectory();
    }

    public class SyncProgress<T> : IProgress<T>
    {
        private readonly Action<T> _handler;
        public SyncProgress(Action<T> handler) => _handler = handler;
        public void Report(T value) => _handler(value);
    }

    public static void GetDirectory()
    {
        string path = null;
        Console.Write("파일 경로를 입력해주세요: ");
        while (true)
        {
            path = Console.ReadLine();

            if (Directory.Exists(path) == true)
            {
                break;
            }
            else
            {
                Console.WriteLine("올바른 경로를 입력해주세요");
            }
        }

        Console.WriteLine();
        Console.WriteLine("폴더를 정리할 기준을 골라주세요. 확장자 / 날짜 / 언어");
        Console.WriteLine("날짜의 경우, YYYY-MM 기준으로 정리됩니다.");
        Console.WriteLine("언어의 경우, 파일 이름이 한글이면 Korean, 영어면 English로 분류됩니다.");
        Console.WriteLine("ESC를 누르면 정리를 취소합니다.");
        string mode = Console.ReadLine();

        var folders = new List<string>() { path };    // 입력한 경로의 모든 폴더
        folders.AddRange(Directory.GetDirectories(path));
        CancellationTokenSource cts = new CancellationTokenSource();

        Task.Run(() =>
        {
            while(cts.IsCancellationRequested == false)
            {
                if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                {
                    cts.Cancel();
                    Console.WriteLine("취소되었습니다.");
                    break;
                }    
            }
        });

        int totalFiles = folders.Sum(f => Directory.GetFiles(f).Length);
        int totalDone = 0;
        object progressLock = new object();

        Parallel.ForEach(folders, folder =>
        {
            // 폴더마다 새 인스턴스 생성
            FileOrganizerBase organizer = mode switch
            {
                "확장자" => new Extension(),
                "날짜" => new Date(),
                "언어" => new Language(),
                _ => throw new NotImplementedException()
            };
            try
            { 
                // 진행율 표시
                var progress = new SyncProgress<int>(_ =>
                {
                    int current = Interlocked.Increment(ref totalDone);
                    int percent = (int)((double)current / totalFiles * 100);
                    lock (progressLock)
                    {
                        Console.WriteLine($"\r파일 정리 {percent}% 완료...");
                    }
                });

                string result = organizer.Organize(folder, cts.Token, progress);
                Console.WriteLine($"[{folder}] {result}");
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine($"[{folder}] 폴더 정리가 취소되었습니다.");
            }
        });
    }
}