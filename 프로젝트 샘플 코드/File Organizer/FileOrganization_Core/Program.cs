using FileOrganization_Core;
using FileOrganization_Core.Organization;

class Program
{
    static FileOrganizerBase fileOrganizer = null;
    static void Main(string[] args)
    {
        GetDirectory();
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
        Console.WriteLine("폴더를 정리할 기준을 골라주세요. 확장자 or 날짜 or 파일명 언어?");
        Console.WriteLine("날짜의 경우, YYYY-MM 기준으로 정리됩니다.");
        Console.WriteLine("파일명 언어의 경우, 한글이면 Korean, 영어면 English로 분류됩니다.");
        string mode = Console.ReadLine();

        switch (mode)
        {
            case "확장자":
                fileOrganizer = new Extension();
                break;

            case "날짜":
                fileOrganizer = new Date();
                break;

            case "파일명 언어":
                fileOrganizer = new Language();
                break;
        }

        string result = fileOrganizer.Organize(path);
        Console.WriteLine(result);
    }
}