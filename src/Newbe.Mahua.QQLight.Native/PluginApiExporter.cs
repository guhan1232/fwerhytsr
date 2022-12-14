using System;
using System.Runtime.InteropServices;
using Newbe.Mahua.Logging;
using Newbe.Mahua.QQLight.MahuaEventOutputs;
using Newtonsoft.Json;

namespace Newbe.Mahua.QQLight.Native
{
    public class PluginApiExporter : IPluginApiExporter
    {
        private static readonly ILog Logger = LogProvider.For<PluginApiExporter>();

        private const string Continue = "0";

        private const string SdkVersion = "1";

        [DllExport("Information", CallingConvention.StdCall)]
        public static string Information(int authCode)
        {
            QqLightAuthCodeContainer.StaticAuthCode = authCode;
            PluginInstanceManager.GetInstance()
                .HandleMahuaOutput(new Information
                {
                    AuthCode = authCode,
                });
            var info =
                $"{{\"plugin_id\":\"{AgentInfo.Instance.Id}\",\"plugin_name\":\"{AgentInfo.Instance.Name}\",\"plugin_author\":\"{AgentInfo.Instance.Author}\",\"plugin_version\":\"{AgentInfo.Instance.Version}\",\"plugin_brief\":\"{AgentInfo.Instance.Description}\",\"plugin_sdk\":\"{SdkVersion}\",\"plugin_menu\":\"true\"}}";
            Logger.Info("Plugin Info :{info}", info);

            return info;
        }

        /// <summary>
        /// 初始化插件，插件加载时会调用此事件
        /// </summary>
        /// <returns></returns>
        [DllExport("Event_Initialization", CallingConvention.StdCall)]
        public static int Event_Initialization()
        {
            Native.PluginInstanceManager.GetInstance().HandleMahuaOutput(new Initialization());
            return 0;
        }

        /// <summary>
        /// 插件被启用事件
        /// </summary>
        /// <returns></returns>
        [DllExport("Event_pluginStart", CallingConvention.StdCall)]
        public static int Event_pluginStart()
        {
            Native.PluginInstanceManager.GetInstance().HandleMahuaOutput(new PluginStart());
            return 0;
        }

        /// <summary>
        /// 插件被关闭事件
        /// </summary>
        /// <returns></returns>
        [DllExport("Event_pluginStop", CallingConvention.StdCall)]
        public static int Event_pluginStop()
        {
            PluginInstanceManager.GetInstance().HandleMahuaOutput(new PluginStop());
            return 0;
        }

        /// <summary>
        /// 获取最新信息(好友/群/群临时/讨论组/讨论组临时消息)事件
        /// </summary>
        /// <param name="type"> 1.好友消息 2.群消息 3.群临时消息 4.讨论组消息 5.讨论组临时消息</param>
        /// <param name="fromgroup">类型=1的时候，此参数为空，其余情况下为 群号或讨论组号</param>
        /// <param name="fromqq"></param>
        /// <param name="message"></param>
        /// <param name="messageId"></param>
        /// <returns></returns>
        [DllExport("Event_GetNewMsg", CallingConvention.StdCall)]
        public static string Event_GetNewMsg(int type,
            string fromgroup,
            string fromqq,
            string message,
            string messageId)
        {
            PluginInstanceManager.GetInstance().HandleMahuaOutput(new GetNewMsg
            {
                Message = message,
                Type = type,
                Fromgroup = fromgroup,
                Fromqq = fromqq,
                MessageId = messageId,
            });
            return Continue;
        }

        /// <summary>
        /// QQ财付通转账事件
        /// </summary>
        /// <param name="type">1.好友转账 2.群临时转账 3.讨论组临时转账</param>
        /// <param name="fromgroup">类型1.此参数为空 2.群号 3.讨论组号</param>
        /// <param name="fromqq">转账的QQ</param>
        /// <param name="money">转账金额</param>
        /// <param name="friendRemark">QQ转账对方备注</param>
        /// <param name="orderNo">QQ转账获取的订单号</param>
        /// <returns></returns>
        [DllExport("Event_GetQQWalletData", CallingConvention.StdCall)]
        public static string Event_GetQQWalletData(
            int type,
            string fromgroup,
            string fromqq,
            string money,
            string friendRemark,
            string orderNo)
        {
            PluginInstanceManager.GetInstance().HandleMahuaOutput(new GetQQWalletData
            {
                Type = type,
                Fromqq = fromqq,
                Fromgroup = fromgroup,
                FriendRemark = friendRemark,
                Money = money,
                OrderNo = orderNo
            });
            return Continue;
        }

