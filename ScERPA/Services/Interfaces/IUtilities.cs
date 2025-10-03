using System.Net;

namespace ScERPA.Services.Interfaces
{
    public interface IUtilities
    {
        public string GetRemoteIPV4(string? remoteIP);

        public string Encrypt(string plainText);

        public string Decrypt(string encryptedText);

        public string SanitizeAsPlainText(string textToSanitize);

    

    }


}
