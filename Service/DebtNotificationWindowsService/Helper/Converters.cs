using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DebtNotificationWindowsService
{
    public class Converter
    {
        public static int BoolToInt(bool expr)
        {
            if (expr)
                return 1;
            else
                return 0;
        }

        public static byte[] FileToByteArray(string filepath)
        {
            FileStream fs = new FileStream(filepath, FileMode.Open, FileAccess.Read);
            byte[] data = new byte[fs.Length];
            fs.Read(data, 0, System.Convert.ToInt32(fs.Length));
            fs.Close();
            return data;
        }

        public static string SingleToDoubleDigit(string s)
        {
            if (s.Length == 1)
                return s.PadLeft(2, '0');
            else
                return s;
        }

        public static string BooleanGenderToLitral(string s)
        {
            if (s.ToLower() == "true" || s == "1")
                return "male";
            else
                return "female";
        }


        public static bool IsValidType(object o, Type t)
        {
            try
            {
                System.Convert.ChangeType(o, t);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static List<int> CommaSeperatedToList(string s)
        {
            //foreach (string item in items)
            //    result.Add(Convert.ToInt32(item));
            if (string.IsNullOrWhiteSpace(s)) return new List<int>();

            List<int> result = new List<int>();
            string[] items = s.Split(',');
            foreach (string item in items)
            {
                int outRes;
                int? intItem = string.IsNullOrEmpty(item) ? null : int.TryParse(item, out outRes) ? (int?)int.Parse(item) : null;
                if (intItem.HasValue)
                    result.Add(intItem.Value);
            }
            return result;
        }


        #region Number To Persian Digits
        public static string ToPersian(long number)
        {
            string digits = "۰۱۲۳۴۵۶۷۸۹";
            string result = string.Empty;

            foreach (char c in number.ToString())
            {
                result += digits[Convert.ToInt32(c.ToString())];
            }

            return result;
        }

        public static string ToPersianDigits(string number)
        {
            string persianDigits = "۰۱۲۳۴۵۶۷۸۹";
            string latinDigits = "0123456789";

            string result = string.Empty;

            foreach (char c in number)
            {
                if (latinDigits.IndexOf(c) >= 0)
                    result += persianDigits[Convert.ToInt32(c.ToString())];
                else
                    result += c;
            }

            return result;
        }

        public static string ToLatinDigits(string number)
        {
            string persianDigits = "۰۱۲۳۴۵۶۷۸۹";
            string latinDigits = "0123456789";
            string result = string.Empty;

            foreach (char c in number)
            {
                if (persianDigits.IndexOf(c) >= 0)
                    result += latinDigits[persianDigits.IndexOf(c)];
                else
                    result += c;
            }

            return result;
        }
        #endregion
    }

}
