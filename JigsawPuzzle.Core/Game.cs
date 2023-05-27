using System;
using System.Collections.Generic;

namespace JigsawPuzzle.Core
{
    public class Game
    {
        #region 事件
        /// <summary>
        /// 地图初始化事件
        /// </summary>
        public EventHandler<int[,]> InitMapAfter = null;
        /// <summary>
        /// 移动事件
        /// </summary>
        //public EventHandler<> MoveEvent = null;
        public EventHandler<Tuple<int, int[,]>> MoveEvent = null;
        /// <summary>
        /// 游戏结束事件
        /// </summary>
        public EventHandler GameOverEvent = null;
        #endregion
        private bool _gameOver = true;
        public bool IsGameOver { get { return _gameOver; } }
        private int _mapSize;
        private int[,] _map;
        private int _step = 0;
        private CoordinatePoint _currentPoint;
        public Game()
        {
            _mapSize = 3;
            _map = new int[_mapSize, _mapSize];
        }
        public Game(int mapSize)
        {
            if (mapSize > 9 && mapSize < 3)
            {
                throw new ArgumentException("地图大小不能超过9格小于3格");
            }
            _mapSize = mapSize;
            _map = new int[_mapSize, _mapSize];
        }
        public Game(int[,] map)
        {
            _step = 0;
            _mapSize = map.GetLength(0);
            _map = map;
            _currentPoint = new CoordinatePoint(0, 0, 0);
            _gameOver = false;
        }
        /// <summary>
        /// 初始化地图
        /// </summary>
        public void InitMap()
        {
            _step = 0;
            //用地图大小计算格子数
            int cellCount = _mapSize * _mapSize;
            List<int> cells = new List<int>();
            //第一个固定值为0所以从1开始
            for (int i = 1; i < cellCount; i++)
            {
                cells.Add(i);
            }
            //填充地图
            _map[0,0] = 0;
            Random rd = new Random(DateTime.Now.Millisecond);
            for (int y = 1; y < _mapSize; y++)
            {
                int rdIndex = rd.Next(0, cells.Count - 1);
                _map[0, y] = cells[rdIndex];
                cells.RemoveAt(rdIndex);
            }
            for (int x = 1; x < _mapSize; x++)
            {
                for (int y = 0; y < _mapSize; y++)
                {
                    int rdIndex = rd.Next(0, cells.Count - 1);
                    _map[x, y] = cells[rdIndex];
                    cells.RemoveAt(rdIndex);
                }
            }
            _currentPoint = new CoordinatePoint(0, 0, 0);
            InitMapAfter?.Invoke(null, _map);
            _gameOver = false;
        }
        /// <summary>
        /// 移动操作
        /// </summary>
        /// <param name="operation"></param>
        public void Move(OperationType operation)
        {
            if (_gameOver)
            {
                return;
            }
            switch (operation)
            {
                case OperationType.Up:
                    MoveToNext(_currentPoint.X, _currentPoint.Y + 1);
                    break;
                case OperationType.Down:
                    MoveToNext(_currentPoint.X, _currentPoint.Y - 1);
                    break;
                case OperationType.Left:
                    MoveToNext(_currentPoint.X + 1, _currentPoint.Y);
                    break;
                case OperationType.Right:
                    MoveToNext(_currentPoint.X - 1, _currentPoint.Y);
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(operation));
            }
        }
        /// <summary>
        /// 当前点移动到下一个点
        /// </summary>
        private void MoveToNext(int x, int y)
        {
            if (x < 0 || x >= _mapSize || y < 0 || y >= _mapSize)
            {
                return;
            }
            _step++;
            var nextPoint = new CoordinatePoint(x, y, _currentPoint.Value);
            int value = _map[nextPoint.X, nextPoint.Y];
            _map[nextPoint.X, nextPoint.Y] = nextPoint.Value;
            _map[_currentPoint.X, _currentPoint.Y] = value;
            _currentPoint.Value = value;
            //var moveEventArg = new Tuple<CoordinatePoint, CoordinatePoint>(nextPoint, _currentPoint);
            _currentPoint = nextPoint;
            MoveEvent?.Invoke(null, new Tuple<int, int[,]>(_step, _map));
            GameOverJudging();
        }
        /// <summary>
        /// 判定游戏结束
        /// </summary>
        private void GameOverJudging()
        {
            int value = 0;
            for (int y = 0; y < _mapSize; y++)
            {
                for (int x = 0; x < _mapSize; x++)
                {
                    if (_map[x, y] == value)
                    {
                        value++;
                    }
                    else 
                    { 
                        return ;
                    }
                }
            }
            GameOverEvent?.Invoke(null, EventArgs.Empty);
            _gameOver = true;
        }
        public void ConsoleWrite()
        {
            Console.Clear();
            for (int y = 0; y < _mapSize; y++)
            {
                for (int x = 0; x < _mapSize; x++)
                {
                    Console.Write(string.Format("{0:d3} ", _map[x, y]));
                }
                Console.WriteLine("");
            }
            Console.WriteLine($"当前已走 {_step} 步");
            if (_gameOver)
            {
                Console.WriteLine("游戏结束");
            }
        }
    }
}
