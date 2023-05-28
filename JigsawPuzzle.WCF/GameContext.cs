using JigsawPuzzle.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JigsawPuzzle.WCF
{
    public static class GameContext
    {
        /// <summary>
        /// 本地游戏实例
        /// </summary>
        public static Game NativeGame;
        /// <summary>
        /// 远程联机游戏实例
        /// </summary>
        public static Game RemoteGame;
        /// <summary>
        /// 宫格图片列表
        /// </summary>
        public static List<System.Drawing.Image> GridImageList;
        /// <summary>
        /// 游戏图片
        /// </summary>
        public static Image GameImg;
        /// <summary>
        /// 准备状态
        /// </summary>
        public static bool IsReady;
        /// <summary>
        /// 游戏是否开始
        /// </summary>
        public static bool IsStartGame;
        /// <summary>
        /// WCF回调
        /// </summary>
        public static IGameCallback CallbackClient;
    }
}
