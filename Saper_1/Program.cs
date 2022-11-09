using System;
using OpenQA.Selenium;

namespace Saper_1
{
    class Program
    {
        static void Main(string[] args)
        {
            IWebDriver driver;


            Console.WriteLine("Введите уровень сапера:\n1 - (beginner)\n2 - (Intermediate)\n3 - Expert");
            Console.ReadLine();

            try
            {
                driver = new OpenQA.Selenium.Firefox.FirefoxDriver();

                driver.Navigate().GoToUrl("https://www.chess.com/login_and_go?returnUrl=https://www.chess.com/");

                driver.Manage().Window.Maximize();
            }
            catch
            {
                Console.WriteLine("Что-то пошло не так\n( • ᴖ • ｡) ");
            }
        }
    }
}
