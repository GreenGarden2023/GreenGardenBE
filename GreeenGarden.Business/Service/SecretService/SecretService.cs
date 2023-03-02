using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace GreeenGarden.Business.Service.SecretService
{
    public static class SecretService
    {
        public static string URI = "https://ggkeyvault2023.vault.azure.net/";
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
                var client = new SecretClient(new Uri(URI), new DefaultAzureCredential(), options);

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
            string tokenSecret = "";
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
                var client = new SecretClient(new Uri(URI), new DefaultAzureCredential(), options);

                KeyVaultSecret secret = client.GetSecret("TokenSecret");

                tokenSecret = secret.Value;
                return tokenSecret;
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            return tokenSecret;

        }
        public static string GetIMGConn()
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
                var client = new SecretClient(new Uri(URI), new DefaultAzureCredential(), options);

                KeyVaultSecret secret = client.GetSecret("ImgConnection");

                tokenSecret = secret.Value;
            }
            catch (Exception ex)
            {
                ex.ToString();
            }

            return tokenSecret;
        }
        public static List<string> GetPaymentSecrets()
        {
            List<string> secrets = new List<string>();
            string partnerCode = null;
            string accessKey = null;
            string secretKey = null;
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

                KeyVaultSecret partner = client.GetSecret("PartnerCode");
                partnerCode = partner.Value;
                KeyVaultSecret access = client.GetSecret("AccessKey");
                accessKey = access.Value;
                KeyVaultSecret secret = client.GetSecret("Serectkey");
                secretKey = secret.Value;
                secrets.Add(partnerCode);
                secrets.Add(accessKey);
                secrets.Add(secretKey);
            }
            catch (Exception ex)
            {
                ex.ToString();
            }

            return secrets;
        }
    }
}

