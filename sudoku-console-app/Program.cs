using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;

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
                        CleanUpDatabase(solvedSudokuFilePath, true);
                        break;
                    case "04":
                        CleanUpDatabase(solvedSudokuFilePath, false);
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
                    case "1":
                        File.WriteAllText(unsolvedSudokuFilePath, string.Empty);
                        Console.WriteLine("Baza wyzerowana.");
                        Thread.Sleep(1000);
                        break;
                    case "2":
                        DeleteEntry(unsolvedSudokuFilePath);
                        break;
                    case "3":
                        GenerateUnsolvedSudoku();
                        break;
                    case "4":
                        DisplayDatabase(unsolvedSudokuFilePath);
                        break;
                    case "5":
                        AnimateSudoku(unsolvedSudokuFilePath, isSolved: false);
                        break;
                }
            }
        }

        
        // ----- Metody do rysowania menu -----

        // Unicode obramowania
        // https://en.wikipedia.org/wiki/Box-drawing_characters

        // Rysowanie górnego paska obramowania
        static void PrintTopBorder()
        {
            Console.WriteLine("╔" + new string('═', MenuWidth - 2) + "╗");
        }

        // Rysowanie środkowego paska obramowania
        static void PrintMiddleBorder()
        {
            Console.WriteLine("╠" + new string('═', MenuWidth - 2) + "╣");
        }

        // Rysowanie dolnego paska obramowania
        static void PrintBottomBorder()
        {
            Console.WriteLine("╚" + new string('═', MenuWidth - 2) + "╝");
        }

        // Rysowanie pustej linii bez tekstu (tylko obramowanie ║ po lewej i prawej ) 
        static void PrintEmptyLine()
        {
            Console.WriteLine("║" + new string(' ', MenuWidth - 2) + "║");
        }

        // Rysuje obramowania ║ po lewej i prawej oraz wypisuje wyśrodkowany podany tekst dla nagłówków w menu
        static void PrintCenteredText(string text)
        {
            // Oblicza ilość znaków potrzebnych do wyśrodkowania tekstu
            int totalSpaces = MenuWidth - 2 - text.Length;
            int leftSpaces = totalSpaces / 2;
            int rightSpaces = totalSpaces - leftSpaces;

            // Zabezpieczenie przed błędem gdzie tekst jest dłuższy od szerokości menu
            if (totalSpaces < 0)
            {
                text = text.Substring(0, MenuWidth - 5) + "...";
                leftSpaces = 0; rightSpaces = 0;
            }

            Console.WriteLine("║" + new string(' ', leftSpaces) + text + new string(' ', rightSpaces) + "║");
        }

        // Rysuje obramowania ║ po lewej i prawej oraz wypisuje podany tekst wyrównany do lewej dla pozycji w menu
        static void PrintLeftText(string text)
        {
            string prefix = "  ";
            int rightSpaces = MenuWidth - 2 - text.Length - prefix.Length;

            // Zabezpieczenie przed błędem gdzie tekst jest dłuższy od szerokości menu
            if (rightSpaces < 0)
            {
                text = text.Substring(0, MenuWidth - 5 - prefix.Length) + "...";
                rightSpaces = 0;
            }

            Console.WriteLine("║" + prefix + text + new string(' ', rightSpaces) + "║");
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

        // Pobieranie danych z bazy
        static void VerifySudokusFromCsv(string path)
        {
            if (!File.Exists(path))
            {
                Console.WriteLine($"Nie znaleziono pliku: {path}");
                return;
            }

            try
            {
                string[] lines = File.ReadAllLines(path);
                List<string> currentSudokuLines = new List<string>();
                int sudokuCounter = 1;

                foreach (string line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    currentSudokuLines.Add(line);

                    if (currentSudokuLines.Count == 9)
                    {
                        ProcessAndPrintSudoku(currentSudokuLines, sudokuCounter);
                        sudokuCounter++;
                        currentSudokuLines.Clear();
                        Thread.Sleep(500);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Wystąpił błąd odczytu pliku {ex.Message}");
            }
        }

        static void ProcessAndPrintSudoku(List<string> rowData, int index)
        {
            Console.WriteLine($"\n --- Sudoku #{index} --- \n");
            int[,] sudoku = new int[9, 9];

            try
            {
                for (int row = 0; row < 9; row++)
                {
                    string[] currentRow = rowData[row].Split(new[] {',',';',' '}, StringSplitOptions.RemoveEmptyEntries);
                    for (int col = 0; col < 9; col++)
                    {
                        sudoku[row, col] = int.Parse(currentRow[col].Trim());
                    }
                }

                PrintGrid(sudoku);
                string result = ValidateSudokuLogic(sudoku);
                Console.WriteLine(result);
            }
            catch(Exception)
            {
                Console.WriteLine("Błąd odczytu planszy.");
            }
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

        // Usuwanie wszystkich lub wybranego wpisu
        static void DeleteEntry(string path)
        {

        }

        static void CleanUpDatabase(string path, bool keepValid)
        {

        }

        static void DisplayDatabase(string path)
        {

        }

        static void ShowStats(string path)
        {

        }

        static void ManualEntry(string path)
        {

        }

        static void AnimateSudoku(string path, bool isSolved)
        {

        }
    }
}
