using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.ZMYY
{
    internal class AesHelper
    {
        /// <summary>
        ///AES 算法加密(ECB模式) 将明文加密，加密后进行Hex编码，返回密文
        /// </summary>
        /// <param name="str">明文</param>
        /// <param name="key">密钥</param>
        /// <returns>加密后Hex编码的密文</returns>
        public static string AesEncryptor(string str, string key)
        {
            try
            {
                if (string.IsNullOrEmpty(str)) return null;
                Byte[] toEncryptArray = Encoding.UTF8.GetBytes(str);

                using System.Security.Cryptography.RijndaelManaged rm = new System.Security.Cryptography.RijndaelManaged
                {
                    Key = Encoding.UTF8.GetBytes(key),
                    Mode = System.Security.Cryptography.CipherMode.CBC,
                    Padding = System.Security.Cryptography.PaddingMode.PKCS7,
                    IV = Encoding.UTF8.GetBytes("1234567890000000")
                };

                using System.Security.Cryptography.ICryptoTransform cTransform = rm.CreateEncryptor();
                Byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

                return ToHexString(resultArray);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        ///AES 算法解密(ECB模式) 将密文Hex解码后进行解密，返回明文
        /// </summary>
        /// <param name="str">密文</param>
        /// <param name="key">密钥</param>
        /// <returns>明文</returns>
        public static string AesDecryptor(string str, string key)
        {
            try
            {
                if (string.IsNullOrEmpty(str))
                    return null;
                Byte[] toEncryptArray = StrToHexByte(str);
                using System.Security.Cryptography.RijndaelManaged rm = new System.Security.Cryptography.RijndaelManaged
                {
                    Key = Encoding.UTF8.GetBytes(key),
                    Mode = System.Security.Cryptography.CipherMode.CBC,
                    Padding = System.Security.Cryptography.PaddingMode.PKCS7,
                    IV = Encoding.UTF8.GetBytes("1234567890000000")
                };
                using System.Security.Cryptography.ICryptoTransform cTransform = rm.CreateDecryptor();
                Byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

                return Encoding.UTF8.GetString(resultArray);
            }
            catch
            {
                return null;
            }
        }


        /// <summary>
        /// byte数组Hex编码
        /// </summary>
        /// <param name="bytes">需要进行编码的byte[]</param>
        /// <returns></returns>
        private static string ToHexString(byte[] bytes) // 0xae00cf => "AE00CF "
        {
            string hexString = string.Empty;
            if (bytes != null)
            {
                StringBuilder strB = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    strB.Append(bytes[i].ToString("X2"));
                }
                hexString = strB.ToString();
            }
            return hexString;
        }
        /// <summary> 
        /// 字符串进行Hex解码(Hex.decodeHex())
        /// </summary> 
        /// <param name="hexString">需要进行解码的字符串</param> 
        /// <returns></returns> 
        private static byte[] StrToHexByte(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0)
                hexString += " ";
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }
    }
}
