using System;
using System.Security.Cryptography;
using System.IO;


namespace PeerWebApp2
{
    public class Cryptos
    {
        private byte[] key,IV;

        public Cryptos(byte[] InputKey, byte[] IVect)
        {
            key = InputKey;
            IV = IVect;
        }


        public byte[] EncryptStringToBytes_Aes(string InputMessage)
        {
            byte[] encrypted;

            try
            {
                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = key;
                    aesAlg.IV = IV;
                    aesAlg.Padding = PaddingMode.Zeros;

                    // Create an encryptor to perform the stream transform.
                    ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                    // Create the streams used for encryption.
                    using (MemoryStream msEncrypt = new MemoryStream())
                    {
                        using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        {
                            using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                            {
                                //Write all data to the stream.
                                swEncrypt.Write(InputMessage);
                            }
                            encrypted = msEncrypt.ToArray();
                        }
                    }
                }

                return encrypted;
            }
            catch(Exception exp)
            {
                Console.WriteLine(" ШИфрован = "+exp);
                return null;
            }
        }

        public string DecryptStringFromBytes_Aes(byte[] cipherText)
        {
            string plaintext ="";

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = IV;
                aesAlg.Padding = PaddingMode.Zeros;

                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
            
            return plaintext;    
        }
    }
}

