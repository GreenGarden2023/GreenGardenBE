using System;
using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace GreeenGarden.Business.Service.SecretService
{
	public static class SecretService
	{
        public static string GetConnectionString()
        {
            string conn = null;
            try
            {
                SecretClientOptions options = new SecretClientOptions()
                {
                    Retry =
                    {
                        Delay= TimeSpan.FromSeconds(2),
                        MaxDelay = TimeSpan.FromSeconds(16),
                        MaxRetries = 5,
                        Mode = RetryMode.Exponential
                    }
                };
                var client = new SecretClient(new Uri("https://ggkeyvault2023.vault.azure.net/"), new DefaultAzureCredential(), options);

                KeyVaultSecret secret = client.GetSecret("ConnectionString");

                conn = secret.Value;
            }
            catch (Exception ex)
            {
                ex.ToString();
            }

            return conn;
        }
        public static string GetTokenSecret()
        {
            string tokenSecret = null;
            try
            {
                SecretClientOptions options = new SecretClientOptions()
                {
                    Retry =
                    {
                        Delay= TimeSpan.FromSeconds(2),
                        MaxDelay = TimeSpan.FromSeconds(16),
                        MaxRetries = 5,
                        Mode = RetryMode.Exponential
                    }
                };
                var client = new SecretClient(new Uri("https://ggkeyvault2023.vault.azure.net/"), new DefaultAzureCredential(), options);

                KeyVaultSecret secret = client.GetSecret("TokenSecret");

                tokenSecret = secret.Value;
            }
            catch (Exception ex)
            {
                ex.ToString();
            }

            return tokenSecret;
        }
    }
}

