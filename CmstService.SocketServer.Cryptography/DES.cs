using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using CmstService.SocketServer.JsonObject;

namespace CmstService.SocketServer.Cryptography
{
    // 对称 Key 加密
    public sealed class DES
    {
        private DES()
        { 
        
        }

        // 对称密钥
        // 原为Byte[]数组，{ 0x12, 0x34, 0x56, 0x78, 0x90, 0xab, 0xcd, 0xef }，经过Convert.ToBase64String()得到
        private static string[] rgbKey = new string[2] { "EjRWeJCrze8=", "" };

        // 初始化向量
        // 原为Byte[]数组，{ 0x37, 0x67, 0xf6, 0x4f, 0x24, 0x63, 0xa7, 0x03 }
        private static string[] rgbIV = new string[2] { "N2f2TyRjpwM=", "" };

        // 加密服务提供程序
        // 默认 CipherMode = CBC，PaddingMode = PKCS7
        private static System.Security.Cryptography.DESCryptoServiceProvider provider = null;

        // 是否自定义密钥、向量
        public static bool IsCustom { get; set; }

        public static string RgbKey {
            get { return IsCustom ? rgbKey[1] : rgbKey[0]; }
            set 
            { 
                if (IsCustom)
                {
                    rgbKey[1] = value.Length > 8 ? value.Substring(0, 8) : value.PadRight(8, 'X'); 
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
                    rgbIV[1] = value.Length > 8 ? value.Substring(0, 8) : value.PadRight(8, 'X');
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
                    provider = new System.Security.Cryptography.DESCryptoServiceProvider();
                }
                byte[] byteKey = IsCustom ? Encoding.UTF8.GetBytes(RgbKey) : Convert.FromBase64String(RgbKey);
                byte[] byteIV = IsCustom ? Encoding.UTF8.GetBytes(RgbIV) : Convert.FromBase64String(RgbIV);
                byte[] byteEncryt = encrypt ? Encoding.UTF8.GetBytes(origin) : Convert.FromBase64String(origin);
                MemoryStream ms = new MemoryStream();
                System.Security.Cryptography.CryptoStream cs = new System.Security.Cryptography.CryptoStream(ms, encrypt ?
                    provider.CreateEncryptor(byteKey, byteIV) :
                    provider.CreateDecryptor(byteKey, byteIV),
                    System.Security.Cryptography.CryptoStreamMode.Write
                );
                cs.Write(byteEncryt, 0, byteEncryt.Length);
                cs.FlushFinalBlock();
                res = encrypt ? Convert.ToBase64String(ms.ToArray()) : Encoding.UTF8.GetString(ms.ToArray());
            }
            //catch (FormatException e) { throw new Exception(JsonConvert.SerializeObject(new Error(e.GetType().Name, e.Message))); }
            //catch (NotSupportedException e) { throw new Exception(JsonConvert.SerializeObject(new Error(e.GetType().Name, e.Message))); }
            //catch (ArgumentNullException e) { throw new Exception(JsonConvert.SerializeObject(new Error(e.GetType().Name, e.Message))); }
            //catch (ArgumentOutOfRangeException e) { throw new Exception(JsonConvert.SerializeObject(new Error(e.GetType().Name, e.Message))); }
            //catch (EncoderFallbackException e) { throw new Exception(JsonConvert.SerializeObject(new Error(e.GetType().Name, e.Message))); }
            //catch (ArgumentException e) { throw new Exception(JsonConvert.SerializeObject(new Error(e.GetType().Name, e.Message))); }
            //catch (System.Security.Cryptography.CryptographicException e) { throw new Exception(JsonConvert.SerializeObject(new Error(e.GetType().Name, e.Message))); }
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
