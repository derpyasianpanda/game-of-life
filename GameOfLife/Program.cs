using System;
using System.Threading;
using System.Threading.Tasks;

namespace GameOfLife
{
    class Program
    {
        public static void Main(string[] args)
        {
            bool isRunning = true;
            bool enableMoreInfo = false;
            int sleepAmount = 500;
            Board grid = new Board();

            Console.CancelKeyPress += (sender,  args) =>
            {
                isRunning = false;
                Console.WriteLine("\nQuitting Application");
                Thread.Sleep(1000);
                Environment.Exit(0);
            };

            while (isRunning)
            {
                while (Console.KeyAvailable == false)
                {
                    PrintOver(grid.ToString(), sleepAmount, enableMoreInfo);
                    grid.NextGeneration();
                }
                ConsoleKey key = Console.ReadKey(true).Key;
                switch (key) 
                {
                    case ConsoleKey.F1:
                        enableMoreInfo = !enableMoreInfo;
                        break;
                    case ConsoleKey.F5:
                        grid = new Board();
                        break;
                    case ConsoleKey.UpArrow:
                        sleepAmount += 100;
                        break;
                    case ConsoleKey.DownArrow:
                        sleepAmount -= sleepAmount < 100 ? sleepAmount : 100;
                        break;
                    default:
                        break;
                }
            }
        }
        public static void PrintOver(string content, int sleepAmount,
            bool enableMoreInfo)
        {
            string moreInfo = "Press F5 to refresh the board\n" +
                $"Evolving every {sleepAmount}ms\n";
            Console.BackgroundColor = ConsoleColor.Black;
            Console.CursorVisible = false;
            Console.SetCursorPosition(0, 0);
            Console.WriteLine(
                "Welcome to the KV's Game of Life " +
                "(Originally by John Conway)\n" +
                "Press F1 to toggle more information"
                );
            if (enableMoreInfo) Console.WriteLine(moreInfo);
            Console.WriteLine(content);
            if (sleepAmount > 0) Thread.Sleep(sleepAmount);
        }
    }

    public enum Status { Dead , Alive };

    class Tile
    {
        public Status State { get; set; }
        public Tile(Status state)
        {
            State = state;
        }
    }

    class Board
    {        
        public Tile[,] Grid { get; set; }
        public int Rows { get; set; }
        public int Columns { get; set; }

        public Board(int rows = 75, int columns = 20,
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
}
