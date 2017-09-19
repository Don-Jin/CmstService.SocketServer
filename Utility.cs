using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using CmstService.SocketServer.JsonObject;
using CmstService.SocketServer.Cryptography;
using CmstService.SocketServer.ConfigurationHelper.JsonConfig;

namespace CmstService.SocketServer
{
    public static class Utility
    {
        // 会话关闭本地化表达
        private static string[] localeCloseReason = new string[9] { "未知错误", "服务器关闭", "客户端连接中断", "服务端连接中断", "应用程序异常", "通信异常", "通信超时", "通信协议异常", "服务器内部异常" };

        public static string GetLocaleCloseReason(int id)
        {
            return localeCloseReason[id];
        }
        /*
        public static string anonymousPrefix = "路人";

        public static string anonymousGroup = "匿名组";

        // 匿名用户
        private static string anonymousName = "甲乙丙丁戊己庚辛壬癸子丑寅卯辰巳午未申酉戌亥夏商周秦楚汉魏晋隋唐宋明";

        // 取得匿名用户名
        public static string AnonymousUser 
        {
            get
            {
                // 随机取整数，然后从指定字符串中截取一个字符
                return anonymousName.Substring(new Random(Guid.NewGuid().GetHashCode()).Next(anonymousName.Length), 1);
            }
        }
        */
        // 获取本机IP
        public static IPAddress GetHostIPAddress()
        { 
            foreach (IPAddress ip in Dns.GetHostAddresses(Dns.GetHostName()))
            {
                // 只获取IPV4
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip;
                }
            }
            return null;
        }

        // 获取对称加密DES的密钥
        public static string GetDesKey 
        {
            get 
            {
                DES.IsCustom = false;
                return DES.RgbKey; 
            }
        }

        // 获取对称加密DES的向量
        public static string GetDesIV
        {
            get 
            {
                DES.IsCustom = false;
                return DES.RgbIV; 
            }
        }

        // 自定义DES密钥
        private static string cryptoKey = "CmstService.SocketServer";

        // 自定义DES向量
        private static string cryptoIV = "revreStekcoS.ecivreStsmC";

        // 加密解密，加密 = encrypt 为 true
        public static string DESCryptography(string origin, bool encrypt = true)
        {
            // 初始化加密器
            DES.IsCustom = true;
            DES.RgbKey = cryptoKey;
            DES.RgbIV = cryptoIV;

            return encrypt ? DES.Encrypt(origin) : DES.Decrypt(origin);
        }

        public static string AESCryptography(string origin, bool encrypt = true)
        {
            // 初始化加密器
            AES.IsCustom = true;
            AES.RgbKey = cryptoKey;
            AES.RgbIV = cryptoIV;
            
            return encrypt ? AES.Encrypt(origin) : AES.Decrypt(origin);
        }

        // 根据AES加密子串，匹配登录状态
        // 加密算法：AES (USER + '|' + DATE + '|' + IP)
        public static bool IsLogin(string user, string ip, string encrypt) 
        {
            if (string.IsNullOrEmpty(encrypt))
            {
                return false;
            }
            try
            {
                string[] decrypt = AESCryptography(encrypt, false).Split('|');
                return decrypt[0].Equals(user) && decrypt[2].Equals(ip);
            }
            catch { return false; }
        }

        // 处理页面重定向的URL
        public static string[] GetRedirectUrl(string baseUrl, string userGroup, string userName, string currentSession)
        {
            string[] result = new string[2] { baseUrl, baseUrl }; 
            try
            {
                Match match = Regex.Match(baseUrl, @"((?:http[s]?:\/\/|)[^\/]+)(.*)", RegexOptions.IgnoreCase);
                string[] urls = new string[2] { match.Groups[1].Value, match.Groups[2].Value }; 
                string[] path = urls[1].Split('/');
                int len = path.GetUpperBound(0);
                string last = path[len];
                path[len] = string.Join("/", new string[] { userGroup, userName, last });
                result[0] = (urls[0] + "/" + string.Join("/", path)).Replace("//", "/");
                path[len] = string.Join("/", new string[] { userGroup, userName, currentSession, last });
                result[1] = (urls[0] + "/" + string.Join("/", path)).Replace("//", "/");
            }
            catch { }
            return result;
        }

        // 检测命令权限
        public static bool IsPrivilegeCommand(string operation)
        {
            return !string.IsNullOrEmpty(operation) && operation.ToUpper().Equals("SELECT");
        }

        public static bool IsPrivilegeCommand(QueryMessage query, ref List<QueryConfig2> list)
        {
            // 处理预反射信息
            Dictionary<string, List<QueryInfo>> refConf = new Dictionary<string, List<QueryInfo>>();
            foreach (QueryInfo info in query.QueryList)
            {
                if (refConf.ContainsKey(info.DatabaseName))
                {
                    refConf[info.DatabaseName].Add(info);
                    continue;
                }
                refConf.Add(info.DatabaseName, new List<QueryInfo>() { info });
            }

            // 获取SQL执行语句
            foreach (string key in refConf.Keys)
            {
                list.Add(ReflectionHelper.GetSqls(key, refConf[key]));
            }

            // 检测SQL语句的执行权限
            foreach (QueryConfig2 queryConf in list)
            {
                foreach (CommandConfig cmdConf in queryConf.QueryList)
                {
                    if (!IsPrivilegeCommand(cmdConf.SqlExpression.Type))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public static bool IsPrivilegeCommand(FunctionMessage call)
        {
            return IsPrivilegeCommand(ReflectionHelper.GetMethod(call.DatabaseName, call.AssemblyName, call.MethodName).Type);
        }
    }
}
