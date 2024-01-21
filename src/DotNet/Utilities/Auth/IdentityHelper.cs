using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Core.DotNet.Utilities.Auth;

public class IdentityHelper
{
    public static string NewIdentity(int keyLength = 32)
    {
        return GenerateRandomString(keyLength, null);
    }
    
    public static string NewIdentityInt(int keyLength = 32)
    {
        return GenerateRandomInt(keyLength, null);
    }

    public static string NewRandomSha256Hash()
    {
        return ComputeSha256Hash($"{NewIdentity(32)}{DateTime.UtcNow:yyyyMMddhhmmssfffffff}");
    }

    public static string ComputeSha256Hash(object obj)
    {
        return ComputeSha256Hash(JsonSerializer.Serialize(obj));
    }

    public static string ComputeSha256Hash(string rawData)
    {
        using var sha256Hash = SHA256.Create();
        var bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
        var builder = new StringBuilder();

        foreach (var t in bytes)
        {
            builder.Append(t.ToString("x2"));
        }

        return builder.ToString();
    }

    public static string GenerateRandomCryptographicKey(int keyLength)
    {
        using RandomNumberGenerator rng = new RNGCryptoServiceProvider();

        var tokenData = new byte[keyLength];
        rng.GetBytes(tokenData);

        var result = Convert.ToBase64String(tokenData);

        return result;
    }

    public static string GenerateRandomString(int length, string includedChars)
    {
        var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        return GenerateRandomString(length, chars, includedChars);
    }
    
    public static string GenerateRandomInt(int length, string includedChars)
    {
        var chars = "0123456789";

        return GenerateRandomString(length, chars, includedChars);
    }


    public static string GenerateRandomString(int length, string chars, string includedChars)
    {
        chars += includedChars;

        var stringChars = new char[length];
        var random = new Random();

        for (var i = 0; i < stringChars.Length; i++)
        {
            stringChars[i] = chars[random.Next(chars.Length)];
        }

        return new string(stringChars);
    }

    public static string AES_Encrypt(string Input, string Key, string IV)
    {
        var managed = new RijndaelManaged
        {
            KeySize = 0x100,
            BlockSize = 0x80,
            Padding = PaddingMode.PKCS7,
            Mode = CipherMode.CBC,
            Key = FromHex(Key),
            IV = FromHex(IV)
        };
        var transform = managed.CreateEncryptor(managed.Key, managed.IV);
        byte[] inArray = null;
        using (var mstream = new MemoryStream())
        {
            using (var cstream = new CryptoStream(mstream, transform, CryptoStreamMode.Write))
            {
                byte[] bytes = Encoding.UTF8.GetBytes(Input);
                cstream.Write(bytes, 0, bytes.Length);
            }
            inArray = mstream.ToArray();
        }
        return Convert.ToBase64String(inArray);
    }

    public static string AES_Decrypt(string Input, string Key, string IV)
    {
        var managed = new RijndaelManaged
        {
            KeySize = 0x100,
            BlockSize = 0x80,
            Padding = PaddingMode.PKCS7,
            Mode = CipherMode.CBC,
            Key = FromHex(Key),
            IV = FromHex(IV)
        };
        var transform = managed.CreateDecryptor(managed.Key, managed.IV);
        byte[] bytes = null;
        using (var mstream = new MemoryStream())
        {
            using (var cstream = new CryptoStream(mstream, transform, CryptoStreamMode.Write))
            {
                byte[] buffer = Convert.FromBase64String(Input);
                cstream.Write(buffer, 0, buffer.Length);
            }
            bytes = mstream.ToArray();
        }
        return Encoding.UTF8.GetString(bytes);
    }

    private static byte[] FromHex(string hex)
    {
        hex = hex.Replace("-", "");
        byte[] buffer = new byte[hex.Length / 2];
        for (int i = 0; i < buffer.Length; i++)
        {
            buffer[i] = Convert.ToByte(hex.Substring(i * 2, 2), 0x10);
        }
        return buffer;
    }
    
    public static string AES_V2_Encrypt(string plainText, string key, string iv)
    {
        byte[] keyBytes = Encoding.UTF8.GetBytes(key);
        byte[] ivBytes = Encoding.UTF8.GetBytes(iv);
        
        using (Aes aes = Aes.Create())
        {
            aes.KeySize = 256;
            aes.Key = keyBytes;
            aes.IV = ivBytes;
            aes.Mode = CipherMode.CFB; // You can change the mode if needed, this is just an example

            var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            byte[] encryptedBytes;

            using (var ms = new MemoryStream())
            {
                using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    byte[] inputBytes = Encoding.UTF8.GetBytes(plainText);
                    cs.Write(inputBytes, 0, inputBytes.Length);
                }

                encryptedBytes = ms.ToArray();
            }

            return Convert.ToBase64String(encryptedBytes);
        }
    }
    
    public static string AES_V2_Decrypt(string encryptedText, string key, string iv)
    {
        byte[] keyBytes = Encoding.UTF8.GetBytes(key);
        byte[] ivBytes = Encoding.UTF8.GetBytes(iv);

        using (Aes aes = Aes.Create())
        {
            aes.KeySize = 256;
            aes.Key = keyBytes;
            aes.IV = ivBytes;
            aes.Mode = CipherMode.CFB; // You need to use the same mode that was used for encryption

            var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            byte[] encryptedBytes = Convert.FromBase64String(encryptedText);
            byte[] decryptedBytes;

            using (var ms = new MemoryStream(encryptedBytes))
            {
                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                {
                    using (var outputMs = new MemoryStream())
                    {
                        cs.CopyTo(outputMs);
                        decryptedBytes = outputMs.ToArray();
                    }
                }
            }

            return Encoding.UTF8.GetString(decryptedBytes);
        }
    }
}