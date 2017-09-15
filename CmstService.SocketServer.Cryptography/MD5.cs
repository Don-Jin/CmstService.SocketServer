using System;
using System.Text;
using Newtonsoft.Json;
using CmstService.SocketServer.JsonObject;

namespace CmstService.SocketServer.Cryptography
{
    public sealed class MD5
    {
        private MD5()
        { 
        
        }

        // MD5不可逆，只有加密算法
        // 可能的异常：
        // System.ArgumentException
        // System.ArgumentNullException，继承自System.ArgumentException
        // System.Text.EncoderFallbackException
        // System.InvalidOperationException
        // System.ObjectDisposedException，继承自System.ArgumentException
        // System.FormatException
        // 为简便化，但也为测试好找错误来源，统一进行了异常封装，全部统一为基类 Exception
        // 并且将 Message 序列化为 JSON 数据，在使用时可以反序列化 Message 为 ErrorMessage，可获得内容
        // 如果只是 Socket 的简单发送，直接发送 Message 即可
        public static string Encrypt(string origin)
        {
            string res = "";
            try
            {
                byte[] byteKey = Encoding.GetEncoding("UTF8").GetBytes(origin);
                foreach (byte key in new System.Security.Cryptography.MD5CryptoServiceProvider().ComputeHash(byteKey))
                {
                    res += key.ToString("x2");
                }
            }
            //catch (FormatException e) { throw new Exception(JsonConvert.SerializeObject(new Error(e.GetType().Name, e.Message))); }
            //catch (ArgumentNullException e) { throw new Exception(JsonConvert.SerializeObject(new Error(e.GetType().Name, e.Message))); }
            //catch (ObjectDisposedException e) { throw new Exception(JsonConvert.SerializeObject(new Error(e.GetType().Name, e.Message))); }
            //catch (EncoderFallbackException e) { throw new Exception(JsonConvert.SerializeObject(new Error(e.GetType().Name, e.Message))); }
            //catch (InvalidOperationException e) { throw new Exception(JsonConvert.SerializeObject(new Error(e.GetType().Name, e.Message))); }
            //catch (ArgumentException e) { throw new Exception(JsonConvert.SerializeObject(new Error(e.GetType().Name, e.Message))); }
            catch (Exception e) { throw e; }
            return res.ToLower();
        }
    }
}
