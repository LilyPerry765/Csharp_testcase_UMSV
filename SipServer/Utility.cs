using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Net;
using System.Security.Cryptography;
using System.Management;

namespace Folder.Voip
{
    class Utility
    {
        public static string GetFriendlyName(string name)
        {
            string fiendlyName = string.Empty;
            foreach (Char chr in name)
            {
                if (Char.IsUpper(chr))
                    fiendlyName += " " + chr;
                else
                    fiendlyName += chr;
            }

            return fiendlyName;
        }

        public static string FindHDDSerial()
        {
            ManagementObject disk = new ManagementObject("win32_logicaldisk.deviceid=\"C:\"");
            disk.Get();
            return disk["VolumeSerialNumber"].ToString();
        }

        public static string GenerateBranch()
        {
            return string.Format("z9hG4bK-{0}-ppco", Guid.NewGuid());
        }

        public static string ComputeMd5Hash(string clearString)
        {
            Func<byte[], string> ToHex = ((pass2) => BitConverter.ToString(pass2).Replace("-", "").ToLower());
            MD5CryptoServiceProvider svc = new MD5CryptoServiceProvider();
            byte[] codedStream = svc.ComputeHash(Encoding.ASCII.GetBytes(clearString));
            return ToHex(codedStream);
        }

        public static int Generate32bitRandomNumber()
        {
            Random rnd = new Random();
            var buffer = new byte[sizeof(Int32)];
            rnd.NextBytes(buffer);
            Int32 rn = BitConverter.ToInt32(buffer, 0);
            if (rn > 0)
                return rn;
            return rn * -1;
        }
    }
}