using System;
using System.IO;
using System.Collections.Generic;

namespace sudoku_console_app
{
    internal class Program
    {
        static string solvedSudokuFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\solvedSudokus.csv");
        static string unsolvedSudokuFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\unsolvedSudokus.csv");
        private static Random random = new Random();
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("--- APLIKACJA SUDOKU ---");

            while (true)
            {
                // Wprowadzanie danych
                Console.WriteLine("\nPodaj liczbę sudoku do wygenerowania [0-20] (Wpisz 'exit' aby zakończyć działanie programu)\n");
                var input = Console.ReadLine()?.Trim();

                // Walidacja danych
                if (input == "exit")
                {
                    Console.WriteLine("\nKoniec programu.\n");
                    break;
                }

                if (!int.TryParse(input, out int count))
                {
                    Console.WriteLine("\nNieprawidłowy format danych. Spróbuj ponownie.\n");
                    continue;
                }

                if (count < 0 || count > 20)
                {
                    Console.WriteLine("\nIlość sudoku wychodzi poza dozwolony przedział. Spróbuj ponownie\n");
                    continue;
                }

                // Generowanie sudoku
                GenerateAndSaveSudokus(count);
                Console.Write($"\nZ powodzeniem wygenerowano i zapisano [{count}] sudoku do bazy.");
                Thread.Sleep(3000);
                Console.Clear();
                Console.WriteLine("--- APLIKACJA SUDOKU ---");

                // Zarządzanie bazą
                Console.WriteLine("\nCo chcesz zrobić przed ponowną generacją?\n");
                Console.WriteLine("0 - Zakończ program.");
                Console.WriteLine("1 - Wyzeruj bazę");
                Console.WriteLine("2 - Zostaw bazę");
                Console.WriteLine("3 - Wypisz aktualną sprawdzoną bazę");
                Console.WriteLine();

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "0": Console.WriteLine("Zamykanie programu..."); return;
                    case "1":
                        File.WriteAllText(solvedSudokuFilePath, string.Empty);
                        Console.Write("\n[Info] Baza została pomyślnie wyczyszczona.");
                        Thread.Sleep(3000);
                        Console.Clear();
                        Console.WriteLine("--- APLIKACJA SUDOKU ---");
                        break;
                    case "2":
                        Console.Write("\n[Info] Baza została zachowana. Kolejne plansze zostaną dopisane.");
                        Thread.Sleep(3000);
                        Console.Clear();
                        Console.WriteLine("--- APLIKACJA SUDOKU ---");
                        break;
                    case "3":
                        VerifySudokusFromCsv(solvedSudokuFilePath);
                        Console.WriteLine("\nNaciśnij dowolny klawisz, aby wrócić: ");
                        Console.ReadKey();
                        Console.Clear();
                        Console.WriteLine("--- APLIKACJA SUDOKU ---");
                        break;
                }
            }
        }

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
    }
}