        /// <summary>
        /// 管理员变动事件
        /// </summary>
        /// <param name="type">1.xx被添加管理 2.xx被解除管理</param>
        /// <param name="fromgroup"></param>
        /// <param name="fromqq"></param>
        /// <returns></returns>
        [DllExport("Event_AdminChange", CallingConvention.StdCall)]
        public static string Event_AdminChange(int type, string fromgroup, string fromqq)
        {
            PluginInstanceManager.GetInstance().HandleMahuaOutput(new AdminChange
            {
                Type = type,
                Fromqq = fromqq,
                Fromgroup = fromgroup
            });
            return Continue;
        }

        /// <summary>
        /// 群成员增加事件
        /// </summary>
        /// <param name="type">1.主动入群  2.被xxx邀请入群</param>
        /// <param name="fromgroup">群号</param>
        /// <param name="fromqq">进群QQ</param>
        /// <param name="operatorQq"> 类型为1.管理员 2.邀请人</param>
        /// <returns></returns>
        [DllExport("Event_GroupMemberIncrease", CallingConvention.StdCall)]
        public static string Event_GroupMemberIncrease(
            int type,
            string fromgroup,
            string fromqq,
            string operatorQq)
        {
            PluginInstanceManager.GetInstance().HandleMahuaOutput(new GroupMemberIncrease
            {
                Type = type,
                Fromqq = fromqq,
                Fromgroup = fromgroup,
                OperatorQq = operatorQq,
            });
            return Continue;
        }

        /// <summary>
        /// 群成员减少事件
        /// </summary>
        /// <param name="type">1.主动退群  2.被xxx踢出群</param>
        /// <param name="fromgroup">群号</param>
        /// <param name="fromqq">退群QQ</param>
        /// <param name="operatorQq">类型为1时参数为空</param>
        /// <returns></returns>
        [DllExport("Event_GroupMemberDecrease", CallingConvention.StdCall)]
        public static string Event_GroupMemberDecrease(
            int type,
            string fromgroup,
            string fromqq,
            string operatorQq)
        {
            PluginInstanceManager.GetInstance().HandleMahuaOutput(new GroupMemberDecrease
            {
                Type = type,
                Fromqq = fromqq,
                Fromgroup = fromgroup,
                OperatorQq = operatorQq
            });
            return Continue;
        }

        /// <summary>
        /// 群添加事件
        /// </summary>
        /// <param name="type">1.主动加群  2.被邀请进群 3.机器人被邀请入群</param>
        /// <param name="fromgroup"></param>
        /// <param name="fromqq"></param>
        /// <param name="invatorQq">邀请者QQ</param>
        /// <param name="moreMsg">加群者的附加信息，类型为2，3时参数为空</param>
        /// <param name="seq">群添加事件产生的Seq标识</param>
        /// <returns></returns>
        [DllExport("Event_AddGroup", CallingConvention.StdCall)]
        public static string Event_AddGroup(
            int type,
            string fromgroup,
            string fromqq,
            string invatorQq,
            string moreMsg,
            string seq)
        {
            PluginInstanceManager.GetInstance().HandleMahuaOutput(new AddGroup
            {
                Type = type,
                Fromqq = fromqq,
                Fromgroup = fromgroup,
                InvatorQq = invatorQq,
                MoreMsg = moreMsg,
                Seq = seq
            });
            return Continue;
        }

        /// <summary>
        /// 好友添加事件
        /// </summary>
        /// <param name="fromqq"></param>
        /// <param name="reason">好友添加理由</param>
        /// <returns></returns>
        /// 这个拼写就是这样，没毛病
        [DllExport("Event_AddFrinend", CallingConvention.StdCall)]
        public static string Event_AddFrinend(string fromqq, string reason)
        {
            PluginInstanceManager.GetInstance().HandleMahuaOutput(new AddFrinend
            {
                Fromqq = fromqq,
                Reason = reason
            });
            return Continue;
        }

        /// <summary>
        /// 好友变动事件（包含成为单向好友，双向好友，被好友删除）
        /// type 1.成为好友（单向） 2、成为好友（双向） 3、解除好友关系
        /// </summary>
        [DllExport("Event_FriendChange", CallingConvention.StdCall)]
        public static string Event_FriendChange(int type, string fromqq)
        {
            PluginInstanceManager.GetInstance().HandleMahuaOutput(new FriendChange
            {
                Fromqq = fromqq,
                Type = type
            });
            return Continue;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        [DllExport("Event_Menu", CallingConvention.StdCall)]
        public static int Event_Menu()
        {
            // TODO 点击设置中心，暂时没有任何作用
            Console.WriteLine("nothing");
            return 0;
        }
    }
}