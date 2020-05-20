using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Easy.Common.Helpers
{
    public static class EncryptionHelper
    {
        /// <summary>
        /// RSA加密
        /// </summary>
        /// <param name="content">明文</param>
        /// <param name="publicKey">公钥Key</param>
        /// <param name="encoding">编码方式</param>
        /// <returns>密文</returns>
        public static string RSA加密(string content, string publicKey, Encoding encoding = null)
        {
            encoding = encoding ?? Encoding.UTF8;

            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();

            byte[] cipherbytes;

            rsa.FromXmlString(publicKey);

            cipherbytes = rsa.Encrypt(encoding.GetBytes(content), false);

            return Convert.ToBase64String(cipherbytes);
        }
        /// <summary>
        /// RSA解密
        /// </summary>
        /// <param name="content">密文</param>
        /// <param name="privateKey">私钥Key</param>
        /// <param name="encoding">编码方式</param>
        /// <returns>明文</returns>
        public static string RSA解密(string content, string privateKey, Encoding encoding = null)
        {
            encoding = encoding ?? Encoding.UTF8;

            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();

            byte[] cipherbytes;

            rsa.FromXmlString(privateKey);

            cipherbytes = rsa.Decrypt(Convert.FromBase64String(content), false);

            return encoding.GetString(cipherbytes);
        }

        /// <summary>
        /// RSA公钥证书加密
        /// </summary>
        /// <param name="content">明文</param>
        /// <param name="publicKeyFilePath">公钥证书文件路径</param>
        /// <param name="encoding">编码方式</param>
        /// <returns>密文</returns>
        public static string RSA公钥证书加密(string content, string publicKeyFilePath, Encoding encoding = null)
        {
            encoding = encoding ?? Encoding.UTF8;

            byte[] DataToEncrypt = encoding.GetBytes(content);

            X509Certificate2 cert = new X509Certificate2(publicKeyFilePath);

            RSACryptoServiceProvider rsa = (RSACryptoServiceProvider)cert.PublicKey.Key;

            SHA1 sh = new SHA1CryptoServiceProvider();

            byte[] cipherbytes = rsa.Encrypt(DataToEncrypt, false);

            return Convert.ToBase64String(cipherbytes);
        }

        /// <summary>
        /// RSA私钥证书解密
        /// </summary>
        /// <param name="content">密文</param>
        /// <param name="privateKeyFilePath">私钥证书文件路径</param>
        /// <param name="password">私钥证书密码</param>
        /// <param name="encoding"></param>
        /// <returns>明文</returns>
        public static string RSA私钥证书解密(string content, string privateKeyFilePath, string password, Encoding encoding = null)
        {
            encoding = encoding ?? Encoding.UTF8;

            byte[] DataToDecrypt = Convert.FromBase64String(content);

            X509Certificate2 cert = new X509Certificate2(privateKeyFilePath, password);

            //将证书testCertificate.pfx的私钥强制转换为一个RSACryptoServiceProvider对象，用于解密操作
            RSACryptoServiceProvider rsa = (RSACryptoServiceProvider)cert.PrivateKey;

            byte[] cipherbytes = rsa.Decrypt(DataToDecrypt, false);

            return encoding.GetString(cipherbytes);
        }

        /// <summary>
        /// DES加密（对称）
        /// </summary>
        /// <param name="content">明文</param>
        /// <param name="sKey">密钥</param>
        /// <returns>密文</returns>
        public static string DES加密(string content, string sKey)
        {
            //将要加密的内容转换成一个Byte数组
            byte[] inputByteArray = Encoding.Default.GetBytes(content);

            //创建一个DES加密服务提供者
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();

            //设置密钥和初始化向量
            des.Key = Encoding.ASCII.GetBytes(sKey);
            des.IV = Encoding.ASCII.GetBytes(sKey);

            //创建一个内存流对象
            MemoryStream ms = new MemoryStream();

            //创建一个加密流对象
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);

            //将要加密的文本写到加密流中
            cs.Write(inputByteArray, 0, inputByteArray.Length);

            //更新缓冲 */
            cs.FlushFinalBlock();

            //获取加密过的文本
            StringBuilder sb = new StringBuilder();

            foreach (byte b in ms.ToArray())
            {
                sb.AppendFormat("{0:X2}", b);
            }

            //释放资源
            cs.Close();
            ms.Close();
            des.Clear();

            //返回结果
            return sb.ToString();
        }

        /// <summary>
        /// DES解密（对称）
        /// </summary>
        /// <param name="sContent">密文</param>
        /// <param name="sKey">密钥</param>
        /// <returns>明文</returns>
        public static string DES解密(string sContent, string sKey)
        {
            /* 将要解密的内容转换成一个Byte数组 */
            byte[] inputByteArray = new byte[sContent.Length / 2];

            for (int x = 0; x < sContent.Length / 2; x++)
            {
                int i = (Convert.ToInt32(sContent.Substring(x * 2, 2), 16));
                inputByteArray[x] = (byte)i;
            }

            //创建一个DES加密服务提供者
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();

            //设置密钥和初始化向量
            des.Key = Encoding.ASCII.GetBytes(sKey);
            des.IV = Encoding.ASCII.GetBytes(sKey);

            //创建一个内存流对象
            MemoryStream ms = new MemoryStream();

            //创建一个加密流对象
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);

            //将要解密的文本写到加密流中
            cs.Write(inputByteArray, 0, inputByteArray.Length);

            //更新缓冲 */
            cs.FlushFinalBlock();

            string result = Encoding.Default.GetString(ms.ToArray());

            //释放资源
            cs.Close();
            ms.Close();
            des.Clear();

            return result;
        }
    }
}
