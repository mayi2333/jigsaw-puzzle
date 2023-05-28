using JigsawPuzzle.Core;
using JigsawPuzzle.WCF;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace JigsawPuzzle.Wpf
{
    public class GameCallback : IGameCallback
    {
        public void StartGame(byte[] map)
        {
            using (MemoryStream ms = new MemoryStream(map))
            {
                System.Runtime.Serialization.IFormatter f = new BinaryFormatter();
                int[,] mapArray = f.Deserialize(ms) as int[,];
                GameContext.RemoteGame = new Core.Game("remote", mapArray);
                GameContext.NativeGame = new Game("native", mapArray);
            }
            GameContext.IsStartGame = true;
        }
        public void Move(OperationType type)
        {
            GameContext.RemoteGame.Move(type);
        }
    }
}
