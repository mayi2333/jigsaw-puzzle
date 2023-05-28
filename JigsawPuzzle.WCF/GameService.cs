using JigsawPuzzle.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.ServiceModel;
using System.Text;

namespace JigsawPuzzle.WCF
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的类名“Service1”。
    public class GameService : IGameService
    {
        public GameService()
        {
            GameContext.CallbackClient = OperationContext.Current.GetCallbackChannel<IGameCallback>();
        }
        /// <summary>
        /// 客户端加入服务端服务端返回游戏图片到客户端
        /// </summary>
        /// <returns></returns>
        public byte[] JoinGame()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                System.Runtime.Serialization.IFormatter f = new BinaryFormatter();
                f.Serialize(ms, GameContext.GameImg);
                return ms.ToArray();
            }
        }
        /// <summary>
        /// 客户端准备游戏服务端标记客户端准备状态和广播准备事件
        /// </summary>
        public void ReadyGame()
        {
            GameContext.IsReady = true;
            EventBus.ReadyEvent?.Invoke(null, null);
        }
        /// <summary>
        /// 客户端玩家操作
        /// </summary>
        /// <param name="type"></param>
        public void OnlineUserMove(OperationType type)
        {
            GameContext.RemoteGame.Move(type);
        }
    }

}
