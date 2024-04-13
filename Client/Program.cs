using System.Net;
using System.Runtime.ConstrainedExecution;

namespace Client
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<string> filePaths;
            List<Task> fileCheckTasks = new List<Task>();
            var path = Console.ReadLine();


            if (Directory.Exists(path))
            {
                filePaths = new List<string>();
                filePaths.AddRange(Directory.GetFiles(path));
            }
            else
            {
                Console.WriteLine($"Каталога {path} не существует.");
                return;
            }

            foreach (var filePath in filePaths)
            {
                fileCheckTasks.Add(IsPalindromeCheckProcess(filePath));
            }

            Task.WaitAll(fileCheckTasks.ToArray());
            Console.ReadKey();
        }
        static async Task IsPalindromeCheckProcess(string filePath)
        {
            string fileText = GetTextFromFile(filePath);

            using (HttpClient client = new HttpClient())
            {
                while (true)
                {
                    var response = await client.GetAsync($"https://localhost:7162/PalindromeCheck?text={fileText}");
                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"filePath = {filePath}\n" +
                            $"response = {await response.Content.ReadAsStringAsync()}");
                        return;
                    }
                    else if (response.StatusCode == HttpStatusCode.ServiceUnavailable)
                    {
                        await Task.Delay(1000);
                    }
                    else
                    {
                        Console.WriteLine($"Ошибка {response.StatusCode}");
                        return;
                    }
                }
            }
        }
        
        static string GetTextFromFile(string filePath)
        {
            using (StreamReader sr = new StreamReader(filePath))
                return sr.ReadToEnd();
        }
    }
}
