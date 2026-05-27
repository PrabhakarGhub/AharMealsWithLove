using System.Security.Cryptography;
using System.Text;

namespace AharMealsWithLove.Data
{
    public static class SecurityHelper
    {
        private const string Key = "AharMealsWithLove2023";

        public static string Encrypt(string plainText)
        {
            using var md5 = MD5.Create();
            byte[] keyBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(Key));

            using var des = TripleDES.Create();
            des.Key = keyBytes;
            des.Mode = CipherMode.ECB;
            des.Padding = PaddingMode.PKCS7;

            byte[] inputBytes = Encoding.UTF8.GetBytes(plainText);
            using var encryptor = des.CreateEncryptor();
            byte[] result = encryptor.TransformFinalBlock(inputBytes, 0, inputBytes.Length);
            return Convert.ToBase64String(result);
        }

        public static string Decrypt(string encryptedText)
        {
            try
            {
                using var md5 = MD5.Create();
                byte[] keyBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(Key));

                using var des = TripleDES.Create();
                des.Key = keyBytes;
                des.Mode = CipherMode.ECB;
                des.Padding = PaddingMode.PKCS7;

                byte[] inputBytes = Convert.FromBase64String(encryptedText);
                using var decryptor = des.CreateDecryptor();
                byte[] result = decryptor.TransformFinalBlock(inputBytes, 0, inputBytes.Length);
                return Encoding.UTF8.GetString(result);
            }
            catch { return ""; }
        }

        public static string GenerateOTP() => new Random().Next(10000, 99999).ToString();
    }
}
