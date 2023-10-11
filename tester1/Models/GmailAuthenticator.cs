using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Services;
using System.IO;

namespace tester1.Models
{
    public class GmailAuthenticator
    {
        private readonly string _clientSecretFilePath;
        private readonly string[] _scopes;
        private readonly string _applicationName;

        public GmailAuthenticator(string clientSecretFilePath, string[] scopes, string applicationName)
        {
            _clientSecretFilePath = clientSecretFilePath;
            _scopes = scopes;
            _applicationName = applicationName;
        }

        public UserCredential GetUserCredential(string user)
        {
            using (var stream = new FileStream(_clientSecretFilePath, FileMode.Open, FileAccess.Read))
            {
                string credPath = $"{user}_token.json";
                return GoogleWebAuthorizationBroker.AuthorizeAsync(GoogleClientSecrets.Load(stream).Secrets,
                    _scopes,
                    user,
                    System.Threading.CancellationToken.None,
                    new Google.Apis.Util.Store.FileDataStore(credPath, true)).Result;
            }
        }

        public GmailService GetGmailService(UserCredential credential)
        {
            return new GmailService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = _applicationName
            });
        }
    }
}
