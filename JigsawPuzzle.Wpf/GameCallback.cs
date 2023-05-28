using JigsawPuzzle.Core;
using JigsawPuzzle.Wpf.WCFGameService;
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
    public class GameCallback : IGameServiceCallback
    {
        public void StartGame(byte[] map)
        {
            using (MemoryStream ms = new MemoryStream(map))
            {
                System.Runtime.Serialization.IFormatter f = new BinaryFormatter();
                int[,] mapArray = f.Deserialize(ms) as int[,];
                WCF.GameContext.RemoteGame = new Core.Game("remote", mapArray);
                WCF.GameContext.NativeGame = new Game("native", mapArray);
            }
            WCF.GameContext.IsStartGame = true;
        }
        public void Move(OperationType type)
        {
            WCF.GameContext.RemoteGame.Move(type);
        }
    }
}
