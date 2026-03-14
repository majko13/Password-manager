using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace Password_manager
{
    public static class SecureEncryptor
    {
        public static byte[] DeriveKeyFromPassword(string password, byte[] salt, int keyBytes = 32)
        {
            using (var pbkdf2 = new Rfc2898DeriveBytes(
                password,
                salt,
                100000,
                HashAlgorithmName.SHA256))
            {
                return pbkdf2.GetBytes(keyBytes);
            }
        }

        public static byte[] GenerateRandomIV()
        {
            byte[] iv = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(iv);
            }
            return iv;
        }

        public static byte[] Encrypt(string plainText, byte[] key, byte[] iv)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;
                aes.Padding = PaddingMode.PKCS7;

                using (MemoryStream ms = new MemoryStream())
                using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                using (StreamWriter sw = new StreamWriter(cs))
                {
                    sw.Write(plainText);
                    sw.Flush();
                    cs.FlushFinalBlock();
                    return ms.ToArray();
                }
            }
        }

        public static string Decrypt(byte[] cipherText, byte[] key, byte[] iv)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;
                aes.Padding = PaddingMode.PKCS7;

                using (MemoryStream ms = new MemoryStream(cipherText))
                using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read))
                using (StreamReader sr = new StreamReader(cs))
                {
                    return sr.ReadToEnd();
                }
            }
        }
        public static string DecryptPasswordWithMasterKey(byte[] encryptedBytes, byte[] iv, byte[] userSalt)
        {
            string masterPassword = null;
            try
            {
                masterPassword = SecurePasswordManager.GetMasterPasswordAsString();
                if (string.IsNullOrEmpty(masterPassword) || userSalt == null)
                {
                    return "***NOT LOGGED IN***";
                }

                if (iv == null || iv.Length == 0)
                {
                    return "***INVALID IV - OLD RECORD***";
                }

                byte[] key = SecureEncryptor.DeriveKeyFromPassword(masterPassword, userSalt);

                return SecureEncryptor.Decrypt(encryptedBytes, key, iv);
            }
            catch (CryptographicException)
            {
                return "***WRONG KEY***";

            }
            finally
            {
                if (masterPassword != null)
                {
                    SecurePasswordManager.ClearString(ref masterPassword);
                }
            }

        }
    }
}