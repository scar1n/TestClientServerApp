﻿using System.Net;

namespace Client
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<string> filePaths;
            List<Task> fileCheckTasks = new List<Task>();
            Console.Write($"Введите путь к директории: ");
            var path = Console.ReadLine();


            if (Directory.Exists(path))
            {
                filePaths = new List<string>();
                filePaths.AddRange(Directory.GetFiles(path, "*.txt"));

                Console.WriteLine("Проверка файлов запущена.");
                foreach (var filePath in filePaths)
                    fileCheckTasks.Add(IsPalindromeCheckProcessAsync(filePath));

                Task.WaitAll(fileCheckTasks.ToArray());
                Console.WriteLine("Проверка файлов окончена.");
            }
            else
            {
                Console.WriteLine($"Каталога {path} не существует.");
            }

            Console.ReadKey();
        }
        static async Task IsPalindromeCheckProcessAsync(string filePath)
        {
            string text = GetTextFromFile(filePath);
            string result = "";

            try
            {
                result = await GetIsPalindromeAsync(text);
            }
            catch (Exception ex)
            {
                result = $"Ошибка: {ex.Message} ";
            }
            finally
            {
                await Console.Out.WriteLineAsync($"Путь к файлу: {filePath}\n" +
                    $"Результат проверки: {result}");
            }
        }

        static string GetTextFromFile(string filePath)
        {
            using (StreamReader sr = new StreamReader(filePath))
                return sr.ReadToEnd();
        }
        static async Task<string> GetIsPalindromeAsync(string text)
        {
            const int attemptCount = 100;
            const int nextAttemptDelay = 1500;

            using (HttpClient client = new HttpClient())
            {
                for (int i = 0; i < attemptCount; i++)
                {
                    var response = await client.GetAsync($"https://localhost:7162/PalindromeCheck?text={text}");

                    if (response.IsSuccessStatusCode)
                        return await response.Content.ReadAsStringAsync();
                    else if (response.StatusCode == HttpStatusCode.ServiceUnavailable)
                        await Task.Delay(nextAttemptDelay);
                    else
                        throw new Exception($"Ошибка при выполнении запроса ({response.StatusCode})");
                }

                throw new Exception("Время ожидания истекло");
            }
        }
    }
}
