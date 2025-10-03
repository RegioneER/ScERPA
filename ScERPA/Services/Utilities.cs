using ScERPA.Data;
using ScERPA.Services.Interfaces;
using ScERPA.ViewModels;
using Ganss.Xss;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace ScERPA.Services
{
    public class Utilities : IUtilities
    {

        private readonly string _password;

        public Utilities(IConfiguration configuration)
        {

            _password = configuration.GetValue<string>("Password") ?? "";

        }

        public string Decrypt(string encryptedText)
        {
            string risultato;

            try
            {
                risultato = DecryptWithPassword(encryptedText, _password);
            } catch
            {
                risultato = "";
            }

            return risultato;
        }

        public string Encrypt(string plainText)
        {
            return EncryptWithPassword(plainText, _password);
        }

        public string GetRemoteIPV4(string? remoteIP)
        {
            string strIp;

            if (remoteIP != null)
            {
                var ip = IPAddress.Parse(remoteIP);
   

                if (ip.IsIPv4MappedToIPv6)
                {
                    strIp = remoteIP.Replace("::ffff:", "");

                }
                else
                {
                    strIp = ip.MapToIPv4().ToString();
                }
            } else
            {
                strIp = "";
            }


            return strIp;
        }


        public string EncryptWithPassword(string value, string password)
        {
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] aesKey = SHA256.Create().ComputeHash(passwordBytes);
            byte[] aesIV = MD5.Create().ComputeHash(passwordBytes);
            using (var aesAlgorithm = Aes.Create())
            {
                using (var encryptor = aesAlgorithm.CreateEncryptor(aesKey, aesIV))
                using (var memoryStream = new MemoryStream())
                using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    using (var streamWriter = new StreamWriter(cryptoStream, Encoding.UTF8))
                    {
                        streamWriter.Write(value);
                    }

                    return Convert.ToBase64String(memoryStream.ToArray());
                }
            }
        }

        public string DecryptWithPassword(string value, string password)
        {
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] aesKey = SHA256.Create().ComputeHash(passwordBytes);
            byte[] aesIV = MD5.Create().ComputeHash(passwordBytes);
            byte[] encryptedBytes = Convert.FromBase64String(value);
            using (var aesAlgorithm = Aes.Create())
            using (var decryptor = aesAlgorithm.CreateDecryptor(aesKey, aesIV))
            using (var memoryStream = new MemoryStream(encryptedBytes))
            using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
            using (var streamReader = new StreamReader(cryptoStream, Encoding.UTF8))
            {
                return streamReader.ReadToEnd();
            }
        }

        public byte[] GetHash(string password, byte[] salt)
        {
            // derive a 256-bit subkey (use HMACSHA256 with 10,000 iterations)
            byte[] hashed = KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8);
            return hashed;
        }

        public string SanitizeAsPlainText(string textToSanitize)
        {
            var sanitizer = new HtmlSanitizer();
            sanitizer.AllowedTags.Clear();
            sanitizer.AllowedClasses.Clear();
            sanitizer.AllowedAttributes.Clear();
            sanitizer.AllowedSchemes.Clear();
            sanitizer.AllowedAtRules.Clear();


            return sanitizer.Sanitize(textToSanitize);
        }


    }




}
