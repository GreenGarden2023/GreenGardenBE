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
                SecretClientOptions options = new()
                {
                    Retry =
                    {
                        Delay= TimeSpan.FromSeconds(2),
                        MaxDelay = TimeSpan.FromSeconds(16),
                        MaxRetries = 5,
                        Mode = RetryMode.Exponential
                    }
                };
                SecretClient client = new(new Uri(URI), new DefaultAzureCredential(), options);

                KeyVaultSecret secret = client.GetSecret("ConnectionString");

                conn = secret.Value;
            }
            catch (Exception ex)
            {
                _ = ex.ToString();
            }

            return conn;
        }
        public static string GetTokenSecret()
        {
            string tokenSecret = "";
            try
            {
                SecretClientOptions options = new()
                {
                    Retry =
                    {
                        Delay= TimeSpan.FromSeconds(2),
                        MaxDelay = TimeSpan.FromSeconds(16),
                        MaxRetries = 5,
                        Mode = RetryMode.Exponential
                    }
                };
                SecretClient client = new(new Uri(URI), new DefaultAzureCredential(), options);

                KeyVaultSecret secret = client.GetSecret("TokenSecret");

                tokenSecret = secret.Value;
                return tokenSecret;
            }
            catch (Exception ex)
            {
                _ = ex.ToString();
            }
            return tokenSecret;

        }
        public static string GetIMGConn()
        {
            string tokenSecret = null;
            try
            {
                SecretClientOptions options = new()
                {
                    Retry =
                    {
                        Delay= TimeSpan.FromSeconds(2),
                        MaxDelay = TimeSpan.FromSeconds(16),
                        MaxRetries = 5,
                        Mode = RetryMode.Exponential
                    }
                };
                SecretClient client = new(new Uri(URI), new DefaultAzureCredential(), options);

                KeyVaultSecret secret = client.GetSecret("ImgConnection");

                tokenSecret = secret.Value;
            }
            catch (Exception ex)
            {
                _ = ex.ToString();
            }

            return tokenSecret;
        }
        public static string GetStorageKey()
        {
            string tokenSecret = null;
            try
            {
                SecretClientOptions options = new()
                {
                    Retry =
                    {
                        Delay= TimeSpan.FromSeconds(2),
                        MaxDelay = TimeSpan.FromSeconds(16),
                        MaxRetries = 5,
                        Mode = RetryMode.Exponential
                    }
                };
                SecretClient client = new(new Uri(URI), new DefaultAzureCredential(), options);

                KeyVaultSecret secret = client.GetSecret("StorageKey");

                tokenSecret = secret.Value;
            }
            catch (Exception ex)
            {
                _ = ex.ToString();
            }

            return tokenSecret;
        }
        public static List<string> GetPaymentSecrets()
        {
            List<string> secrets = new();
            try
            {
                SecretClientOptions options = new()
                {
                    Retry =
                    {
                        Delay= TimeSpan.FromSeconds(2),
                        MaxDelay = TimeSpan.FromSeconds(16),
                        MaxRetries = 5,
                        Mode = RetryMode.Exponential
                    }
                };
                SecretClient client = new(new Uri("https://ggkeyvault2023.vault.azure.net/"), new DefaultAzureCredential(), options);

                KeyVaultSecret partner = client.GetSecret("PartnerCode");
                string partnerCode = partner.Value;
                KeyVaultSecret access = client.GetSecret("AccessKey");
                string accessKey = access.Value;
                KeyVaultSecret secret = client.GetSecret("Serectkey");
                string secretKey = secret.Value;
                secrets.Add(partnerCode);
                secrets.Add(accessKey);
                secrets.Add(secretKey);
            }
            catch (Exception ex)
            {
                _ = ex.ToString();
            }

            return secrets;
        }
        public static EmailSecrets GetEmailCred()
        {
            EmailSecrets emailSecrets = new EmailSecrets();
            try
            {
                SecretClientOptions options = new()
                {
                    Retry =
                    {
                        Delay= TimeSpan.FromSeconds(2),
                        MaxDelay = TimeSpan.FromSeconds(16),
                        MaxRetries = 5,
                        Mode = RetryMode.Exponential
                    }
                };
                SecretClient client = new(new Uri(URI), new DefaultAzureCredential(), options);
                KeyVaultSecret address = client.GetSecret("EmailAddress");
                KeyVaultSecret password = client.GetSecret("EmailPassword");

                emailSecrets.EmailAddress = address.Value;
                emailSecrets.EmailPassword = password.Value;

            }
            catch (Exception ex)
            {
                _ = ex.ToString();
            }

            return emailSecrets;
        }
    }
    public  class EmailSecrets
    {
        public string EmailAddress { get; set; }
        public string EmailPassword { get; set; }
    }

}

