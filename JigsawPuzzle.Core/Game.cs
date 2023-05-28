using System;
using System.Collections.Generic;

namespace JigsawPuzzle.Core
{
    public class Game
    {
        private string _userName;
        public string UserName { get { return _userName; } }
        private bool _gameOver = true;
        public bool IsGameOver { get { return _gameOver; } }
        private int _mapSize;
        private int[,] _map;
        private int _step = 0;
        private CoordinatePoint _currentPoint;
        public Game(string userName)
        {
            _userName = userName;
            _mapSize = 3;
            _map = new int[_mapSize, _mapSize];
        }
        public Game(string userName, int mapSize)
        {
            if (mapSize > 9 && mapSize < 3)
            {
                throw new ArgumentException("��ͼ��С���ܳ���9��С��3��");
            }
            _userName = userName;
            _mapSize = mapSize;
            _map = new int[_mapSize, _mapSize];
        }
        public Game(string userName, int[,] map)
        {
            _userName = userName;
            _step = 0;
            _mapSize = map.GetLength(0);
            _map = map;
            _currentPoint = new CoordinatePoint(0, 0, 0);
            _gameOver = false;
        }
        /// <summary>
        /// ��ʼ����ͼ(�˰汾��ʼ���㷨������)
        /// </summary>
        //public void InitMap()
        //{
        //    _step = 0;
        //    //�õ�ͼ��С���������
        //    int cellCount = _mapSize * _mapSize;
        //    List<int> cells = new List<int>();
        //    //��һ���̶�ֵΪ0���Դ�1��ʼ
        //    for (int i = 1; i < cellCount; i++)
        //    {
        //        cells.Add(i);
        //    }
        //    //����ͼ
        //    _map[0,0] = 0;
        //    Random rd = new Random(DateTime.Now.Millisecond);
        //    for (int y = 1; y < _mapSize; y++)
        //    {
        //        int rdIndex = rd.Next(0, cells.Count - 1);
        //        _map[0, y] = cells[rdIndex];
        //        cells.RemoveAt(rdIndex);
        //    }
        //    for (int x = 1; x < _mapSize; x++)
        //    {
        //        for (int y = 0; y < _mapSize; y++)
        //        {
        //            int rdIndex = rd.Next(0, cells.Count - 1);
        //            _map[x, y] = cells[rdIndex];
        //            cells.RemoveAt(rdIndex);
        //        }
        //    }
        //    _currentPoint = new CoordinatePoint(0, 0, 0);
        //    EventBus.InitMapAfter?.Invoke(null, _map);
        //    _gameOver = false;
        //}

        public int[,] InitMap()
        {
            int i = 0;
            for (int y = 0; y < _mapSize; y++)
            {
                for (int x = 0; x < _mapSize; x++)
                {
                    _map[x, y] = i;
                    i++;
                }
            }
            _currentPoint = new CoordinatePoint(0, 0, 0);
            RandomMap(i * 10);
            _step = 0;
            EventBus.InitMapAfter?.Invoke(UserName, _map);
            _gameOver = false;
            return _map;
        }
        private void RandomMap(int count)
        {
            Random rd = new Random(DateTime.Now.Millisecond);
            for (int i = 0; i < count; i++)
            {
                Move((OperationType)rd.Next(0, 3), true);
            }
            for (int i = 1; i < _mapSize; i++)
            {
                Move(OperationType.Right, true);
                Move(OperationType.Down, true);
            }
        }
        /// <summary>
        /// �ƶ�����
        /// </summary>
        /// <param name="operation"></param>
        public void Move(OperationType operation, bool isInit=false)
        {
            if (!isInit && _gameOver)
            {
                return;
            }
            switch (operation)
            {
                case OperationType.Up:
                    MoveToNext(_currentPoint.X, _currentPoint.Y + 1, isInit);
                    break;
                case OperationType.Down:
                    MoveToNext(_currentPoint.X, _currentPoint.Y - 1, isInit);
                    break;
                case OperationType.Left:
                    MoveToNext(_currentPoint.X + 1, _currentPoint.Y, isInit);
                    break;
                case OperationType.Right:
                    MoveToNext(_currentPoint.X - 1, _currentPoint.Y, isInit);
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(operation));
            }
        }
        /// <summary>
        /// ��ǰ���ƶ�����һ����
        /// </summary>
        private void MoveToNext(int x, int y, bool isInit)
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
            if (!isInit)
            { 
                EventBus.MoveEvent?.Invoke(UserName, new Tuple<int, int[,]>(_step, _map));
                GameOverJudging();
            }
        }
        /// <summary>
        /// �ж���Ϸ����
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
            EventBus.GameOverEvent?.Invoke(UserName, EventArgs.Empty);
            _gameOver = true;
        }
        /// <summary>
        /// ������Ϸ
        /// </summary>
        public void GameOver()
        {
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
            Console.WriteLine($"��ǰ���� {_step} ��");
            if (_gameOver)
            {
                Console.WriteLine("��Ϸ����");
            }
        }
    }
}
