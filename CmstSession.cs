using System;
using System.Collections.Generic;
using SuperSocket.WebSocket;
using SuperSocket.SocketBase;
using CmstService.SocketServer.JsonObject;

namespace CmstService.SocketServer
{
    // Cmst Socket 通信会话
    public class CmstSession : WebSocketSession<CmstSession>
    {
        // 重写服务器
        public new CmstServer AppServer
        {
            get { return (CmstServer)base.AppServer; }
        }
       
        // 用户名
        public string User { get; internal set; }

        // 是否登录
        //public bool IsLoggedIn { get; internal set; }

        // 订阅消息列表
        //public List<string> SubscriptionList { get; internal set; }

        // 1、先从 Cookies 中取得相关信息
        // 当登录之后，系统会在 Cookies 中设置用户信息，如果用户浏览器支持写入 Cookie 的话，则会存有如下信息：
        // { "userList": "...", "currentSession": "..." }，
        // currentSession的Value是经过DES对 IP + USER 加密后用于服务器验证登录的字符串，只有登录后的用户才会设置该键值对，组成格式为：USER|DESKEY
        // 如果用户的浏览器禁用了Cookie，则需要会话建立后发送 SIGNIN 命令，把本地 Local Storage 中与Cookie一致的信息发送到服务器进行验证
        // 2、如果 Cookies 中未取得信息，则从服务器相同IP的会话中取得信息，如果没有相同IP的会话，则等待SIGNIN指令，否则以匿名用户保持会话
            
        // 后，经过测试发现 Socket 这个 Cookies 并没有序列化，原来是 WebSocket 并不像 HTTP 一样携带 Cookie 头
        // 不过如今H5的本地存储策略比较多，Web Storage、IndexedDB、Web SQL等，IndexedDB API 取代了 Web Storage API，后者在 HTML5 规范中已不推荐使用
        // 综上，最终选择了IndexedDB，因为数据库存储数据必然是主流的选择，今后ECMA的支持力度必然更大，前景好
        // 因此，在此统一了客户端登录验证流程，并移除了重写的 OnSessionStarted 事件方法：
        // 1、页面刷新、新开后，先建立 WebSocket 连接；
        // 2、握手后，发送 SIGNIN 命令，将相关数据发送到服务端验证；
        // 3、验证通过后，服务器会向客户端发送用户信息，如果需要跳转则优先发送跳转信息，跳转后重复1-3，直至最终信息一致。
    }
}
