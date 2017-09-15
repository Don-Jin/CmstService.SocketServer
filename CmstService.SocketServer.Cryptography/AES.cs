using System;
using System.Text;
using Newtonsoft.Json;
using CmstService.SocketServer.JsonObject;

namespace CmstService.SocketServer.Cryptography
{
    // 对称 Key 加密
    public sealed class AES
    {
        private AES()
        { 
            
        }

        // 对称密钥
        private static string[] rgbKey = new string[2] { "0123456789ABCDEFGHIJKLMNOPQRSTUV", "" };

        // 初始化向量
        private static string[] rgbIV = new string[2] { "ZYXWVUTSRQPONMLK", "" };

        // 加密服务提供程序
        // 默认 CipherMode = CBC，PaddingMode = PKCS7
        private static System.Security.Cryptography.AesCryptoServiceProvider provider = null;

        // 是否自定义密钥、向量
        public static bool IsCustom { get; set; }

        public static string RgbKey {
            get { return IsCustom ? rgbKey[1] : rgbKey[0]; }
            set 
            { 
                if (IsCustom)
                {
                    rgbKey[1] = value.Length > 32 ? value.Substring(0, 32) : value.PadRight(32, 'X'); // 32位密钥
                }
            }
        }

        public static string RgbIV
        {
            get { return IsCustom ? rgbIV[1] : rgbIV[0]; }
            set 
            {
                if (IsCustom)
                {
                    rgbIV[1] = value.Length > 16 ? value.Substring(0, 16) : value.PadRight(16, 'X');
                }
            }
        }

        // 公用算法
        private static string CommonCryptography(string origin, bool encrypt)
        {
            string res = "";
            try
            {
                if (provider == null)
                {
                    // 加密解密器实例
                    provider = new System.Security.Cryptography.AesCryptoServiceProvider();
                }
                byte[] byteKey = Encoding.UTF8.GetBytes(RgbKey);
                byte[] byteIV = Encoding.UTF8.GetBytes(RgbIV);
                byte[] byteEncryt = encrypt ? Encoding.UTF8.GetBytes(origin) : Convert.FromBase64String(origin);
                // 加密解密运算转换
                System.Security.Cryptography.ICryptoTransform transform = encrypt ? provider.CreateEncryptor(byteKey, byteIV) : provider.CreateDecryptor(byteKey, byteIV);
                byte[] byteResult = transform.TransformFinalBlock(byteEncryt, 0, byteEncryt.Length);
                res = encrypt ? Convert.ToBase64String(byteResult) : Encoding.UTF8.GetString(byteResult);
            }
            //catch (FormatException e) { throw new Exception(JsonConvert.SerializeObject(new Error(e.GetType().Name, e.Message))); }
            //catch (ArgumentNullException e) { throw new Exception(JsonConvert.SerializeObject(new Error(e.GetType().Name, e.Message))); }
            //catch (ArgumentOutOfRangeException e) { throw new Exception(JsonConvert.SerializeObject(new Error(e.GetType().Name, e.Message))); }
            //catch (EncoderFallbackException e) { throw new Exception(JsonConvert.SerializeObject(new Error(e.GetType().Name, e.Message))); }
            //catch (ArgumentException e) { throw new Exception(JsonConvert.SerializeObject(new Error(e.GetType().Name, e.Message))); }
            //catch (PlatformNotSupportedException e) { throw new Exception(JsonConvert.SerializeObject(new Error(e.GetType().Name, e.Message))); }
            //catch (NotSupportedException e) { throw new Exception(JsonConvert.SerializeObject(new Error(e.GetType().Name, e.Message))); }
            catch (Exception e) { throw e; }
            return res;
        }

        // 加密算法
        public static string Encrypt(string origin)
        {
            return CommonCryptography(origin, true);
        }

        // 解密算法
        public static string Decrypt(string encrypt)
        {
            return CommonCryptography(encrypt, false);
        }
    }
}
