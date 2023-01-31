using System;
using System.Security.Cryptography;
using System.IO;
using System.Text;

namespace CustomShell
{
    internal class Hashes
    {
        public Hashes() 
        {

        }

        MainController main = MainController.controller;

        public void GetMD5(string input)
        {
            if (main.IsFilePath(input))
            {
                using (MD5 md5 = MD5.Create())
                {
                    using (FileStream stream = File.OpenRead(input))
                    {
                        byte[] hash = md5.ComputeHash(stream);
                        main.AddTextToConsole(BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant());
                    }
                }
            }
            else //Hash a string
            {
                using (MD5 md5 = MD5.Create())
                {
                    main.AddTextToConsole(BitConverter.ToString(md5.ComputeHash(Encoding.UTF8.GetBytes(input))).Replace("-", ""));
                }
            }
        }

        public void GetSHA256(string input)
        {
            if (main.IsFilePath(input))
            {
                using (SHA256 sha256 = SHA256.Create())
                {
                    using (FileStream stream = File.OpenRead(input))
                    {
                        byte[] hash = sha256.ComputeHash(stream);
                        main.AddTextToConsole(BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant());
                    }
                }
            }
            else
            {
                using (SHA256 sha256 = SHA256.Create())
                {
                    main.AddTextToConsole(BitConverter.ToString(sha256.ComputeHash(Encoding.UTF8.GetBytes(input))).Replace("-", ""));
                }
            }
        }

        public void GetSHA512(string input)
        {
            if (main.IsFilePath(input))
            {
                using (SHA512 sha512 = SHA512.Create())
                {
                    using (FileStream stream = File.OpenRead(input))
                    {
                        byte[] hash = sha512.ComputeHash(stream);
                        main.AddTextToConsole(BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant());
                    }
                }
            }
            else
            {
                using (SHA512 sha512 = SHA512.Create())
                {
                    main.AddTextToConsole(BitConverter.ToString(sha512.ComputeHash(Encoding.UTF8.GetBytes(input))).Replace("-", ""));
                }
            }
        }
    }
}
