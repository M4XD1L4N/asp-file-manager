using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

class Program
{
    static void Main()
    {
        // Generate a random AES key and initialization vector (IV)
        byte[] aesKey = GenerateRandomBytes(32); // 256-bit key size (32 bytes)
        byte[] iv = GenerateRandomBytes(16); // 128-bit IV size (16 bytes)

        // Encrypt data
        string plainText = "Hello, AES!FFD8FFE000104A46494600010100000100010000FFDB00430001010101010101010101010101020203020202020204030302030504050505040404050607060505070604040609060708080808080506090A09080A07080808FFDB00430101010102020204020204080504050808080808080808080808080808080808080808080808080808080808080808080808080808080808080808080808080808FFC000110801C6032703012200021101031101FFC4001F0000010501010101010100000000000000000102030405060708090A0BFFC400B5100002010303020403050504040000017D01020300041105122131410613516107227114328191A10823";
        byte[] encryptedData = EncryptStringToBytes_Aes(plainText, aesKey, iv);

        // Decrypt data
        string decryptedText = DecryptStringFromBytes_Aes(encryptedData, aesKey, iv);

        // Display results
        Console.WriteLine("Original Text: " + plainText);
        Console.WriteLine("Encrypted Text: " + Convert.ToBase64String(encryptedData));
        Console.WriteLine("Decrypted Text: " + decryptedText);
        Console.ReadLine();
    }

    static byte[] GenerateRandomBytes(int length)
    {
        using (var rng = new RNGCryptoServiceProvider())
        {
            byte[] randomBytes = new byte[length];
            rng.GetBytes(randomBytes);
            return randomBytes;
        }
    }

    static byte[] EncryptStringToBytes_Aes(string plainText, byte[] key, byte[] iv)
    {
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = key;
            aesAlg.IV = iv;

            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);
                    }
                }
                return msEncrypt.ToArray();
            }
        }
    }

    static string DecryptStringFromBytes_Aes(byte[] cipherText, byte[] key, byte[] iv)
    {
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = key;
            aesAlg.IV = iv;

            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using (MemoryStream msDecrypt = new MemoryStream(cipherText))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        return srDecrypt.ReadToEnd();
                    }
                }
            }
        }
    }
}
