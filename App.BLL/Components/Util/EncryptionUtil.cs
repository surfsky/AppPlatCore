using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace App
{
    /// <summary>
    /// �����ַ�����
    /// </summary>
    public class EncryptionUtil
    {

        /// <summary>
        /// �����ַ�����MD5��ϣֵ
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns>�ַ���MD5��ϣֵ��ʮ�������ַ���</returns>
        public static string StringToMD5Hash(string inputString)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] encryptedBytes = md5.ComputeHash(Encoding.ASCII.GetBytes(inputString));
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < encryptedBytes.Length; i++)
            {
                sb.AppendFormat("{0:x2}", encryptedBytes[i]);
            }
            return sb.ToString();
        }
    }
}
