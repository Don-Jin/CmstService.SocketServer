using System;
using System.Text;
using Newtonsoft.Json;
using CmstService.SocketServer.JsonObject;

namespace CmstService.SocketServer.Cryptography
{
    public sealed class RSA
    {
        // 私有构造，防止被实例
        private RSA() 
        { 
        
        }

        // 公钥
        private static string[] publicKey = new string[2] { @"<RSAKeyValue><Modulus>5m9m14XH3oqLJ8bNGw9e4rGpXpcktv9MSkHSVFVMjHbfv+SJ5v0ubqQxa5YjLN4vc49z7SVju8s0X4gZ6AzZTn06jzWOgyPRV54Q4I0DCYadWW4Ze3e+BOtwgVU1Og3qHKn8vygoj40J6U85Z/PTJu3hN1m75Zr195ju7g9v4Hk=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>", "" };
        
        // 私钥
        private static string[] privateKey = new string[2] { @"<RSAKeyValue><Modulus>5m9m14XH3oqLJ8bNGw9e4rGpXpcktv9MSkHSVFVMjHbfv+SJ5v0ubqQxa5YjLN4vc49z7SVju8s0X4gZ6AzZTn06jzWOgyPRV54Q4I0DCYadWW4Ze3e+BOtwgVU1Og3qHKn8vygoj40J6U85Z/PTJu3hN1m75Zr195ju7g9v4Hk=</Modulus><Exponent>AQAB</Exponent><P>/hf2dnK7rNfl3lbqghWcpFdu778hUpIEBixCDL5WiBtpkZdpSw90aERmHJYaW2RGvGRi6zSftLh00KHsPcNUMw==</P><Q>6Cn/jOLrPapDTEp1Fkq+uz++1Do0eeX7HYqi9rY29CqShzCeI7LEYOoSwYuAJ3xA/DuCdQENPSoJ9KFbO4Wsow==</Q><DP>ga1rHIJro8e/yhxjrKYo/nqc5ICQGhrpMNlPkD9n3CjZVPOISkWF7FzUHEzDANeJfkZhcZa21z24aG3rKo5Qnw==</DP><DQ>MNGsCB8rYlMsRZ2ek2pyQwO7h/sZT8y5ilO9wu08Dwnot/7UMiOEQfDWstY3w5XQQHnvC9WFyCfP4h4QBissyw==</DQ><InverseQ>EG02S7SADhH1EVT9DD0Z62Y0uY7gIYvxX/uq+IzKSCwB8M2G7Qv9xgZQaQlLpCaeKbux3Y59hHM+KpamGL19Kg==</InverseQ><D>vmaYHEbPAgOJvaEXQl+t8DQKFT1fudEysTy31LTyXjGu6XiltXXHUuZaa2IPyHgBz0Nd7znwsW/S44iql0Fen1kzKioEL3svANui63O3o5xdDeExVM6zOf1wUUh/oldovPweChyoAdMtUzgvCbJk1sYDJf++Nr0FeNW1RB1XG30=</D></RSAKeyValue>", "" };
        
        // 加解密器实例
        private static System.Security.Cryptography.RSACryptoServiceProvider provider = null;

        // 是否自定义公钥、私钥
        public static bool IsCustom { get; set; }

        public static string PublicKey
        {
            get 
            {
                return IsCustom ? publicKey[1] : publicKey[0]; 
            }
            set 
            { 
                if (IsCustom)
                {
                    publicKey[1] = value;
                }
            }
        }

        public static string PrivateKey
        {
            get 
            {
                return IsCustom ? privateKey[1] : privateKey[0]; 
            }
            set 
            {
                if (IsCustom)
                {
                    privateKey[1] = value;
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
                    provider = new System.Security.Cryptography.RSACryptoServiceProvider();
                }
                provider.FromXmlString(encrypt ? PublicKey : PrivateKey);
                byte[] resBytes = encrypt ? provider.Encrypt(Encoding.UTF8.GetBytes(origin), false) : provider.Decrypt(Convert.FromBase64String(origin), false);
                res = encrypt ? Convert.ToBase64String(resBytes) : Encoding.UTF8.GetString(resBytes);
            }
            //catch (FormatException e) { throw new Exception(JsonConvert.SerializeObject(new Error(e.GetType().Name, e.Message))); }
            //catch (ArgumentNullException e) { throw new Exception(JsonConvert.SerializeObject(new Error(e.GetType().Name, e.Message))); }
            //catch (EncoderFallbackException e) { throw new Exception(JsonConvert.SerializeObject(new Error(e.GetType().Name, e.Message))); }
            //catch (ArgumentException e) { throw new Exception(JsonConvert.SerializeObject(new Error(e.GetType().Name, e.Message))); }
            //catch (System.Security.Cryptography.CryptographicException e) { throw new Exception(JsonConvert.SerializeObject(new Error(e.GetType().Name, e.Message))); }
            catch (Exception e) { throw e; }
            return res;
        }

        // RSA加密
        public static string Encrypt(string origin)
        {
            return CommonCryptography(origin, true);
        }

        // RSA解密
        public static string Decrypt(string encrypt)
        {
            return CommonCryptography(encrypt, false);
        }
    }
}
