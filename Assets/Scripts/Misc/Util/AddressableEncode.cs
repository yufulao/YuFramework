// ******************************************************************
//@file         AddressableEncode.cs
//@brief        Addressable.cn的自定义加密类
//@author       yufulao, yufulao@qq.com
//@createTime   2024.05.18 01:33:34
// ******************************************************************
using System.IO;
using System.Security.Cryptography;
using Yu;

namespace UnityEngine.ResourceManagement.ResourceProviders
{
    /// <summary>
    /// 提供给Addressable.cn的自定义加密方式
    /// </summary>
    public class AddressableEncode : IDataConverter
    {
        private static byte[] Key => UtilsForEncode.StringToByte(UtilsForEncode.TrimKey("yufulao@qq.com", 16)); //修改此处密钥,需要16字节

        private SymmetricAlgorithm _mAlgorithm;

        private SymmetricAlgorithm Algorithm
        {
            get
            {
                if (_mAlgorithm != null) return _mAlgorithm;
                _mAlgorithm = new AesManaged {Padding = PaddingMode.Zeros};
                var initVector = new byte[_mAlgorithm.BlockSize / 8];
                for (var i = 0; i < initVector.Length; i++)
                    initVector[i] = (byte) i;
                _mAlgorithm.IV = initVector;
                _mAlgorithm.Key = Key;
                _mAlgorithm.Mode = CipherMode.ECB;

                return _mAlgorithm;
            }
        }

        public Stream CreateReadStream(Stream input, string id)
        {
            return new CryptoStream(input,
                Algorithm.CreateDecryptor(Algorithm.Key, Algorithm.IV),
                CryptoStreamMode.Read);
        }

        public Stream CreateWriteStream(Stream input, string id)
        {
            return new CryptoStream(input,
                Algorithm.CreateEncryptor(Algorithm.Key, Algorithm.IV),
                CryptoStreamMode.Write);
        }
    }
}