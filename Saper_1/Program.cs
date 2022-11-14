using System;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;

namespace Saper_1
{
    class Program
    {
        public static int n;
        public static int m;
        public static IWebDriver driver;
        static void Main(string[] args)
        {
            int us_choise;
            Console.WriteLine("ВЫберите режим сапера:\n1 - Стандартный режим\n2 - Режим без угадывания");
            while (!(int.TryParse(Console.ReadLine(), out us_choise)) && (us_choise >= 1 && us_choise <= 2))
            {
                Console.WriteLine("Проверьте корректность ввода.");
            }
            switch (us_choise)
            {
                case 1:

                case 2:

            }

            string us_answ;
            int num_ans;

            //запрос сложности
            Console.WriteLine("Введите уровень сапера:\n1 - Beginner\n2 - Intermediate\n3 - Expert");
            while (!((us_answ = Console.ReadLine()) != null && int.TryParse(us_answ, out num_ans) == true && (num_ans >= 1 && num_ans <= 3))) 
            {
                Console.WriteLine("Проверьте корректность ввода.");
            }

            //запуск браузера, выбор сложности
            try
            {
                ChromeOptions chromeOptions = new ChromeOptions();
                chromeOptions.BinaryLocation = "D:\\Program Files\\Google\\Chrome\\Application\\chrome.exe";
                //chromeOptions.AddArgument("--headless");
                driver = new OpenQA.Selenium.Chrome.ChromeDriver(chromeOptions);
                driver.Navigate().GoToUrl("https://minesweeper.online");
                //driver.Manage().Window.Maximize();

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

                //время на прогрузку страницы
                Thread.Sleep(2000);

                //играем пока играется
                while (driver.FindElement(By.XPath($"//*[@id=\"top_area_face\"]")).GetAttribute("class").Contains("unpressed"))
                {
                    int count = 0;
                    //открываем до скопления
                    while (count != -1)
                    {
                        Random rnd = new Random();
                        int rnd_n = rnd.Next(0, n - 1);
                        int rnd_m = rnd.Next(0, m - 1);
                        driver.FindElement(By.XPath($"//*[@id=\"cell_{rnd_n}_{rnd_m}\"]")).Click();
                        string status = driver.FindElement(By.XPath($"//*[@id=\"cell_{rnd_n}_{rnd_m}\"]")).GetAttribute("class");
                        if ((status[status.Length - 1] - '0' == 0)) count = -1;
                    }
                    Obvious_Moves(New_Matrix());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n\nЧто-то не так\n( • ᴖ • ｡) {ex.ToString()}\n\n");
            }

            //проверяет существуют ли клетки (для отброса клеток у границ)
            static bool Is_Valid(int col, int row)
            {
                return ((row >= 0) && (row < n) && (col >= 0) && (col < m));
            }

            //считает количество не открытх клеток + флажки в окружающих слот клетках
            static int[] Unrevealed_Count(int col, int row, int[,] matrix)
            {
                int count_blank = 0;
                int count_flag = 0;
                for (int i = -1; i < 2; i++)
                {
                    for (int j = -1; j < 2; j++)
                    {
                        if (Is_Valid(col + i, row + j) && matrix[col + i, row + j] == -1)
                            count_blank++;
                        if (Is_Valid(col + i, row + j) && matrix[col + i, row + j] == -5)
                            count_flag++;
                    }
                }
                int[] counts = new int[] { count_blank, count_flag};
                return counts;
            }

            //мины == количество в клетке -> ставим флаг
            static int[,] Mines_Eq_Value(int col, int row, int[,] matrix)
            {
                Actions actions = new Actions(driver);
                for (int a = -1; a < 2; a++)
                {
                    for (int b = -1; b < 2; b++)
                    {
                        if (Is_Valid(col + a, row + b) && matrix[col + a, row + b] == -1)
                        {
                            actions.ContextClick(driver.FindElement(By.XPath($"//*[@id=\"cell_{row + b}_{col + a}\"]"))).Perform();
                            matrix[col + a, row + b] = -5;
                        }
                    }
                }
                return (matrix);
            }

            //открывает безопасные клетки вокруг одной определенной
            static int[,] Revel_Cell(int col, int row, int[,] matrix)
            {
                for (int a = -1; a < 2; a++)
                {
                    for (int b = -1; b < 2; b++)
                    {
                        if (Is_Valid(col + a, row + b) && matrix[col + a, row + b] == -1)
                        {
                            driver.FindElement(By.XPath($"//*[@id=\"cell_{row + b}_{col + a}\"]")).Click();
                            matrix = New_Matrix();
                        }
                    }
                }
                return (matrix);
            }

            //1_2_1 по одной стороне
            static int[,] Cells_1_2_1(int col, int row, int[,] matrix)
            {
                Actions actions = new Actions(driver);
                if (((matrix[col, row - 1] == 1 && matrix[col, row] == 2 && matrix[col, row + 1] == 1) ||
                    (matrix[col - 1, row] == 1 && matrix[col, row] == 2 && matrix[col + 1, row] == 1)) &&
                    Unrevealed_Count(col, row, matrix)[0] + Unrevealed_Count(col, row, matrix)[1] == 3)
                {
                    for (int a = -1; a < 2; a += 2)
                    {
                        for (int b = -1; b < 2; b += 2)
                        {
                            if (Is_Valid(col + a, row + b) && matrix[col + a, row + b] == -1)
                            {
                                actions.ContextClick(driver.FindElement(By.XPath($"//*[@id=\"cell_{row + b}_{col + a}\"]"))).Perform();
                                matrix[col + a, row + b] = -5;
                            }
                        }
                    }
                    matrix = New_Matrix();
                }
                return (matrix);
            }

            //очевидные решения
            static void Obvious_Moves(int[,] matrix)
            {
                for (int i = 0; i < m; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        if (matrix[i, j] > 0 && matrix[i, j] == (Unrevealed_Count(i, j, matrix)[0] + Unrevealed_Count(i, j, matrix)[1]))
                            matrix = Mines_Eq_Value(i, j, matrix);
                        if (matrix[i, j] > 0 && matrix[i, j] == Unrevealed_Count(i, j, matrix)[1])
                            matrix = Revel_Cell(i, j, matrix);
                        //if (matrix[i, j] == 2)
                        //    matrix = Cells_1_2_1(i, j, matrix);
                    }
                }
            }

            // считывание матрицы значений поля в данный момент в двумерный массив
            static int[,] New_Matrix()
            {
                IWebElement elem;
                int[,] matrix = new int[m, n];
                for (int i = 0; i < m; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        elem = driver.FindElement(By.XPath($"//*[@id=\"cell_{j}_{i}\"]"));
                        string status = elem.GetAttribute("class");
                        //флаг
                        if (status.Contains("flag"))
                        {
                            matrix[i, j] = -5;
                        }
                        //закрытые
                        else if (status.Contains("closed"))
                        {
                            matrix[i, j] = -1;
                        }
                        //бомба (для отладки)
                        else if (status.Contains("opened") && (int)(status[status.Length - 2] - '0') == 1)
                        {
                            matrix[i, j] = -2;
                        }
                        //значение клетки
                        else if (status.Contains("opened"))
                        {
                            matrix[i, j] = (int)(status[status.Length - 1] - '0');
                        }
                    }
                }
                return (matrix);
            }
        }
    }
}
