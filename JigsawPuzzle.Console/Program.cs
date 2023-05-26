// See https://aka.ms/new-console-template for more information
using JigsawPuzzle.Core;
var game = new Game(3);
game.InitMap();
game.ConsoleWrite();
Console.WriteLine("游戏开始");
while (true)
{ 
    var key = Console.ReadKey();
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

