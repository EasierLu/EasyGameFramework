using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace EGFramework.Runtime.Util
{
    public static partial class Utility
    {
        public static partial class Security
        {
            #region MD5
            public static string ToHex(byte[] bytes)
            {
                return System.BitConverter.ToString(bytes).Replace("-", "");
            }

            public static string GetMD5(byte[] bytes)
            {
                var hash = MD5.Create().ComputeHash(bytes);
                return ToHex(hash);
            }

            public static string GetStreamMD5(Stream stream)
            {
                var hash = MD5.Create().ComputeHash(stream);
                return ToHex(hash);
            }
            #endregion

            #region SHA

            public static string GetSHA1(byte[] bytes)
            {
                var hash = SHA1.Create().ComputeHash(bytes);
                return ToHex(hash);
            }

            public static string GetSHA1(Stream stream)
            {
                var hash = SHA1.Create().ComputeHash(stream);
                return ToHex(hash);
            }

            public static string GetSHA256(byte[] bytes)
            {
                var hash = SHA256.Create().ComputeHash(bytes);
                return ToHex(hash);
            }

            public static string GetSHA256(Stream stream)
            {
                var hash = SHA256.Create().ComputeHash(stream);
                return ToHex(hash);
            }

            public static string GetSHA512(byte[] bytes)
            {
                var hash = SHA512.Create().ComputeHash(bytes);
                return ToHex(hash);
            }

            public static string GetSHA512(Stream stream)
            {
                var hash = SHA512.Create().ComputeHash(stream);
                return ToHex(hash);
            }
            #endregion

            #region Base64

            public static string ToBase64(byte[] bytes)
            {
                return System.Convert.ToBase64String(bytes);
            }
            public static byte[] FromBase64(string base64)
            {
                return System.Convert.FromBase64String(base64);
            }
            #endregion

            #region AES
            private static RijndaelManaged CreateRijndael(byte[] key, byte[] iv)
            { 
                RijndaelManaged rijndael = new RijndaelManaged();
                rijndael.Key = key;
                rijndael.IV = iv;
                rijndael.Mode = CipherMode.CBC;
                rijndael.Padding = PaddingMode.PKCS7;
                return rijndael;
            }

            public static byte[] AESEncryption(byte[] bytes, string key, string iv)
            {
                return AESEncryption(bytes, Encoding.UTF8.GetBytes(key), Encoding.UTF8.GetBytes(iv));
            }

            public static byte[] AESEncryption(byte[] bytes, byte[] key, byte[] iv)
            {
                RijndaelManaged rijndael = CreateRijndael(key, iv);
                ICryptoTransform transform = rijndael.CreateEncryptor();
                byte[] result = transform.TransformFinalBlock(bytes, 0, bytes.Length);
                return result;
            }

            public static byte[] AESEncryption(Stream input, byte[] key, byte[] iv)
            {
                MemoryStream output = new MemoryStream();
                AESEncryption(input, output, key, iv);
                return output.ToArray();
            }

            public static void AESEncryption(Stream input, Stream output, byte[] key, byte[] iv)
            {
                RijndaelManaged rijndael = CreateRijndael(key, iv);
                ICryptoTransform transform = rijndael.CreateEncryptor();
                CryptoStream cryptoStream = new CryptoStream(output, transform, CryptoStreamMode.Write);
                byte[] buffer = new byte[1024];
                int bytesRead;
                do
                {
                    bytesRead = input.Read(buffer, 0, buffer.Length);
                    cryptoStream.Write(buffer, 0, bytesRead);
                } while (bytesRead > 0);
                cryptoStream.FlushFinalBlock();
                cryptoStream.Close();
            }

            public static byte[] AESDecryption(byte[] bytes, string key, string iv)
            {
                return AESEncryption(bytes, Encoding.UTF8.GetBytes(key), Encoding.UTF8.GetBytes(iv));
            }

            public static byte[] AESDecryption(byte[] bytes, byte[] key, byte[] iv)
            {
                RijndaelManaged rijndael = CreateRijndael(key, iv);
                ICryptoTransform transform = rijndael.CreateDecryptor();
                byte[] result = transform.TransformFinalBlock(bytes, 0, bytes.Length);
                return result;
            }

            public static byte[] AESDecryption(Stream input, byte[] key, byte[] iv)
            {
                MemoryStream output = new MemoryStream();
                AESDecryption(input, output, key, iv);
                return output.ToArray();
            }

            public static void AESDecryption(Stream input, Stream output, byte[] key, byte[] iv)
            {
                RijndaelManaged rijndael = CreateRijndael(key, iv);
                ICryptoTransform transform = rijndael.CreateDecryptor();
                CryptoStream cryptoStream = new CryptoStream(output, transform, CryptoStreamMode.Write);
                byte[] buffer = new byte[1024];
                int bytesRead;
                do
                {
                    bytesRead = input.Read(buffer, 0, buffer.Length);
                    cryptoStream.Write(buffer, 0, bytesRead);
                } while (bytesRead > 0);
                cryptoStream.FlushFinalBlock();
                cryptoStream.Close();
            }

            #endregion

            #region RSA
            public static byte[] RSAEncryption_Public(byte[] bytes, string publicKey)
            {
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                rsa.FromXmlString(publicKey);
                byte[] result = rsa.Encrypt(bytes, false);
                return result;
            }

            public static byte[] RSADecryption_Public(byte[] bytes, string publicKey)
            {
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                rsa.FromXmlString(publicKey);
                byte[] result = rsa.Decrypt(bytes, false);
                return result;
            }
            #endregion

            #region Byte
            public static byte[] XOR(byte[] bytes, byte key)
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    bytes[i] ^= key;
                }
                return bytes;
            }
            #endregion
        }
    }
}
