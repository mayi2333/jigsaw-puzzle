using JigsawPuzzle.Core;
using JigsawPuzzle.WCF;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
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
        /// 本地游戏实例
        /// </summary>
        Game nativeGame;
        /// <summary>
        /// 远程联机游戏实例
        /// </summary>
        Game remoteGame;

        List<System.Drawing.Image> gridImageList;

        public MainWindow()
        {
            InitializeComponent();
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
                host = new ServiceHost(typeof(GameService));

                //绑定
                System.ServiceModel.Channels.Binding tcpBinding = new NetTcpBinding();
                //终结点
                host.AddServiceEndpoint(typeof(IGameService), tcpBinding, "net.tcp://127.0.0.1:9999/GameService");
                host.Open();
                MessageBox.Show("房间创建成功");
                btnJoinGame.IsEnabled = false;
                btnSelectImg.IsEnabled = false;
                btnReady.IsEnabled = false;
                //if (host.Description.Behaviors.Find<System.ServiceModel.Description.ServiceMetadataBehavior>() == null)
                //{
                //    //行为
                //    ServiceMetadataBehavior behavior = new ServiceMetadataBehavior();
                //    behavior.HttpGetEnabled = true;

                //    //元数据地址
                //    behavior.HttpGetUrl = new Uri("http://localhost:8002/Service1");
                //    Host.Description.Behaviors.Add(behavior);

                //    //启动
                //    Host.Open();
                //}
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
            new WCFGameService.GameServiceClient().GetData(1);
        }

        /// <summary>
        /// 开始游戏
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartGame_Click(object sender, RoutedEventArgs e)
        {
            if (gridImageList == null && gridImageList.Count == 0)
            {
                MessageBox.Show("请选择游戏图片");
                return;
            }
            if (nativeGame != null && !nativeGame.IsGameOver)
            {
                MessageBox.Show("游戏已经开始,请用键盘上下左右键操控");
                return;
            }
            nativeGame = new Game();
            //绑定本机游戏地图初始化事件
            nativeGame.InitMapAfter += NativeGameScreenRedraw;
            //绑定本机游戏移动事件
            nativeGame.MoveEvent += NativeGameScreenRedraw;
            //绑定本机游戏结束事件
            nativeGame.GameOverEvent += NativeGameOver;
            nativeGame.InitMap();
        }

        /// <summary>
        /// 准备
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Ready_Click(object sender, RoutedEventArgs e)
        {

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
                gridImageList = img.ToGridImages();
                previewImg.Source = img.ToBitmapImage();
            }
        }
        private void MainWindows_Keydown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up:
                    nativeGame?.Move(OperationType.Up);
                    break;
                case Key.Down:
                    nativeGame?.Move(OperationType.Down);
                    break;
                case Key.Left:
                    nativeGame?.Move(OperationType.Left);
                    break;
                case Key.Right:
                    nativeGame?.Move(OperationType.Right);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 本机游戏地图初始化事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="map"></param>
        private void NativeGameScreenRedraw(object sender, int[,] map)
        {
            nativeScreenImg.Source = gridImageList.GridToBitmapImage(map);
        }
        /// <summary>
        /// 本机游戏地图初始化事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="map"></param>
        private void NativeGameScreenRedraw(object sender, Tuple<int, int[,]> e)
        {
            nativeScreenImg.Source = gridImageList.GridToBitmapImage(e.Item2);
        }

        private void NativeGameOver(object sender, EventArgs e)
        {
            MessageBox.Show("你赢了");
        }
    }
}
