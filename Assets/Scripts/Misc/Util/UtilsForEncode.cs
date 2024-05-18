// ******************************************************************
//@file         UtilsForEncode.cs
//@brief        加密工具类
//@author       yufulao, yufulao@qq.com
//@createTime   2024.05.18 01:34:39
// ******************************************************************
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Yu
{
    /// <summary>
    /// 加密解密工具类
    /// </summary>
    public static class UtilsForEncode
    {
        //UTF-8中，中文字符占3B，英文字符和数字和符号占1B

        #region 其他工具

        /// <summary>
        /// 字节流转字符串
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string BytesToString(byte[] data)
        {
            return Encoding.UTF8.GetString(data);
        }

        /// <summary>
        /// 字符串转字节流
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] StringToByte(string data)
        {
            return Encoding.UTF8.GetBytes(data);
        }
        
        /// <summary>
        /// 限定指定字节数的秘钥
        /// </summary>
        /// <param name="key">原秘钥</param>
        /// <param name="orderByteCount"></param>
        /// <returns></returns>
        public static string TrimKey(string key,int orderByteCount)
        {
            //将输入字符串编码为UTF-8字节数组
            var utf8Bytes = StringToByte(key);
            //如果字节数组长度小于16，则用*填充至16字节
            if (utf8Bytes.Length < orderByteCount)
            {
                var paddedBytes = new byte[orderByteCount];
                Array.Copy(utf8Bytes, paddedBytes, utf8Bytes.Length);
                for (var i = utf8Bytes.Length; i < orderByteCount; i++)
                {
                    paddedBytes[i] = (byte)'*';
                }
                utf8Bytes = paddedBytes;
            }
            //如果字节数组长度大于16，则删除尾部多余的字节
            if (utf8Bytes.Length > orderByteCount)
            {
                Array.Resize(ref utf8Bytes, orderByteCount);
            }
            
            return BytesToString(utf8Bytes);
        }

        #endregion

        #region MD5

        /// <summary>
        /// MD5，单向加密成密文
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string ToMD5(string data)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            var bytes = Encoding.Default.GetBytes(data); //将要加密的字符串转换为字节数组
            var encryptData = md5.ComputeHash(bytes); //将字符串加密后也转换为字符数组
            return Convert.ToBase64String(encryptData); //将加密后的字节数组转换为加密字符串
        }

        #endregion

        #region Des

        /// <summary>
        /// DES加密
        /// </summary>
        /// <param name="data">加密数据</param>
        /// <param name="key">8位字符的密钥字符串</param>
        /// <param name="iv">8位字符的初始化向量字符串</param>
        /// <returns></returns>
        public static string DesEncrypt(string data, string key, string iv)
        {
            var byKey = Encoding.ASCII.GetBytes(key);
            var byIv = Encoding.ASCII.GetBytes(iv);

            var cryptoProvider = new DESCryptoServiceProvider();
            var i = cryptoProvider.KeySize;
            var ms = new MemoryStream();
            var cst = new CryptoStream(ms, cryptoProvider.CreateEncryptor(byKey, byIv), CryptoStreamMode.Write);

            var sw = new StreamWriter(cst);
            sw.Write(data);
            sw.Flush();
            cst.FlushFinalBlock();
            sw.Flush();
            return Convert.ToBase64String(ms.GetBuffer(), 0, (int) ms.Length);
        }

        /// <summary>
        /// DES解密
        /// </summary>
        /// <param name="data">解密数据</param>
        /// <param name="key">8位字符的密钥字符串(需要和加密时相同)</param>
        /// <param name="iv">8位字符的初始化向量字符串(需要和加密时相同)</param>
        /// <returns></returns>
        public static string DesDecrypt(string data, string key, string iv)
        {
            var byKey = Encoding.ASCII.GetBytes(key);
            var byIv = Encoding.ASCII.GetBytes(iv);

            byte[] byEnc;
            try
            {
                byEnc = Convert.FromBase64String(data);
            }
            catch
            {
                return null;
            }

            var cryptoProvider = new DESCryptoServiceProvider();
            var ms = new MemoryStream(byEnc);
            var cst = new CryptoStream(ms, cryptoProvider.CreateDecryptor(byKey, byIv), CryptoStreamMode.Read);
            var sr = new StreamReader(cst);
            return sr.ReadToEnd();
        }

        #endregion

        #region RSA

        /// <summary> 
        /// RSA加密数据 
        /// </summary> 
        /// <param name="data">要加密数据</param> 
        /// <param name="keyContainerName">密匙容器的名称</param> 
        /// <returns></returns> 
        public static string RsaEncryption(string data, string keyContainerName = null)
        {
            var param = new CspParameters {KeyContainerName = keyContainerName ?? "yufulao"}; //密匙容器的名称，保持加密解密一致才能解密成功
            using var rsa = new RSACryptoServiceProvider(param);
            var plainData = Encoding.Default.GetBytes(data); //将要加密的字符串转换为字节数组
            var encryptData = rsa.Encrypt(plainData, false); //将加密后的字节数据转换为新的加密字节数组
            return Convert.ToBase64String(encryptData); //将加密后的字节数组转换为字符串
        }

        /// <summary> 
        /// RSA解密数据 
        /// </summary> 
        /// <param name="data">要解密数据</param> 
        /// <param name="keyContainerName">密匙容器的名称</param> 
        /// <returns></returns> 
        public static string RsaDecrypt(string data, string keyContainerName = null)
        {
            var param = new CspParameters {KeyContainerName = keyContainerName ?? "yufulao"}; //密匙容器的名称，保持加密解密一致才能解密成功
            using var rsa = new RSACryptoServiceProvider(param);
            var encryptData = Convert.FromBase64String(data);
            var decryptData = rsa.Decrypt(encryptData, false);
            return Encoding.Default.GetString(decryptData);
        }

        #endregion

        #region Base64

        /// <summary>
        /// Base64加密
        /// </summary>
        /// <param name="data">需要加密的字符串</param>
        /// <returns></returns>
        public static string Base64Encrypt(string data)
        {
            return Base64Encrypt(data, new UTF8Encoding());
        }

        /// <summary>
        /// Base64加密
        /// </summary>
        /// <param name="data">需要加密的字符串</param>
        /// <param name="encode">字符编码</param>
        /// <returns></returns>
        public static string Base64Encrypt(string data, Encoding encode)
        {
            return Convert.ToBase64String(encode.GetBytes(data));
        }

        /// <summary>
        /// Base64解密
        /// </summary>
        /// <param name="data">需要解密的字符串</param>
        /// <returns></returns>
        public static string Base64Decrypt(string data)
        {
            return Base64Decrypt(data, new UTF8Encoding());
        }

        /// <summary>
        /// Base64解密
        /// </summary>
        /// <param name="data">需要解密的字符串</param>
        /// <param name="encode">字符的编码</param>
        /// <returns></returns>
        public static string Base64Decrypt(string data, Encoding encode)
        {
            return encode.GetString(Convert.FromBase64String(data));
        }

        #endregion

        #region SHA

        //SHA为不可逆加密方式
        public static string SHA1Encrypt(string data)
        {
            var bytes = System.Text.Encoding.Default.GetBytes(data);
            var encryptBytes = new System.Security.Cryptography.SHA1CryptoServiceProvider().ComputeHash(bytes);
            return Convert.ToBase64String(encryptBytes);
        }

        public static string SHA256Encrypt(string data)
        {
            var bytes = System.Text.Encoding.Default.GetBytes(data);
            var encryptBytes = new System.Security.Cryptography.SHA256CryptoServiceProvider().ComputeHash(bytes);
            return Convert.ToBase64String(encryptBytes);
        }

        public static string SHA384Encrypt(string data)
        {
            var bytes = System.Text.Encoding.Default.GetBytes(data);
            var encryptBytes = new System.Security.Cryptography.SHA384CryptoServiceProvider().ComputeHash(bytes);
            return Convert.ToBase64String(encryptBytes);
        }

        public static string SHA512Encrypt(string data)
        {
            var bytes = System.Text.Encoding.Default.GetBytes(data);
            var encryptBytes = new System.Security.Cryptography.SHA512CryptoServiceProvider().ComputeHash(bytes);
            return Convert.ToBase64String(encryptBytes);
        }

        #endregion

        #region AES

        /// <summary>
        ///  AES 加密
        /// </summary>
        /// <param name="data">明文（待加密）</param>
        /// <param name="key">密文</param>
        /// <returns></returns>
        public static byte[] AesEncrypt(byte[] data, string key)
        {
            if (key == null || key.Length != 16)
            {
                key = TrimKey(key,16);
            }

            var rm = new RijndaelManaged
            {
                Key = Encoding.UTF8.GetBytes(key),
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };

            var cTransform = rm.CreateEncryptor();
            var resultArray = cTransform.TransformFinalBlock(data, 0, data.Length);
            return resultArray;
        }

        public static string AesEncrypt(string data, string key)
        {
            return BytesToString(AesEncrypt(StringToByte(data), key));
        }


        /// <summary>
        ///  AES 解密
        /// </summary>
        /// <param name="data">明文（待解密）</param>
        /// <param name="key">密文</param>
        /// <returns></returns>
        public static byte[] AesDecrypt(byte[] data, string key)
        {
            if (key == null || key.Length != 16)
            {
                key = TrimKey(key,16);
            }

            var rm = new RijndaelManaged
            {
                Key = Encoding.UTF8.GetBytes(key),
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };

            var cTransform = rm.CreateDecryptor();
            var resultArray = cTransform.TransformFinalBlock(data, 0, data.Length);
            return resultArray;
        }

        public static string AesDecrypt(string data, string key)
        {
            return BytesToString(AesDecrypt(StringToByte(data), key));
        }

        #endregion
    }
}