using System.Security.Cryptography;
using System.Text;

namespace Oauth_1a_Demo
{
    public class EncryptionHelper
    {
        public static string Encrypt(string plainText, string base64Key)
        {
            using (Aes aesAlg = Aes.Create())
            {
                // Decode the Base64 key to bytes
                byte[] key = Convert.FromBase64String(base64Key);

                // Pad the key to 32 bytes if it's not the right size
                if (key.Length < 32)
                {
                    byte[] paddedKey = new byte[32];
                    Array.Copy(key, paddedKey, key.Length); // Copy the existing key into the padded key array
                                                            // You can append padding as zero bytes (or another value of your choice)
                                                            // This just appends a zero byte to make it 32 bytes long.
                                                            // If you want to use other padding logic, you can adjust it here.
                    paddedKey[key.Length] = 0x00;
                    key = paddedKey;
                }

                // Debugging: Check the key size after padding
                Console.WriteLine("Padded key size: " + key.Length); // Should print 32

                aesAlg.Key = key;
                aesAlg.IV = new byte[16]; // Zero IV for simplicity (can be randomized for better security)

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);
                    }
                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }

        // Decrypt a ciphertext string using AES with the provided key (Base64 encoded)
        public static string Decrypt(string cipherText, string base64Key)
        {
            using (Aes aesAlg = Aes.Create())
            {
                // Decode the Base64 key to bytes
                byte[] key = Convert.FromBase64String(base64Key);

                // Pad the key to 32 bytes if it's not the right size
                if (key.Length < 32)
                {
                    byte[] paddedKey = new byte[32];
                    Array.Copy(key, paddedKey, key.Length); // Copy the existing key into the padded key array
                                                            // Append a zero byte (or another padding strategy if needed)
                    paddedKey[key.Length] = 0x00;
                    key = paddedKey;
                }

                // Debugging: Check the key size after padding
                Console.WriteLine("Padded key size: " + key.Length); // Should print 32

                aesAlg.Key = key;
                aesAlg.IV = new byte[16]; // Zero IV for simplicity

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText)))
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                {
                    return srDecrypt.ReadToEnd();
                }
            }
        }

        public static byte[] GetValidKey(string key)
        {
            // AES key must be 16, 24, or 32 bytes long
            if (key.Length < 16)
                throw new CryptographicException("Key length is too short. AES requires at least 16 characters.");

            // Convert the key to bytes
            var keyBytes = Encoding.UTF8.GetBytes(key);

            // If the key is 128-bit (16 bytes) or 192-bit (24 bytes), use as is
            if (keyBytes.Length == 16)
            {
                return keyBytes; // 128-bit key
            }
            else if (keyBytes.Length == 24)
            {
                return keyBytes; // 192-bit key
            }
            else
            {
                // If the key is longer than 32 bytes (256-bit), truncate to 32 bytes
                if (keyBytes.Length > 32)
                {
                    Array.Resize(ref keyBytes, 32); // 256-bit key
                }
                return keyBytes;
            }
        }
    }
}
