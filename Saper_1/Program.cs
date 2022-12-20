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
        public static int count;
        static void Main(string[] args)
        {
            //выбор режима игры
            int us_choise;
            Console.WriteLine("ВЫберите режим сапера:\n1 - Стандартный режим\n2 - Режим без угадывания");
            while (!((int.TryParse(Console.ReadLine(), out us_choise)) && (us_choise >= 1 && us_choise <= 2)))
            {
                Console.WriteLine("Проверьте корректность ввода.");
            }

            //запуск браузера, выбор сложности
            int us_answ;
            try
            {
                ChromeOptions chromeOptions = new ChromeOptions();
                chromeOptions.BinaryLocation = "D:\\Program Files\\Google\\Chrome\\Application\\chrome.exe";
                //chromeOptions.AddArgument("--headless");
                driver = new OpenQA.Selenium.Chrome.ChromeDriver(chromeOptions);
                //driver.Manage().Window.Maximize();

                //стандартный режим
                if (us_choise == 1)
                {
                    driver.Navigate().GoToUrl("https://minesweeper.online");
                    //запрос сложности
                    Console.WriteLine("Введите уровень сапера:\n1 - Beginner\n2 - Intermediate\n3 - Expert\n");
                    while (!(int.TryParse(Console.ReadLine(), out us_answ) && us_answ >= 1 && us_answ <= 3))
                    {
                        Console.WriteLine("Проверьте корректность ввода.\n");
                    }
                    switch (us_answ)
                    {
                        case 1:
                            driver.FindElement(By.XPath("//*[@id=\"homepage\"]/div[1]/div[1]/a")).Click();
                            n = 9;
                            m = 9;
                            break;

                        case 2:
                            driver.FindElement(By.XPath("//*[@id=\"homepage\"]/div[1]/div[2]/a")).Click();
                            n = 16;
                            m = 16;
                            break;

                        case 3:
                            driver.FindElement(By.XPath("//*[@id=\"homepage\"]/div[1]/div[3]/a")).Click();
                            n = 30;
                            m = 16;
                            break;
                    }
                }

                //режим без угадывания
                else
                {
                    driver.Navigate().GoToUrl("https://minesweeper.online/ru/new-game/ng");
                    //запрос сложности
                    Console.WriteLine("Введите уровень сапера:\n1 - Beginner\n2 - Intermediate\n3 - Expert\n4 - Evil\n");
                    while (!(int.TryParse(Console.ReadLine(), out us_answ) == true && us_answ >= 1 && us_answ <= 4))
                    {
                        Console.WriteLine("Проверьте корректность ввода.");
                    }
                    
                    switch (us_answ)
                    {
                        case 1:
                            driver.FindElement(By.XPath("//*[@id=\"level_select_11\"]/span")).Click();
                            n = 9;
                            m = 9;
                            break;

                        case 2:
                            driver.FindElement(By.XPath("//*[@id=\"level_select_12\"]/span")).Click();
                            n = 16;
                            m = 16;
                            break;

                        case 3:
                            driver.FindElement(By.XPath("//*[@id=\"level_select_13\"]/span")).Click();
                            n = 30;
                            m = 16;
                            break;

                        case 4:
                            driver.FindElement(By.XPath("//*[@id=\"level_select_14\"]/span")).Click();
                            n = 30;
                            m = 20;
                            break;
                    }
                    Thread.Sleep(1000);

                   
                }

                //время на прогрузку страницы
                Thread.Sleep(2000);

                //Алгоритм:
                //Пока Make_Moves что-то изменяет, он зациклен,
                //но как только он не обнаруживает ходов, он запускает рандом
                //после него опять перепроверяет на наличие доступных ходов.
                count = -1;
                char us_ch;


                int win_games = 0;
                int lose_games = 0;
                float stats;
                for (int games = 0; games < 200; games++)
                {
                    Thread.Sleep(4000);
                    try
                    {
                        driver.FindElement(By.XPath($"//*[@id=\"ads_header_close\"]")).Click();
                    }
                    catch{}
                    try
                    {
                        driver.FindElement(By.XPath($"//*[@id=\"top_area_face\"]")).Click();
                        Thread.Sleep(2000);

                        if (us_choise != 1)
                        {
                            //нажимаем точку начала
                            driver.FindElement(By.XPath($"//*[contains(@class, 'cell size24 hd_closed start')]")).Click();
                        }
                        while (driver.FindElement(By.XPath($"//*[@id=\"top_area_face\"]")).GetAttribute("class").Contains("unpressed"))
                        {
                            //стандартный режим - певый ход - открываем до того, как откроем скопление ячеек
                            if (us_choise == 1)
                            {
                                while (count == -1)
                                {
                                    Random rnd = new Random();
                                    int rnd_n = rnd.Next(0, n - 1);
                                    int rnd_m = rnd.Next(0, m - 1);
                                    driver.FindElement(By.XPath($"//*[@id=\"cell_{rnd_n}_{rnd_m}\"]")).Click();
                                    string status = driver.FindElement(By.XPath($"//*[@id=\"cell_{rnd_n}_{rnd_m}\"]")).GetAttribute("class");
                                    if ((status[status.Length - 1] - '0' == 0)) count = -2;
                                }
                            }
                            //основной повторяющийся алгоритм
                            //повторяяется два раза для того, чтобы обработать все варианты на поле
                            count = 0;
                            Make_Moves(New_Matrix());
                            int[,] matrix = Make_Moves(New_Matrix());

                            //даем возможность пользователю помочь алгоритму; если нет - он запускает рандом.
                            if (count == 0)
                            {
                                //Console.WriteLine("Алгоритм не смог найти ход. Хотите помочь? (y/n)");
                                //while (!((char.TryParse(Console.ReadLine(), out us_ch)) && (us_ch =='y' || us_ch == 'n')))
                                //{
                                //    Console.WriteLine("Проверьте корректность ввода.");
                                //}
                                //if (us_ch == 'y')
                                if (false)
                                {
                                    Console.WriteLine("Нажмите Enter для продолжения.");
                                    Console.ReadLine();
                                }
                                else
                                {
                                    int rnd_n, rnd_m;
                                    while (count == 0)
                                    {
                                        int find_one = 0;
                                        Random rnd = new Random();

                                        for (int i = 0; i < m; i++)
                                        {
                                            for (int j = 0; j < n; j++)
                                            {
                                                if (matrix[i, j] == 1)
                                                {
                                                    for (int i_check = -1; i_check < 2; i_check++)
                                                    {
                                                        if (find_one == 0)
                                                        {
                                                            for (int j_check = -1; j_check < 2; j_check++)
                                                            {
                                                                if (Is_Valid(i + i_check, j + j_check) && matrix[i + i_check, j + j_check] == -1)
                                                                {
                                                                    driver.FindElement(By.XPath($"//*[@id=\"cell_{j + j_check}_{i + i_check}\"]")).Click();
                                                                    count++;
                                                                    find_one++;
                                                                    break;
                                                                }
                                                            }
                                                        }
                                                    }
                                                    if (find_one != 0) break;
                                                }
                                                if (find_one != 0) break;
                                            }
                                            if (find_one != 0) break;
                                        }
                                        if (find_one == 0)
                                        {
                                            int clik_on = 0;
                                            while (clik_on == 0)
                                            {
                                                rnd_n = rnd.Next(0, n - 1);
                                                rnd_m = rnd.Next(0, m - 1);
                                                if (Is_Valid(rnd_m, rnd_n) && matrix[rnd_m, rnd_n] == -1)
                                                {
                                                    driver.FindElement(By.XPath($"//*[@id=\"cell_{rnd_n}_{rnd_m}\"]")).Click();
                                                    count++;
                                                    clik_on++;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"\n\nЧто-то не так\n(о _ о )\n {ex.ToString()},   {ex.Source}\n\n");
                    }
                    Console.WriteLine($"Игра №{games+1}\n");
                    if (driver.FindElement(By.XPath($"//*[@id=\"top_area_face\"]")).GetAttribute("class").Contains("win"))
                        win_games++;
                    else if (driver.FindElement(By.XPath($"//*[@id=\"top_area_face\"]")).GetAttribute("class").Contains("lose"))
                        lose_games++;
                    Console.WriteLine($"\n\nВыйгранные игры = {win_games}\nПроигранные игры = {lose_games}\n");
                }
                //Console.WriteLine("Алгоритм завершил свою работу");
                Console.WriteLine($"\n\nВыйгранные игры = {win_games}\nПроигранные игры = {lose_games}\n");
                if (lose_games != 0)
                {
                    stats = win_games / lose_games;
                    Console.WriteLine($"\nСтатистика побед = {stats}");
                }
                else
                    Console.WriteLine($"\nСтопроцентная победа!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n\nЧто-то не так\n(о _ о )\n {ex.ToString()}\n\n");
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

            //мины = значение в клетке -> ставим флаги
            static int[,] Set_Flag(int col, int row, int[,] matrix)
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
                            count++;
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
                            count++;
                        }
                    }
                }
                return (matrix);
            }

            //1_2_1 по одной стороне
            static int[,] Cells_1_2_1(int col, int row, int[,] matrix)
            {
                int old_count = count;
                Actions actions = new Actions(driver);
                if ((matrix[col, row - 1] == 1  && matrix[col, row + 1] == 1) && (Unrevealed_Count(col, row, matrix)[0] + Unrevealed_Count(col, row, matrix)[1]) == 3)
                {
                    if (matrix[col + 1, row - 1] == -1 && matrix[col + 1, row] == -1 && matrix[col + 1, row + 1] == -1)
                    {
                        actions.ContextClick(driver.FindElement(By.XPath($"//*[@id=\"cell_{row + 1}_{col + 1}\"]"))).Perform();
                        actions.ContextClick(driver.FindElement(By.XPath($"//*[@id=\"cell_{row - 1}_{col + 1}\"]"))).Perform();
                        matrix[col + 1, row + 1] = -5;
                        matrix[col + 1, row -1] = -5;
                        count++;
                    }
                    else if (matrix[col - 1, row - 1] == -1 && matrix[col - 1, row] == -1 && matrix[col - 1, row + 1] == -1)
                    {
                        actions.ContextClick(driver.FindElement(By.XPath($"//*[@id=\"cell_{row + 1}_{col - 1}\"]"))).Perform();
                        actions.ContextClick(driver.FindElement(By.XPath($"//*[@id=\"cell_{row - 1}_{col - 1}\"]"))).Perform();
                        matrix[col - 1, row + 1] = -5;
                        matrix[col - 1, row - 1] = -5;
                        count++;
                    }
                }
                else if ((matrix[col - 1, row] == 1 && matrix[col + 1, row] == 1) && (Unrevealed_Count(col, row, matrix)[0] + Unrevealed_Count(col, row, matrix)[1]) == 3) 
                {
                    if (matrix[col - 1, row + 1] == -1 && matrix[col, row + 1] == -1 && matrix[col + 1, row + 1] == -1)
                    {
                        actions.ContextClick(driver.FindElement(By.XPath($"//*[@id=\"cell_{row + 1}_{col + 1}\"]"))).Perform();
                        actions.ContextClick(driver.FindElement(By.XPath($"//*[@id=\"cell_{row + 1}_{col - 1}\"]"))).Perform();
                        matrix[col + 1, row + 1] = -5;
                        matrix[col - 1, row + 1] = -5;
                        count++;
                    }
                    else if (matrix[col - 1, row - 1] == -1 && matrix[col, row - 1] == -1 && matrix[col + 1, row - 1] == -1)
                    {
                        actions.ContextClick(driver.FindElement(By.XPath($"//*[@id=\"cell_{row - 1}_{col + 1}\"]"))).Perform();
                        actions.ContextClick(driver.FindElement(By.XPath($"//*[@id=\"cell_{row - 1}_{col - 1}\"]"))).Perform();
                        matrix[col + 1, row - 1] = -5;
                        matrix[col - 1, row - 1] = -5;
                        count++;
                    }
                }
                if(old_count != count)
                    matrix = New_Matrix();

                return (matrix);
            }


            //производим ходы
            static int[,] Make_Moves(int[,] matrix)
            {
                for (int i = 0; i < m; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        if (matrix[i, j] > 0 && matrix[i, j] == (Unrevealed_Count(i, j, matrix)[0] + Unrevealed_Count(i, j, matrix)[1]))
                        {
                            matrix = Set_Flag(i, j, matrix);
                        }
                        if (matrix[i, j] > 0 && matrix[i, j] == Unrevealed_Count(i, j, matrix)[1])
                        {
                            matrix = Revel_Cell(i, j, matrix);
                        }
                        try
                        {
                            if (/*count == 0 && */(
                            matrix[i, j] == 2 - Unrevealed_Count(i, j, matrix)[1] && ((matrix[i, j - 1] == 1 - Unrevealed_Count(i, j, matrix)[1] && matrix[i, j + 1] == 1 - Unrevealed_Count(i, j, matrix)[1]) ||
                            (matrix[i - 1, j] == 1 - Unrevealed_Count(i, j, matrix)[1] && matrix[i + 1, j] == 1 - Unrevealed_Count(i, j, matrix)[1]))))
                                matrix = Cells_1_2_1(i, j, matrix);
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                }
                return (matrix);
            }

            //считывание значений поля в данный момент в двумерный массив
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
