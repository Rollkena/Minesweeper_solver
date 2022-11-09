using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Saper_1
{
    class Program
    {
        public static int n;
        public static int m;
        static void Main(string[] args)
        {
            IWebDriver driver;
            string us_answ;
            int num_ans;

            Console.WriteLine("Введите уровень сапера:\n1 - (beginner)\n2 - (intermediate)\n3 - (expert)");
            while (!((us_answ = Console.ReadLine()) != null && int.TryParse(us_answ, out num_ans) == true && (num_ans >= 1 && num_ans <= 3))) 
            {
                Console.WriteLine("Проверьте корректность ввода.");
            }
            //выбор сложности

            try
            {
                ChromeOptions chromeOptions = new ChromeOptions();
                chromeOptions.BinaryLocation = "D:\\Program Files\\Google\\Chrome\\Application\\chrome.exe";
                driver = new OpenQA.Selenium.Chrome.ChromeDriver(chromeOptions);
                driver.Navigate().GoToUrl("https://minesweeper.online");
                driver.Manage().Window.Maximize();

                switch (us_answ)
                {
                    case "1":
                        driver.FindElement(By.XPath("//*[@id=\"homepage\"]/div[1]/div[1]/a")).Click();
                        n = 9;
                        m = 9;
                        break;

                    case "2":
                        driver.FindElement(By.XPath("//*[@id=\"homepage\"]/div[1]/div[2]/a")).Click();
                        n = 16;
                        m = 16;
                        break;

                    case "3":
                        driver.FindElement(By.XPath("//*[@id=\"homepage\"]/div[1]/div[3]/a")).Click();
                        n = 30;
                        m = 16;
                        break;
                }
                IWebElement elem;

                int[,] matri = new int[n, m];
                for (int i = 0; i <= n; i++)
                {
                    for(int j = 0; j <= m; j++)
                    {
                        elem = driver.FindElement(By.Id($"cell_{n - 1}_{m - 1}"));
                        string status = elem.GetAttribute("class");
                        if (status.Contains("closed"))
                        {
                            matri[n,m] = -1;
                        }
                        else if (status.Contains("opened"))
                        {
                            matri[n,m] = (int)status[status.Length - 1];
                        }
                    }
                }
                for (int i = 0; i <= (n * m); i++)
                {
                    for (int j = 0; j <= m; j++)
                    {
                        Console.WriteLine(matri[n, m]);
                    }
                }




            }
            catch (Exception ex)
            {
                //Console.WriteLine($"Что-то не так\n( • ᴖ • ｡)");
                Console.WriteLine($"Что-то не так\n( • ᴖ • ｡) {ex.ToString()}");
            }
            //открытие браузера + выбор сложности

            
        }
    }

}
