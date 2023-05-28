using JigsawPuzzle.Core;
using JigsawPuzzle.WCF;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace JigsawPuzzle.Wpf
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// 游戏服务主机
        /// </summary>
        ServiceHost host;
        /// <summary>
        /// WCF服务
        /// </summary>
        WCFGameService.GameServiceClient serviceProxy;


        public MainWindow()
        {
            InitializeComponent();
            //绑定游戏地图初始化事件
            EventBus.InitMapAfter += GameScreenRedraw;
            //绑定游戏移动事件
            EventBus.MoveEvent += GameScreenRedraw;
            //绑定游戏结束事件
            EventBus.GameOverEvent += GameOver;
            //绑定加入游戏事件
            EventBus.JoinGameEvent += ClientJoinGameEvent;
            GameContext.IsStartGame = false;
        }

        /// <summary>
        /// 开启游戏服务
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartService_Click(object sender, RoutedEventArgs e)
        {
            if (host == null)
            {
                if (GameContext.GridImageList == null || GameContext.GridImageList.Count == 0)
                {
                    MessageBox.Show("请选择游戏图片");
                    return;
                }
                host = new ServiceHost(typeof(GameService));

                //绑定
                System.ServiceModel.Channels.Binding tcpBinding = new NetTcpBinding();
                //终结点
                host.AddServiceEndpoint(typeof(IGameService), tcpBinding, "net.tcp://127.0.0.1:9999/GameService");
                host.Open();
                MessageBox.Show("房间创建成功,等待加入游戏中...");
                btnJoinGame.IsEnabled = false;
                btnSelectImg.IsEnabled = false;
                btnReady.IsEnabled = false;
                btnStartService.IsEnabled = false;
            }
            else
            {
                MessageBox.Show("请勿重复创建房间");
            }
        }
        /// <summary>
        /// 加入房间
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void JoinGame_Click(object sender, RoutedEventArgs e)
        {
            if (serviceProxy == null || serviceProxy.State != CommunicationState.Opened)
            {
                var callbackInstance = new InstanceContext(new GameCallback());
                serviceProxy = new WCFGameService.GameServiceClient(callbackInstance);
                if (serviceProxy.State == CommunicationState.Created)
                {
                    //byte[] img = serviceProxy.JoinGame();
                    //using (MemoryStream ms = new MemoryStream(img))
                    //{
                    //    System.Runtime.Serialization.IFormatter f = new BinaryFormatter();
                    //    GameContext.GameImg = f.Deserialize(ms) as System.Drawing.Image;
                    //}
                    GameContext.GridImageList = GameContext.GameImg.ToGridImages();
                    previewImg.Source = GameContext.GameImg.ToBitmapImage();
                    btnJoinGame.IsEnabled = false;
                    btnSelectImg.IsEnabled = false;
                    btnStartGame.IsEnabled = false;
                    btnStartService.IsEnabled = false;
                    MessageBox.Show("加入游戏成功，请准备游戏");
                }
                else
                {
                    MessageBox.Show("加入游戏失败");
                }
            }
            else
            {
                MessageBox.Show("已加入房间");
            }
        }

        /// <summary>
        /// 开始游戏
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartGame_Click(object sender, RoutedEventArgs e)
        {
            if (!GameContext.IsReady)
            {
                MessageBox.Show("请等待联机玩家准备游戏");
                return;
            }
            if (GameContext.NativeGame != null && !GameContext.NativeGame.IsGameOver)
            {
                MessageBox.Show("游戏已经开始,请用键盘上下左右键操控");
                return;
            }
            GameContext.NativeGame = new Game("native");
            var map = GameContext.NativeGame.InitMap();
            GameContext.RemoteGame = new Game("remote", map);
            using (MemoryStream ms = new MemoryStream())
            {
                System.Runtime.Serialization.IFormatter f = new BinaryFormatter();
                f.Serialize(ms, map);
                GameContext.CallbackClient.StartGame(ms.ToArray());
            }
            GameContext.IsStartGame = true;
        }

        /// <summary>
        /// 准备
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Ready_Click(object sender, RoutedEventArgs e)
        {
            if (serviceProxy == null)
            {
                MessageBox.Show("请先加入游戏");
            }
            if (GameContext.NativeGame != null && !GameContext.NativeGame.IsGameOver)
            {
                MessageBox.Show("游戏已经开始,请用键盘上下左右键操控");
                return;
            }
            if (serviceProxy.State == CommunicationState.Opened)
            {
                serviceProxy.ReadyGame();
                MessageBox.Show("准备成功，等待开始游戏");
            }
        }

        /// <summary>
        /// 选择图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectImg_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = true;//该值确定是否可以选择多个文件
            dialog.Title = "请选择游戏图片";
            dialog.Filter = "图片文件(*.*)|*.*";
            if (dialog.ShowDialog().Value == true)
            {
                var img = ImageUtil.LoadImgByFilePath(dialog.FileName);
                if (img == null)
                {
                    MessageBox.Show("请选择宽高大于500的图片");
                    return;
                }
                GameContext.GridImageList = img.ToGridImages();
                previewImg.Source = img.ToBitmapImage();
                GameContext.GameImg = img;
            }
        }
        private void MainWindows_Keydown(object sender, KeyEventArgs e)
        {
            if (!GameContext.IsStartGame)
            {
                return;
            }
            OperationType? operationType;
            switch (e.Key)
            {
                case Key.Up:
                    operationType = OperationType.Up;
                    break;
                case Key.Down:
                    operationType = OperationType.Down;
                    break;
                case Key.Left:
                    operationType = OperationType.Left;
                    break;
                case Key.Right:
                    operationType = OperationType.Right;
                    break;
                default:
                    operationType = null;
                    break;
            }
            if (operationType != null)
            {
                //发送操作信息到远程端
                if (serviceProxy != null)
                {
                    serviceProxy.OnlineUserMove(operationType.Value);
                }
                else
                {
                    GameContext.CallbackClient.Move(operationType.Value);
                }
                //操作本地端
                GameContext.NativeGame.Move(operationType.Value);
            }
        }

        /// <summary>
        /// 游戏地图初始化事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="map"></param>
        private void GameScreenRedraw(object sender, int[,] map)
        {
            if ((string)sender == "native")
            {
                nativeScreenImg.Source = GameContext.GridImageList.GridToBitmapImage(map);
            }
            else
            {
                remoteScreenImg.Source = GameContext.GridImageList.GridToBitmapImage(map);
            }
        }
        /// <summary>
        /// 游戏地图初始化事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="map"></param>
        private void GameScreenRedraw(object sender, Tuple<int, int[,]> e)
        {
            if ((string)sender == "native")
            {
                nativeScreenImg.Source = GameContext.GridImageList.GridToBitmapImage(e.Item2);
            }
            else
            {
                remoteScreenImg.Source = GameContext.GridImageList.GridToBitmapImage(e.Item2);
            }
        }
        /// <summary>
        /// 游戏结束事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GameOver(object sender, EventArgs e)
        {
            GameContext.IsStartGame = false;
            if ((string)sender == "native")
            {
                MessageBox.Show("你赢了");
                GameContext.RemoteGame.GameOver();
            }
            else
            {
                MessageBox.Show("游戏结束");
                GameContext.NativeGame.GameOver();
            }
        }
        /// <summary>
        /// 客户端加入游戏事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClientJoinGameEvent(object sender, EventArgs e)
        {
            MessageBox.Show("玩家已加入房间，请等待玩家准备");
        }
    }
}
