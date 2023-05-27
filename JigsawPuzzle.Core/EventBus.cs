using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JigsawPuzzle.Core
{
    public class EventBus
    {
        #region 游戏事件
        /// <summary>
        /// 地图初始化事件
        /// </summary>
        public static EventHandler<int[,]> InitMapAfter;
        /// <summary>
        /// 移动事件
        /// </summary>
        //public EventHandler<> MoveEvent = null;
        public static EventHandler<Tuple<int, int[,]>> MoveEvent;
        /// <summary>
        /// 游戏结束事件
        /// </summary>
        public static EventHandler GameOverEvent;
        #endregion
        #region 联机事件
        /// <summary>
        /// 加入游戏房间
        /// </summary>
        public static EventHandler JoinGameEvent;
        /// <summary>
        /// 游戏准备事件
        /// </summary>
        public static EventHandler ReadyEvent;
        #endregion
    }
}
