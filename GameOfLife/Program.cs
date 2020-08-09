using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace GameOfLife
{
    class Program
    {
        const int SleepIncrement = 50;

        static bool needsRedraw = true;
        static bool isRunning = true;
        static bool enableMoreInfo = false;
        static bool isPaused = false;
        static int sleepAmount = 250;

        static GameGrid grid = new GameGrid();
        static DateTime timeOfLastEvolution;

        public static void Main(string[] args)
        {
            timeOfLastEvolution = DateTime.Now;

            Console.SetWindowSize(150, 50);
            Console.SetWindowPosition(0, 0);
            Console.CursorVisible = false;

            Console.CancelKeyPress += (sender, args) =>
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
                        Console.SetCursorPosition(0, 0);
                        Console.CursorVisible = false;
                        PrintInfo();
                        PrintGrid();
                        needsRedraw = false;
                    }
                    if ((DateTime.Now - timeOfLastEvolution).TotalMilliseconds
                        >= sleepAmount)
                    {
                        if (!isPaused)
                        {
                            grid.NextGeneration();
                            timeOfLastEvolution = DateTime.Now;
                            needsRedraw = true;
                        }
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
                    case ConsoleKey.P:
                        isPaused = !isPaused;
                        break;
                    default:
                        needsRedraw = false;
                        break;
                }
            }
        }

        public static void PrintInfo()
        {
            Console.Write((
                "Welcome to the KV's Game of Life " +
                "(Originally by John Conway)" + 
                (isPaused ? " GAME PAUSED" : ""))
                .PadRight(Console.WindowWidth));
            Console.Write((
                "Press F1 to toggle more information or P to pause")
                .PadRight(Console.WindowWidth));
            if (enableMoreInfo)
            {
                // Padding is to ensure no remnants from the board
                Console.Write(
                    "Press F5 to refresh the board"
                    .PadRight(Console.WindowWidth));
                Console.Write(
                    ($"Evolving every {sleepAmount}ms " +
                    $"(Up and Down arrow keys to adjust time)")
                    .PadRight(Console.WindowWidth));
                Console.Write(
                    "Press Ctrl + C to exit the app"
                    .PadRight(Console.WindowWidth));
            }
            Console.Write(new string(' ', Console.WindowWidth));
        }

        public static void PrintGrid()
        {
            Console.Write(grid);
            // Loop ensures no remnants from past boards
            for (int i = Console.CursorTop; i < Console.WindowHeight - 1; i++)
            {
                Console.Write(new string(' ', Console.WindowWidth));
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
            double initialPercentage = 0.07)
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
                string temp = "";
                for (int x = 0; x < Rows; x++)
                {
                    temp += Grid[x, y].State == Status.Dead ? " " : "O";
                }
                result += temp.PadRight(Console.WindowWidth);
            }
            return result;
        }

        public void NextGeneration()
        {
            Tile[,] newGrid = new Tile[Rows, Columns];

            for (int x = 0; x < Rows; x++)
            {
                for (int y = 0; y < Columns; y++)
                {
                    int neighbors = GetNeighbors(x, y);
                    if (neighbors < 2 || neighbors > 3) newGrid[x, y] = new Tile(Status.Dead);
                    else if (neighbors == 3) newGrid[x, y] = new Tile(Status.Alive);
                    else newGrid[x, y] = new Tile(Grid[x, y].State);
                }
            }

            Grid = newGrid;
        }

        private int GetNeighbors(int xStart, int yStart)
        {
            int neighbors = 0;
            for (int x = xStart - 1; x <= xStart + 1; x++)
            {
                for (int y = yStart - 1; y <= yStart + 1; y++)
                {
                    if (x != xStart || y != yStart)
                    {
                        neighbors += (int)Grid[
                            x == Rows ? 0 : (x == -1 ? Rows - 1 : x),
                            y == Columns ? 0 : (y == -1 ? Columns - 1 : y)
                            ]
                            .State;
                    }
                }
            }
            return neighbors;
        }
    }

    public enum Status { Dead, Alive };
}
