using UMSV.Schema;
using System.Linq;
using System.Data.Linq;
using Enterprise;
using System.Collections;
using System.Collections.Generic;
using System;

namespace UMSV
{
    public partial class Informing
    {
        private Schedule schedule;
        public Schedule Schedule
        {
            get
            {
                if (schedule == null)
                {
                    if (CallTime == null)
                        schedule = new Schedule();
                    else
                        schedule = ScheduleUtility.DeserializeInforming<Schedule>(CallTime.ToString());
                }
                return schedule;
            }
        }
    }

    public partial class Voice
    {
        public static byte[] GetByName(string name)
        {
            UmsDataContext dc = new UmsDataContext();
            var voice = dc.Voices.FirstOrDefault(f => f.Name == name && f.VoiceGroup == 2);

            if (voice != null && voice.Data != null)
                return voice.Data.ToArray();
            else
            {
                if (System.Text.RegularExpressions.Regex.IsMatch(name, @"^\d+$"))
                    return GetNumberVoice(name);
                
                Logger.WriteWarning("Voice Name '{0}' not found.", name);
                return null;
            }
        }

        public static byte[] GetNumberVoice(string number)
        {
            List<string> digitVoices = new List<string>();
            ParseDigits(number.ToString(), digitVoices);

            List<byte[]> buffers = new List<byte[]>();
            int bufferSize = 0;
            foreach (object digitVoice in digitVoices)
            {
                byte[] buffer = GetByName(digitVoice.ToString());
                bufferSize += buffer.Length;
                if (buffer != null)
                    buffers.Add(buffer);
            }

            byte[] result = new byte[bufferSize];
            int resultIndex = 0;
            for (int i = 0; i < buffers.Count; i++)
            {
                Array.Copy(buffers[i], 0, result, resultIndex, buffers[i].Length);
                resultIndex += buffers[i].Length - 1;
            }

            return result;
        }
        private static void ParseDigits(string number, List<string> integerValues)
        {

            if (number.Length >= 3)
                Pars3Digit(number, integerValues);
            else
                Pars2Digit(number, integerValues);
        }

        private static void Pars3Digit(string number, List<string> intValues)
        {
            if (string.IsNullOrEmpty(number) || number.Length < 3 || double.Parse(number) == 0)
                return;

            int stringLenth = number.Length;
            int splitNo = stringLenth / 3;
            int firstIndex = stringLenth % 3;


            if (firstIndex != 0)
            {
                string no = number.Substring(0, firstIndex);
                Pars2Digit(no, intValues);
                string baseNo = "1";
                int i = 0;
                for (; i < stringLenth - firstIndex; i++)
                    baseNo += "0";

                if (double.Parse(number) == double.Parse(no) * double.Parse(baseNo))
                {
                    if (i > 0)
                        intValues.Add(baseNo);
                }
                else
                {
                    if (i > 0)
                        intValues.Add(baseNo + "o");
                }
                Pars3Digit(number.Substring(firstIndex, number.Length - firstIndex), intValues);
            }
            else
            {
                string no = number.Substring(0, 3);
                Pars2Digit(no, intValues);
                string baseNo = "1";
                int i = 0;
                for (; i < stringLenth - 3; i++)
                    baseNo += "0";
                if (double.Parse(number) == double.Parse(no) * double.Parse(baseNo))
                {
                    if (i > 0)
                        intValues.Add(baseNo);
                }
                else
                {
                    if (i > 0)
                        intValues.Add(baseNo + "o");
                }
                Pars3Digit(number.Substring(3, number.Length - 3), intValues);
            }

        }

        private static void Pars2Digit(string no, List<string> intValues)
        {
            if (no.Length == 0)
                return;
            int val = int.Parse(no);
            if (val > 100 && val < 1000)
            {
                if (val / 100 * 100 == val)
                    intValues.Add((val / 100 * 100).ToString());
                else
                    intValues.Add((val / 100 * 100).ToString() + "o");

                Pars2Digit((val % 100).ToString(), intValues);
            }
            else if (val > 20 && val < 100)
            {
                if (val / 10 * 10 == val)
                    intValues.Add((val / 10 * 10).ToString());
                else
                    intValues.Add((val / 10 * 10).ToString() + "o");

                Pars2Digit((val % 10).ToString(), intValues);
            }
            else
            {
                if (val > 0)
                    intValues.Add(val.ToString());
                return;
            }

        }
    }

    public partial class Call
    {
        public int Duration
        {
            get
            {
                return (int)DisconnectTime.Subtract(CallTime).TotalSeconds;
            }
        }
    }

    public partial class SpecialPhone
    {
    }

    public class BatchMailbox : Mailbox
    {
        public int BoxNoFrom
        {
            get;
            set;
        }

        public int BoxNoTo
        {
            get;
            set;
        }

        public bool BoxNoAsCallerID
        {
            get;
            set;
        }

        public bool RandomPassword
        {
            get;
            set;
        }
    }
}
