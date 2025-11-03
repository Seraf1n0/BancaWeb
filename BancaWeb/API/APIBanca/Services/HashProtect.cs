using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
// https://learn.microsoft.com/es-es/dotnet/api/system.security.cryptography.aes?view=net-8.0

namespace APIBanca.Services
{
    
    public class EncryptionProtect
    {
        private readonly byte[] _key;
        private readonly byte[] _iv;

        public EncryptionProtect(IConfiguration configuration)
        {
            var keyString = configuration["Encryption:Key"]; 
            var ivString = configuration["Encryption:IV"];   

            _key = Encoding.UTF8.GetBytes(keyString);
            _iv = Encoding.UTF8.GetBytes(ivString);
        }

        public string Encrypt(string archivoSecreto)
        {
            try
            {
                using (Aes aes = Aes.Create())
                {
                    aes.Key = _key; 
                    aes.IV = _iv;   
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;

                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        using (CryptoStream cryptoStream = new CryptoStream(
                            memoryStream,
                            aes.CreateEncryptor(),
                            CryptoStreamMode.Write))
                        {
                            using (StreamWriter encryptWriter = new StreamWriter(cryptoStream))
                            {
                                encryptWriter.Write(archivoSecreto);
                            }
                        }

                        return Convert.ToBase64String(memoryStream.ToArray());
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al encriptar: {ex.Message}", ex);
            }
        }

        public string Decrypt(string archivoSecreto)
        {
            try
            {
                byte[] encryptedBytes = Convert.FromBase64String(archivoSecreto);

                using (Aes aes = Aes.Create())
                {
                    aes.Key = _key;
                    aes.IV = _iv;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;

                    using (MemoryStream memoryStream = new MemoryStream(encryptedBytes))
                    {
                        using (CryptoStream cryptoStream = new CryptoStream(
                            memoryStream,
                            aes.CreateDecryptor(),
                            CryptoStreamMode.Read))
                        {
                            using (StreamReader decryptReader = new StreamReader(cryptoStream))
                            {
                                return decryptReader.ReadToEnd();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al desencriptar: {ex.Message}", ex);
            }
        }
    }
}