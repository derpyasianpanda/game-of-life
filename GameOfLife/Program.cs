using System;
using System.Threading;
using System.Threading.Tasks;

namespace GameOfLife
{
    class Program
    {
        public static void Main(string[] args)
        {
            const int SleepIncrement = 50;

            bool needsRedraw = true;
            bool isRunning = true;
            bool enableMoreInfo = false;
            int sleepAmount = 500;
            DateTime timeOfLastEvolution = DateTime.Now;
            GameGrid grid = new GameGrid();

            Console.SetWindowSize(150, 50);
            Console.SetWindowPosition(0, 0);

            Console.CancelKeyPress += (sender,  args) =>
            {
                isRunning = false;
                Console.WriteLine("\nQuitting Application");
                Thread.Sleep(2000);
                Environment.Exit(0);
            };

            while (isRunning)
            {
                while (Console.KeyAvailable == false)
                {
                    if (needsRedraw)
                    {
                        PrintInfo(sleepAmount, enableMoreInfo);
                        PrintGrid(grid);
                        needsRedraw = false;
                    }
                    if ((DateTime.Now - timeOfLastEvolution).TotalMilliseconds 
                        >= sleepAmount)
                    {
                        grid.NextGeneration();
                        timeOfLastEvolution = DateTime.Now;
                        needsRedraw = true;
                    }
                }
                ConsoleKey key = Console.ReadKey(true).Key;
                needsRedraw = true;
                switch (key)
                {
                    case ConsoleKey.F1:
                        enableMoreInfo = !enableMoreInfo;
                        break;
                    case ConsoleKey.F5:
                        grid = new GameGrid();
                        timeOfLastEvolution = DateTime.Now;
                        break;
                    case ConsoleKey.UpArrow:
                        sleepAmount += SleepIncrement;
                        break;
                    case ConsoleKey.DownArrow:
                        sleepAmount -= sleepAmount < SleepIncrement ?
                            sleepAmount : SleepIncrement;
                        break;
                    default:
                        needsRedraw = false;
                        break;
                }
            }
        }

        public static void PrintInfo(int sleepAmount, bool enableMoreInfo)
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.CursorVisible = false;
            Console.SetCursorPosition(0, 0);
            Console.WriteLine(
                "Welcome to the KV's Game of Life " +
                "(Originally by John Conway)\n" +
                "Press F1 to toggle more information"
                );
            if (enableMoreInfo)
            {
                // Padding is to ensure no remnants from the board
                Console.WriteLine(
                    "Press F5 to refresh the board"
                    .PadRight(Console.WindowWidth, ' '));
                Console.WriteLine(
                    ($"Evolving every {sleepAmount}ms " +
                    $"(Up and Down arrow keys to adjust time)")
                    .PadRight(Console.WindowWidth, ' '));
                Console.WriteLine(
                    "Press Ctrl + C to exit the app"
                    .PadRight(Console.WindowWidth, ' '));
            }
            Console.WriteLine(new string(' ', Console.WindowWidth));
        }

        public static void PrintGrid(GameGrid board)
        {
            Console.Write(board);
            // Loop ensures no remnants from past boards
            for (int i = Console.CursorTop; i < Console.WindowHeight - 1; i++)
            {
                Console.Write("\r" + new string(' ', Console.WindowWidth) + "\n");
            }
        }
    }

    class Tile
    {
        public Status State { get; set; }
        public Tile(Status state)
        {
            State = state;
        }
    }

    class GameGrid
    {        
        public Tile[,] Grid { get; set; }
        public int Rows { get; set; }
        public int Columns { get; set; }

        public GameGrid(int rows = 125, int columns = 35,
            double initialPercentage = 0.1)
        {
            Rows = rows;
            Columns = columns;
            Grid = new Tile[Rows, Columns];
            Random numberGenerator = new Random();
            for (int x = 0; x < Rows; x++)
            {
                for (int y = 0; y < Columns; y++)
                {
                    Grid[x, y] = 
                        new Tile(
                            (Status) 
                            (numberGenerator.NextDouble() < initialPercentage 
                                ? 1 : 0)
                        );
                }
            }
        }

        public override string ToString()
        {
            string result = "";
            for (int y = 0; y < Columns; y++)
            {
                for (int x = 0; x < Rows; x++)
                {
                    result += Grid[x, y].State == Status.Dead ? " " : "O";
                }
                result += "\n";
            }
            return result;
        }

        public void NextGeneration()
        {
            for (int x = 0; x < Rows; x++)
            {
                for (int y = 0; y < Columns; y++)
                {
                    int neighbors = GetNeighbors(x, y);
                    if (neighbors < 2 || neighbors > 3) Grid[x, y].State = Status.Dead;
                    else Grid[x, y].State =
                            neighbors == 3 ? Status.Alive : Grid[x, y].State;
                }
            }
        }

        private int GetNeighbors(int xStart, int yStart)
        {
            int neighbors = 0;
            for (int x = xStart - 1; x <= xStart + 1; x++)
            {
                for (int y = yStart - 1; y <= yStart + 1; y++)
                {
                    if (x < Rows && y < Columns && x > 0 && y > 0 && 
                        (x != xStart || y != yStart))
                    {
                        neighbors += (int) Grid[x, y].State;
                    }
                }
            }
            return neighbors;
        }
    }

    public enum Status { Dead, Alive };
}
