using JigsawPuzzle.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace JigsawPuzzle.WCF
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的接口名“IService1”。
    [ServiceContract(CallbackContract = typeof(IGameCallback))]
    public interface IGameService
    {
        [OperationContract]
        string Test1();
        /// <summary>
        /// 联机玩家加入游戏房间
        /// </summary>
        /// <returns></returns>
        [ServiceKnownType(typeof(Bitmap))]
        [OperationContract]
        Bitmap JoinGame();
        /// <summary>
        /// 联机玩家准备游戏
        /// </summary>
        [OperationContract(IsOneWay = true)]
        void ReadyGame();
        /// <summary>
        /// 发送联机玩家操作信息到服务端
        /// </summary>
        /// <param name="type"></param>
        [OperationContract(IsOneWay = true)]
        void OnlineUserMove(OperationType type);
    }

    public interface IGameCallback
    {
        [OperationContract]
        string Test2();
        /// <summary>
        /// 服务端开始游戏发送游戏数据到远端
        /// </summary>
        /// <param name="map"></param>
        /// <returns></returns>
        [OperationContract(IsOneWay = true)]
        void StartGame(byte[] map);
        /// <summary>
        /// 发送服务端玩家操作信息到联机端
        /// </summary>
        /// <param name="type"></param>
        [OperationContract(IsOneWay = true)]
        void Move(OperationType type);
    }
    // 使用下面示例中说明的数据约定将复合类型添加到服务操作。
    // 可以将 XSD 文件添加到项目中。在生成项目后，可以通过命名空间“JigsawPuzzle.WCF.ContractType”直接使用其中定义的数据类型。
    //[DataContract]
    //public class CompositeType
    //{
    //    bool boolValue = true;
    //    string stringValue = "Hello ";

    //    [DataMember]
    //    public bool BoolValue
    //    {
    //        get { return boolValue; }
    //        set { boolValue = value; }
    //    }

    //    [DataMember]
    //    public string StringValue
    //    {
    //        get { return stringValue; }
    //        set { stringValue = value; }
    //    }
    //}
}
