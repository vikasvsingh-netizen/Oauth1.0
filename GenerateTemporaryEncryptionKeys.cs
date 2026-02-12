namespace Oauth_1a_Demo
{
    public static class GenerateTemporaryEncryptionKeys
    {
        private static IConfiguration _config;
        public static void InitializeConfiguration(IConfiguration config)
        {
            _config = config;
        }
        public static void M1(string ConsumerSecret, string TokenSecret)
        {
            //var key = "MySuperStrongEncryptionKey123";
            var encryptionKey = _config["OAuth:DecryptionKey"];
            var encryptedConsumerSecret = EncryptionHelper.Encrypt(ConsumerSecret, encryptionKey);
            var encryptedTokenSecret = EncryptionHelper.Encrypt(TokenSecret, encryptionKey);

            Console.WriteLine($"Encrypted Consumer Secret: {encryptedConsumerSecret}");
            Console.WriteLine($"Encrypted Token Secret: {encryptedTokenSecret}");
        }
    }
}
