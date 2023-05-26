using System;
using JigsawPuzzle.Core;

namespace JigsawPuzzle.Console
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var game = new Game(3);
            game.InitMap();
            game.ConsoleWrite();
            System.Console.WriteLine("游戏开始");
            while (true)
            {
                var key = System.Console.ReadKey();
                if (key.Key == ConsoleKey.W)
                {
                    game.Move(OperationType.Up);
                }
                else if (key.Key == ConsoleKey.S)
                {
                    game.Move(OperationType.Down);
                }
                else if (key.Key == ConsoleKey.A)
                {
                    game.Move(OperationType.Left);
                }
                else if (key.Key == ConsoleKey.D)
                {
                    game.Move(OperationType.Right);
                }
                game.ConsoleWrite();
            }
        }
    }
}
