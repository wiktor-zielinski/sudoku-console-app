using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace sudoku_console_app
{
    internal class Program
    {
        static string solvedSudokuFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\solvedSudokus.csv");
        static string unsolvedSudokuFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\unsolvedSudokus.csv");
        private static Random random = new Random();
        const int MenuWidth = 75;

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;


            while (true)
            {
                Console.Clear();
                PrintTopBorder();
                PrintCenteredText("APLIKACJA SUDOKU");
                PrintMiddleBorder();
                PrintEmptyLine();
                PrintLeftText("Wybierz na której bazie chcesz pracować:");
                PrintEmptyLine();
                PrintLeftText("A - Baza rozwiązanych sudoku");
                PrintLeftText("B - Baza nierozwiązanych");
                PrintEmptyLine();
                PrintLeftText("X - Wyjdź z programu");
                PrintEmptyLine();
                PrintBottomBorder();

                Console.Write("\nTwój wybór: ");
                string choice = Console.ReadLine()?.Trim().ToUpper();

                if (choice == "X") break;
                else if (choice == "A") MenuRozwiazane();
                else if (choice == "B") MenuNierozwiazane();
            }
        }

        static void MenuRozwiazane()
        {
            while (true)
            {
                Console.Clear();
                PrintTopBorder();
                PrintCenteredText("BAZA ROZWIĄZANYCH SUDOKU");
                PrintMiddleBorder();
                PrintCenteredText("USUWANIE");
                PrintMiddleBorder();
                PrintEmptyLine();
                PrintLeftText("01 - Wyzeruj baze");
                PrintLeftText("02 - Usuniecie danego wpisu z bazy");
                PrintLeftText("03 - Pozytywnie wyczyscic baze (usunięcie nieprawidłowych)");
                PrintLeftText("04 - Negatywnie wyczyscic baze (usunięcie prawidłowych)");
                PrintEmptyLine();
                PrintMiddleBorder();
                PrintCenteredText("DODAWANIE / PRZETWARZANIE");
                PrintMiddleBorder();
                PrintEmptyLine();
                PrintLeftText("1 - Generuj nowe wpisy do bazy");
                PrintLeftText("2 - Wyswietl rozwiazania sudoku (wszystkie/wybrane)");
                PrintLeftText("3 - Wypisz statystyki bazy");
                PrintLeftText("4 - Reczne wpisanie rozwiazanego sudoku");
                PrintLeftText("5 - Dekonstrukcja i animacja rozwiazania krok-po-kroku");
                PrintEmptyLine();
                PrintMiddleBorder();
                PrintEmptyLine();
                PrintLeftText("0 - Powrót");
                PrintEmptyLine();
                PrintBottomBorder();

                Console.Write("\nWybierz opcję: ");
                string choice = Console.ReadLine()?.Trim();

                if (choice == "0") break;

                switch (choice)
                {
                    case "0":
                        Console.WriteLine("\nZamykanie programu");
                        return;
                    case "01":
                        File.WriteAllText(solvedSudokuFilePath, string.Empty);
                        Console.WriteLine("Baza wyzerowana.");
                        Thread.Sleep(1000);
                        break;
                    case "02":
                        DeleteEntry(solvedSudokuFilePath);
                        break;
                    case "03":
                        FilterDatabase(solvedSudokuFilePath, true);
                        break;
                    case "04":
                        FilterDatabase(solvedSudokuFilePath, false);
                        break;
                    case "1":
                        Console.Write("Podaj liczbe sudoku do wygenerowania [1-20]: ");
                        if (int.TryParse(Console.ReadLine(), out int count) && count >= 1 && count <= 20)
                        {
                            GenerateAndSaveSudokus(count);
                            Console.WriteLine($"Wygenerowano {count} wpisow.");
                            Thread.Sleep(1000);
                        }
                        break;
                    case "2":
                        DisplayDatabase(solvedSudokuFilePath);
                        break;
                    case "3":
                        ShowStats(solvedSudokuFilePath);
                        break;
                    case "4":
                        ManualEntry(solvedSudokuFilePath);
                        break;
                    case "5":
                        AnimateSudoku(solvedSudokuFilePath, isSolved: true);
                        break;
                }
            }
        }

        static void MenuNierozwiazane()
        {
            while (true)
            {
                Console.Clear();
                PrintTopBorder();
                PrintCenteredText("BAZA NIEROZWIĄZANYCH SUDOKU");
                PrintMiddleBorder();
                PrintCenteredText("USUWANIE");
                PrintMiddleBorder();
                PrintEmptyLine();
                PrintLeftText("01 - Wyzeruj baze");
                PrintLeftText("02 - Usuniecie danego wpisu z bazy");
                PrintEmptyLine();
                PrintMiddleBorder();
                PrintCenteredText("DODAWANIE / PRZETWARZANIE");
                PrintMiddleBorder();
                PrintEmptyLine();
                PrintLeftText("1 - Tworzenie nowej nierozwiazanej planszy (Easy/Medium/Hard)");
                PrintLeftText("2 - Wyswietl dane nierozwiazane sudoku (wszystkie/wybrane)");
                PrintLeftText("3 - Konstrukcja krok-po-kroku wybranego nierozwiazanego sudoku");
                PrintEmptyLine();
                PrintMiddleBorder();
                PrintEmptyLine();
                PrintLeftText("0 - Powrót");
                PrintEmptyLine();
                PrintBottomBorder();

                Console.Write("\nWybierz opcję: ");
                string choice = Console.ReadLine()?.Trim();

                if (choice == "0") break;

                switch (choice)
                {
                    case "01":
                        File.WriteAllText(unsolvedSudokuFilePath, string.Empty);
                        Console.WriteLine("Baza wyzerowana.");
                        Thread.Sleep(1000);
                        break;
                    case "02":
                        DeleteEntry(unsolvedSudokuFilePath);
                        break;
                    case "1":
                        GenerateUnsolvedSudoku();
                        break;
                    case "2":
                        DisplayDatabase(unsolvedSudokuFilePath);
                        break;
                    case "3":
                        AnimateSudoku(unsolvedSudokuFilePath, isSolved: false);
                        break;
                }
            }
        }

        
        // ----- Metody do rysowania menu -----

        // Unicode obramowania
        // https://en.wikipedia.org/wiki/Box-drawing_characters

        static string GetLeftMargin()
        {
            int margin = (Console.WindowWidth - MenuWidth) / 2;
            // Zapobiega błędom zwężania terminalu bardziej niż szerokość menu 
            return new string(' ', Math.Max(0, margin));
        }

        // Rysowanie górnego paska obramowania
        static void PrintTopBorder()
        {
            Console.WriteLine(GetLeftMargin() + "╔" + new string('═', MenuWidth - 2) + "╗");
        }

        // Rysowanie środkowego paska obramowania
        static void PrintMiddleBorder()
        {
            Console.WriteLine(GetLeftMargin() + "╠" + new string('═', MenuWidth - 2) + "╣");
        }

        // Rysowanie dolnego paska obramowania
        static void PrintBottomBorder()
        {
            Console.WriteLine(GetLeftMargin() + "╚" + new string('═', MenuWidth - 2) + "╝");
        }

        // Rysowanie pustej lini w menu wraz z obramowaniem
        static void PrintEmptyLine()
        {
            Console.WriteLine(GetLeftMargin() + "║" + new string(' ', MenuWidth - 2) + "║");
        }

        // Rysowanie obramowanie i wyśrodkowanie nagłówka
        static void PrintCenteredText(string text)
        {
            int totalSpaces = MenuWidth - 2 - text.Length;

            if (totalSpaces < 0)
            {
                text = text.Substring(0, MenuWidth - 5) + "...";
                totalSpaces = 0;
            }

            int leftSpaces = totalSpaces / 2;
            int rightSpaces = totalSpaces - leftSpaces;

            Console.WriteLine(GetLeftMargin() + "║" + new string(' ', leftSpaces) + text + new string(' ', rightSpaces) + "║");
        }

        // Rysownie obramowanie i wypisanie zwykłego tekstu menu wyrównanego do prawej
        static void PrintLeftText(string text)
        {
            string prefix = "  ";
            int rightSpaces = MenuWidth - 2 - text.Length - prefix.Length;

            if (rightSpaces < 0)
            {
                text = text.Substring(0, MenuWidth - 5 - prefix.Length) + "...";
                rightSpaces = 0;
            }

            Console.WriteLine(GetLeftMargin() + "║" + prefix + text + new string(' ', rightSpaces) + "║");
        }


        // ----- Metody sudoku -----

        // Generenowanie i zapisywanie sudoku
        static void GenerateAndSaveSudokus(int count)
        {
            // pozwala dopisywać dane do istniejącego pliku
            using (StreamWriter sw = new StreamWriter(solvedSudokuFilePath, append: true))
            {
                // Losowowanie (50/50 poprawna/niepoprawna plansza sudoku)
                for (int i = 0; i < count; i++)
                {
                    int[,] grid = GenerateValidBaseSudoku();

                    if (random.Next(100) < 50)
                    {
                        SpoilSudoku(grid);
                    }

                    // Zapisywanie
                    for (int r = 0; r < 9; r++)
                    {
                        List<string> row = new List<string>();
                        for (int c = 0; c < 9; c++)
                        {
                            row.Add(grid[r, c].ToString());
                        }
                        sw.WriteLine(string.Join(',', row));
                    }
                    sw.WriteLine();
                }
            }
        }

        // Generowanie planszy sudoku do rozwiązania
        static void GenerateUnsolvedSudoku()
        {

        }

        // Generowanie nowej losowej, poprawnej planszy sudoku na podstawie bazowej.
        static int[,] GenerateValidBaseSudoku()
        {
            // Bazowa, poprawna plansza Sudoku
            int[,] grid = {
            {5,3,4,6,7,8,9,1,2},
            {6,7,2,1,9,5,3,4,8},
            {1,9,8,3,4,2,5,6,7},
            {8,5,9,7,6,1,4,2,3},
            {4,2,6,8,5,3,7,9,1},
            {7,1,3,9,2,4,8,5,6},
            {9,6,1,5,3,7,2,8,4},
            {2,8,7,4,1,9,6,3,5},
            {3,4,5,2,8,6,1,7,9}
        };

            // Mieszanie gotowej planszy (zamiana miejscami najpierw wiersze potem kolumny w obrębie bloków 3x9)

            // Mieszanie wierszy
            for (int block = 0; block < 9; block += 3)
            {
                for (int i = 0; i < 3; i++)
                {
                    int row1 = i + block;
                    int row2 = block + random.Next(3);

                    if (row1 != row2)
                    {
                        for (int col = 0; col < 9; col++)
                        {
                            int temp = grid[row1, col];
                            grid[row1, col] = grid[row2, col];
                            grid[row2, col] = temp;
                        }
                    }
                }
            }

            // Mieszanie kolumn
            for (int block = 0; block < 9; block += 3)
            {
                for (int i = 0; i < 3; i++)
                {
                    int col1 = i + block;
                    int col2 = block + random.Next(3);

                    if (col1 != col2)
                    {
                        for (int row = 0; row < 9; row++)
                        {
                            int temp = grid[row, col1];
                            grid[row, col1] = grid[row, col2];
                            grid[row, col2] = temp;
                        }
                    }
                }
            }

            return grid;
        }

        // Podmiana losowej komórki w sudoku na inną niepoprawną liczbę
        static void SpoilSudoku(int[,] grid)
        {
            int row = random.Next(9);
            int col = random.Next(9);

            // Zapewnia zmianę wartości z jednoczesnym zachowaniem przedziału 1-9
            grid[row, col] = (grid[row, col] % 9) + 1;
        }
        // Wypisanie sudoku
        static void PrintGrid(int[,] sudoku)
        {
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    Console.Write(sudoku[row,col] + " ");
                    if(col == 2 || col == 5) Console.Write("| ");
                }
                Console.WriteLine();
                if (row == 2 || row == 5) Console.WriteLine(new string('-', 7) + '+' + new string('-', 7) + '+' + new string('-', 7));
            }
        }
        // Sprawdzanie czy sudoku jest prawidłowo rozwiązane
        static string ValidateSudokuLogic(int[,] sudokuBeingChecked)
        {
            // Sprawdzanie wierszy
            for (int row = 0; row < 9; row++)
            {
                bool[] exists = new bool[10];
                for (int col = 0; col < 9; col++)
                {
                    int currentNumber = sudokuBeingChecked[row, col];
                    if (currentNumber < 1 || currentNumber > 9)
                        return "\nNIEPRAWIDŁOWE SUDOKU (Liczba poza zakresem 1-9)";
                    if (exists[currentNumber]) return $"\nNIEPRAWIDŁOWE SUDOKU (Błąd w wierszu {row + 1})";
                    exists[currentNumber] = true;
                }
            }

            // Sprawdzanie kolumn
            for (int col = 0; col < 9; col++)
            {
                bool[] exists = new bool[10];
                for (int row = 0; row < 9; row++)
                {
                    int currentNumber = sudokuBeingChecked[row, col];
                    if (exists[currentNumber]) return $"\nNIEPRAWIDŁOWE SUDOKU (Błąd w kolumnie {col + 1})";
                    exists[currentNumber] = true;
                }
            }

            // Sprawdzanie kwadratów 3x3
            for (int x = 0; x < 9; x += 3)
            {
                for (int y = 0; y < 9; y += 3)
                {
                    bool[] istnieje = new bool[10];
                    int squareCounter = 1;
                    for (int i = 0; i < 3; i++)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            int currentNumber = sudokuBeingChecked[x + i, y + j];
                            if (istnieje[currentNumber]) return $"\nNIEPRAWIDŁOWE SUDOKU (Błąd w kwadracie 3x3 nr. {squareCounter})";
                            istnieje[currentNumber] = true;
                        }
                    }
                }
            }

            return "\nPRAWIDŁOWE SUDOKU";
        }

        // Sczytuje i zapisuje do listy 'sudokus' wszystkie plansze sudoku z pliku csv (bez znaków, same cyfry - do obliczeń itp.)
        static List<int[,]> ReadAllSudokus(string path)
        {
            List<int[,]> sudokus = new List<int[,]>();
            if (!File.Exists(path)) return sudokus;

            string[] lines = File.ReadAllLines(path);
            List<string> currentGrid = new List<string>();

            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                currentGrid.Add(line);

                if (currentGrid.Count == 9)
                {
                    int[,] grid = new int[9, 9];
                    for (int row = 0; row < 9; row++)
                    {
                        string[] gridRow = currentGrid[row]
                            .Split(new[] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries);

                        for (int col = 0; col < 9; col++)
                        {
                            if (int.TryParse(gridRow[col], out int cellValue)) grid[row, col] = cellValue;
                        }
                    }
                    sudokus.Add(grid);
                    currentGrid.Clear();
                }
            }

            return sudokus;
        }

        // Zapisywanie wszystkich aktualnych sudoku do csv
        static void SaveAllSudokus(string path, List<int[,]> sudokus)
        {
            using (StreamWriter sw = new StreamWriter(path, append: false))
            {
                foreach (var grid in sudokus)
                {
                    for (int row = 0; row < 9; row++)
                    {
                        List<string> gridRow = new List<string>();
                        for (int col = 0; col < 9; col++) gridRow.Add(grid[row, col].ToString());
                        sw.WriteLine(string.Join(',', gridRow));
                    }
                    sw.WriteLine();
                }
            }
        }

        // Usuwanie wszystkich lub wybranego wpisu (za pomocą gotowej metody bibliotecznej listy .RemoveAt() )
        static void DeleteEntry(string path)
        {
            var sudokus = ReadAllSudokus(path);
            if (sudokus.Count == 0)
            {
                Console.WriteLine("Baza jest pusta.");
                Thread.Sleep(1000);
                return;
            }

            Console.WriteLine($"Obecnie w bazie znajduje się {sudokus.Count} plansz.");
            Console.Write("Podaj numer wpisu do usunięcia: ");
            var input = Console.ReadLine();

            if (!int.TryParse(input, out int index) && index >= 1 && index <= sudokus.Count)
            {
                sudokus.RemoveAt(index - 1);
                SaveAllSudokus(path, sudokus);
                Console.WriteLine("Wpis usunięto pomyślnie");
            }
            else Console.WriteLine("Nieprawidłowy index sudoku.");
            Thread.Sleep(1000);
        }

        // Wyświetla aktualną bazę sudoku (po kolei każdą planszę wraz z informacją czy jest prawidłowa czy nie)
        static void DisplayDatabase(string path)
        {
            var sudokus = ReadAllSudokus(path);
            if (sudokus.Count == 0)
            {
                Console.WriteLine("Baza jest pusta.");
                Thread.Sleep(1000);
                return;
            }

            string choice = "";
            while (true)
            {
                Console.WriteLine($"Znaleziono {sudokus.Count} plansz. Wyświetlić wszystkie (ALL) czy wybraną (ONE)?");
                choice = Console.ReadLine()?.Trim().ToUpper();

                if (choice != "ALL" && choice != "ONE")
                {
                    Console.WriteLine("Nieprawidłowy wybór, Spróbuj ponownie.");
                    Thread.Sleep(1000);
                    continue;
                }
                break;
            }

            if (choice == "ALL")
            {
                for (int i = 0; i < sudokus.Count; i++)
                {
                    Console.WriteLine($"\n--- Sudoku #{i + 1} ---");
                    PrintGrid(sudokus[i]);
                    Console.WriteLine(ValidateSudokuLogic(sudokus[i]));
                    Thread.Sleep(300);
                }
            }
            else if (choice == "ONE")
            {
                Console.Write("\nPodaj numer planszy: ");
                var input = Console.ReadLine();
                if (int.TryParse(input, out int index) && index >= 1 && index <= sudokus.Count)
                {
                    Console.WriteLine($"\n--- Sudoku #{index} ---");
                    PrintGrid(sudokus[index - 1]);
                    Console.WriteLine(ValidateSudokuLogic(sudokus[index - 1]));
                }
            }

            Console.WriteLine("\nNaciśnij dowolny klawisz, aby wrócić.");
            Console.ReadKey(intercept: true);
            Console.Clear();
        }
        
        // Czyści bazę pozytywnie/negatywnie w zależności od zmiennej keepValid.
        static void FilterDatabase(string path, bool keepValid)
        {
            var sudokus = ReadAllSudokus(path);
            if (sudokus.Count == 0)
            {
                Console.WriteLine("Baza jest pusta.");
                Thread.Sleep(1000);
                return;
            }
            Console.WriteLine($"Obecnie w bazie znajduje się {sudokus.Count} plansz.");
            string[] spinnerFrames = { "[□□□□]", "[■□□□]", "[■■□□]", "[■■■□]", "[■■■■]" };

            Console.Write("\nFiltrowanie... ");
            Console.CursorVisible = false;

            for (int i = 0; i < spinnerFrames.Length; i++)
            {
                string currentFrame = spinnerFrames[i];
                Console.Write(currentFrame);

                Thread.Sleep(700);

                // Jeśli to ostatnia klatka, nie cofam kursora, tylko czyszczę znak na końcu
                if (i < spinnerFrames.Length - 1)
                {
                    Console.Write(new string('\b', currentFrame.Length));
                }
                else
                {
                    Console.WriteLine(); // Nadpisanie spacją i przejście do nowej linii
                }
            }
            Console.CursorVisible = true;

            var filtered = sudokus.Where(grid =>
            {
                string result = ValidateSudokuLogic(grid);
                bool isValid = result.Contains("PRAWIDŁOWE SUDOKU") && !result.Contains("NIEPRAWIDŁOWE SUDOKU");
                return keepValid ? isValid : !isValid;
            }).ToList();

            SaveAllSudokus(path, filtered);
            Console.WriteLine($"\nPrzefiltrowano. Pozostało {sudokus.Count} plansz.");
            Thread.Sleep(3000);
        }

        // Wypisuje statystyki bazy
        static void ShowStats(string path)
        {
            var sudokus = ReadAllSudokus(path);
            if (sudokus.Count == 0)
            {
                Console.WriteLine("Baza jest pusta.");
                Thread.Sleep(1000);
                return;
            }

            int validCount = 0;
            int invalidCount = 0;

            foreach (var grid in sudokus)
            {
                string result = ValidateSudokuLogic(grid);
                if (result.Contains("PRAWIDŁOWE SUDOKU") && !result.Contains("NIEPRAWIDŁOWE SUDOKU")) validCount++;
                else invalidCount++;
            }

            int validPercent = (int)Math.Round((double)(validCount * 100) / sudokus.Count);
            int invalidPercent = 100-validPercent;

            Console.WriteLine();
            Console.WriteLine("--- STATYSTYKI BAZY ---");
            Console.WriteLine($"Całkowita liczba plansz : {sudokus.Count}");
            Console.WriteLine($"Poprawne Sudoku         : {validCount} ({validPercent}%)");
            Console.WriteLine($"Błędne Sudoku           : {invalidCount} ({invalidPercent}%)");
            Console.WriteLine("\nNaciśnij dowolny klawisz, aby wrócić.");
            Console.ReadKey();
        }

        static void ManualEntry(string path)
        {
            Console.WriteLine("Wpisz gotową planszę sudoku (9 wierszy po 9 cyfr) lub wklej gotowca");
            List<string> rows = new List<string>();
            try
            {
                for (int i = 0; i < 9; i++)
                {
                    string input = Console.ReadLine();

                    if (string.IsNullOrWhiteSpace(input))
                    {
                        Console.WriteLine($"Błąd: Pusty wiersz (nr {i + 1}). Anulowano wprowadzanie.");
                        Thread.Sleep(2000);
                        return;
                    }

                    input = input.Trim();

                    if (input.Length != 9)
                    {
                        Console.WriteLine($"Błąd: Wiersz {i + 1} zawiera {input.Length} znaków zamiast 9. Anulowano wprowadzanie.");
                        Thread.Sleep(2000);
                        return;
                    }

                    foreach (char c in input)
                    {
                        if (!char.IsDigit(c))
                        {
                            Console.WriteLine($"Błąd: Wiersz {i + 1} zawiera niedozwolony znak '{c}'. Dozwolone tylko cyfry 0-9. Anulowano wprowadzanie.");
                            Thread.Sleep(2000);
                            return;
                        }
                    }

                    rows.Add(input);
                }

                File.AppendAllLines(path, rows);
                File.AppendAllText(path, "\n");
                Console.WriteLine("\nZapisano pomyślnie.");
            }
            catch (Exception)
            {
                Console.WriteLine("Błąd podczas wprowadzania danych.");
            }
            Thread.Sleep(1000);
        }

        static void AnimateSudoku(string path, bool isSolved)
        {

        }
    }
}
