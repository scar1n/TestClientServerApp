using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;

namespace Server.Controllers
{
    [ApiController]
    [Route("[controller]")]

    public class PalindromeCheckController : Controller
    {
        static public bool IsPalindrome(string checkString)
        {
            if (checkString == null) return false;
            for (int i = 0; i < checkString.Length / 2; i++)
            {
                if (checkString[i] != checkString[checkString.Length - 1 - i])
                    return false;
            }
            return true;
        }

        [HttpGet(Name = "PalindromeChecker")]
        public async Task<string> PalindromeChecker(string text)
        {
            await Task.Delay(5000);
            if (IsPalindrome(text))
                return "Полученная строка является палиндромом.";
            else
                return "Полученная строка не является палиндромом.";
        }
    }
}
