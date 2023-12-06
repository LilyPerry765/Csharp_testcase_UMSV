using System.Diagnostics;
using System.Data;
using System;
using Enterprise;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Data.SqlClient;
using UMSV.Schema;
using Folder;

namespace UMSV.Engine
{
    public class VoiceStream
    {
        #region " Private fields "
        internal MemoryStream stream = new MemoryStream();
        #endregion

        #region " Private Methods "
        private void ParseDigits(string number, ref ArrayList integerValues)
        {

            if (number.Length >= 3)
                Pars3Digit(number, ref integerValues);
            else
                Pars2Digit(number, ref integerValues);
        }

        private void Pars3Digit(string number, ref  ArrayList intValues)
        {
            if (string.IsNullOrEmpty(number) || number.Length < 3 || double.Parse(number) == 0)
                return;

            int stringLenth = number.Length;
            int splitNo = stringLenth / 3;
            int firstIndex = stringLenth % 3;


            if (firstIndex != 0)
            {
                string no = number.Substring(0, firstIndex);
                Pars2Digit(no, ref intValues);
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
                Pars3Digit(number.Substring(firstIndex, number.Length - firstIndex), ref intValues);
            }
            else
            {
                string no = number.Substring(0, 3);
                Pars2Digit(no, ref intValues);
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
                Pars3Digit(number.Substring(3, number.Length - 3), ref intValues);
            }

        }

        private void Pars2Digit(string no, ref ArrayList intValues)
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

                Pars2Digit((val % 100).ToString(), ref intValues);
            }
            else if (val > 20 && val < 100)
            {
                if (val / 10 * 10 == val)
                    intValues.Add((val / 10 * 10).ToString());
                else
                    intValues.Add((val / 10 * 10).ToString() + "o");

                Pars2Digit((val % 10).ToString(), ref intValues);
            }
            else
            {
                if (val > 0)
                    intValues.Add(val.ToString());
                return;
            }

        }


        private void TrackCodeVoiceDigits(string code, ref ArrayList digitVoices)
        {
            if (!string.IsNullOrEmpty(code))
            {
                string zeroStr = GetStartZeros(code);
                if (zeroStr != "")
                {
                    AppendAtomicCodeVoice(zeroStr, ref digitVoices);
                    TrackCodeVoiceDigits(code.Substring(zeroStr.Length), ref digitVoices);
                }
                else if (code.Length < 4)
                {
                    AppendAtomicCodeVoice(code, ref digitVoices);
                }
                else if (code.Length == 4)
                {
                    AppendAtomicCodeVoice(code[0].ToString() + code[1].ToString(), ref digitVoices);
                    AppendAtomicCodeVoice(code[2].ToString() + code[3].ToString(), ref digitVoices);
                }
                else
                {
                    AppendAtomicCodeVoice(code.Substring(0, 3), ref digitVoices);
                    TrackCodeVoiceDigits(code.Substring(3), ref digitVoices);
                }
            }
        }

        private string GetStartZeros(string code)
        {
            string zeroStr = string.Empty;
            if (code.StartsWith("0"))
            {
                foreach (char digit in code)
                {
                    if (digit == '0')
                    {
                        zeroStr += "0";
                    }
                    else
                    {
                        break;
                    }
                }
            }
            return zeroStr;
        }

        private void AppendAtomicCodeVoice(string code, ref ArrayList digitVoices)
        {
            string zeroStr = GetStartZeros(code);
            if (zeroStr != "")
            {
                digitVoices.Add(zeroStr);
                if (zeroStr.Length < code.Length)
                {
                    digitVoices.Add(Constants.VoiceName_SilentHalfSecond);
                    digitVoices.Add(code.Substring(1));
                }
            }
            else if (code.Length == 3)
            {
                if (code.Substring(1) == "00")
                {
                    digitVoices.Add(code);
                }
                else
                {
                    digitVoices.Add(code[0] + "00" + NumberSuffix.o.ToString());
                    AppendAtomic2DigitsCodeVoice(code.Substring(1), digitVoices);
                }
            }
            else if (code.Length == 2)
            {
                AppendAtomic2DigitsCodeVoice(code, digitVoices);
            }
            else // If code.Length = 1 Then
            {
                digitVoices.Add(code);
            }
            digitVoices.Add(Constants.VoiceName_SilentHalfSecond);
        }

        private void AppendAtomic2DigitsCodeVoice(string code, ArrayList digitVoices)
        {
            if (code[0] == '0')
            {
                digitVoices.Add(code[1]);
            }
            else if (code[0] == '1' || code[1] == '0')
            {
                digitVoices.Add(code);
            }
            else
            {
                digitVoices.Add(code[0] + "0" + NumberSuffix.o.ToString());
                digitVoices.Add(code[1]);
            }
        }

        public void SpecifyTypeAndAddVoice(string exp)
        {
            try
            {
                int res;

                if (string.IsNullOrEmpty(exp))
                {
                    return;
                }
                if (Regex.IsMatch(exp, @"^\d{1,2}:\d{1,2}$"))
                {
                    AddTimeVoice(exp);
                }
                else if (Regex.IsMatch(exp, @"\d+/\d+/\d+"))
                {
                    DateTime _date = PersianDateTime.PersianToGregorian(exp).Value;
                    AddDateVoice(_date);
                }
                else if (exp.IndexOf("-") > -1 || exp.StartsWith("0") || exp.Length > Constants.VoiceCodeMaxLengthAsNumber)
                {
                    AddCodeVoice(exp.Replace("-", ""));
                }
                else if (int.TryParse(exp, out res))
                {
                    AddNumericVoice(int.Parse(exp));
                }
                else
                {
                    Logger.Write(LogType.Warning, "Invalid voice expression:{0}", exp);
                }
            }
            catch (Exception ex)
            {
                Logger.Write(ex, "Code:{0}", exp);
            }
        }

        public void AddNumericVoice(int number)
        {
            ArrayList digitVoices = new ArrayList();
            ParseDigits(number.ToString(), ref digitVoices);
            foreach (object digitVoice in digitVoices)
            {
                AddVoice(digitVoice.ToString());
            }
        }

        public void AddNumericVoice(long number)
        {
            ArrayList digitVoices = new ArrayList();
            ParseDigits(number.ToString(), ref digitVoices);
            foreach (object digitVoice in digitVoices)
            {
                AddVoice(digitVoice.ToString());
            }
        }

        public void AddCodeVoice(string code)
        {
            ArrayList digitVoices = new ArrayList();
            TrackCodeVoiceDigits(code, ref digitVoices);

            foreach (object digitVoice in digitVoices)
            {
                AddVoice(digitVoice.ToString());
            }
        }

        public void AddTimeVoice(TimeSpan time, System.Nullable<bool> saate)
        {
            if (saate.HasValue)
            {
                if (saate.Value)
                {
                    AddVoice(Constants.VoiceName_Saate);
                }
                else
                {
                    AddVoice(Constants.VoiceName_Saat);
                }
            }

            AddTimeVoice(time);
        }

        public void AddTimeVoice(string timeString)
        {
            TimeSpan time = TimeSpan.Parse(timeString.Split(' ')[0]);
            AddTimeVoice(time);
        }

        public void AddTimeVoice(TimeSpan time)
        {
            if (time.Hours == 0 && time.Minutes == 0)
            {
                AddVoice(Constants.VoiceName_NimeShab);
            }

            if (time.Hours != 0)
            {
                AddVoice(time.Hours, (time.Minutes == 0 ? (NumberSuffix?)null : NumberSuffix.o));
            }

            if (time.Minutes != 0)
            {
                AddVoice(time.Minutes, null);
                AddVoice(Constants.VoiceName_Daqiqe);
            }
        }

        public void AddDateVoice(DateTime time)
        {
            Folder.PersianDateTime per = new Folder.PersianDateTime(time);
            AddVoice(per.Day, NumberSuffix.ome);
            AddVoice(per.MonthName.ToString());
            AddVoice(per.Year, null);
        }

        public void AddFullTimeVoice(DateTime time)
        {
            if (time.Hour == 0 && time.Minute == 0)
            {
                AddVoice(Constants.VoiceName_NimeShabe);
            }

            if (time.Hour != 0)
            {
                AddVoice(Constants.VoiceName_Saate);
                AddVoice(time.Hour, (time.Minute == 0 ? (NumberSuffix?)null : NumberSuffix.o));
            }

            if (time.Minute != 0)
            {
                AddVoice(time.Minute, null);
                AddVoice(Constants.VoiceName_Daqiqeye);
            }

            AddVoice(time.DayOfWeek.ToString());
            AddDateVoice(time);
        }

        private void AddVoiceByType(string voiceValue, VoiceType type)
        {
            AddVoiceByType(voiceValue, type, (VoiceGroup)2);
            //            AddVoiceByType(voiceValue, type, (Pendar.Ums.Model.VoiceGroup)Config.Default.Common.CurrentVoiceGroup);
        }

        private void AddVoiceByType(string voiceValue, VoiceType type, VoiceGroup group)
        {
            switch (type)
            {
                case VoiceType.String:
                    AddVoice(voiceValue, group);
                    break;

                case VoiceType.Number:
                    int number = int.Parse(voiceValue);
                    AddVoice(number, null);
                    break;

                case VoiceType.Code:
                    AddCodeVoice(voiceValue);
                    break;

                case VoiceType.Saat:
                    DateTime time = DateTime.Parse(voiceValue);
                    AddTimeVoice(time.TimeOfDay, false);
                    break;

                case VoiceType.Saate:
                    DateTime time_1 = DateTime.Parse(voiceValue);
                    AddTimeVoice(time_1.TimeOfDay, true);
                    break;

                case VoiceType.FullTime:
                    DateTime time_2 = DateTime.Parse(voiceValue);
                    AddFullTimeVoice(time_2);
                    break;
            }
        }

        public void AddVoice(string name)
        {
            AddVoice(name, (VoiceGroup)2);
            //            AddVoice(name, (Pendar.Ums.Model.VoiceGroup)Config.Default.Common.CurrentVoiceGroup);
        }

        public void AddVoice(string name, VoiceGroup group)
        {
            Match mtch = Regex.Match(name, Constants.RegexSpecialVoice, RegexOptions.Compiled);
            if (mtch.Groups[Constants.RegexSpecialVoice_SpecialVoice].Success)
            {
                VoiceType type = (VoiceType)Enum.Parse(typeof(VoiceType), mtch.Groups[Constants.RegexSpecialVoice_SpecialVoice].Value);
                string value = mtch.Groups["Value"].Value;
                AddVoiceByType(value, type, group);
            }
            else
            {
                byte[] data = null;
                try
                {
                    using (UmsDataContext dc = new UmsDataContext())
                    {
                        var result = dc.Voices.FirstOrDefault(p => p.Name == name && p.VoiceGroup == (byte)group);
                        if (result != null)
                            data = result.Data.ToArray();
                        else
                            Logger.WriteIf(!string.IsNullOrEmpty(name), LogType.Error, "Voice Name '{0}' in Group {1} dose not exist.", name, group);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Write(ex);
                }
                if (data != null && data.Length > 0)
                {
                    stream.Write(data, 0, data.Length);
                }
            }
        }
        #endregion

        #region " Public Static Methods "

        //Public Shared Function Exists(ByVal voiceName As String, ByVal Group As VoiceGroup) As Boolean
        //    Dim voiceAdapter As New VoiceTableAdapter
        //    Return voiceAdapter.Exists(Group, voiceName)
        //End Function

        public static string CreateSpecialVoice(VoiceType type, object voice)
        {
            switch (type)
            {
                case VoiceType.FullTime:
                case VoiceType.Saat:
                case VoiceType.Saate:
                    return string.Format("{0}@{2}", type, System.Convert.ToDateTime(voice).Ticks);

                default:
                    return string.Format("{0}@{1}", type, voice);
            }
        }

        public void AddVoice(byte[] buffer)
        {
            if (buffer != null)
            {
                stream.Write(buffer, 0, buffer.Length);
            }
        }

        public void AddVoice(int number, NumberSuffix? suffix)
        {
            if (number == 0)
            {
                AddVoice("0");
            }
            else
            {
                ArrayList digitVoices = new ArrayList();
                //TrackNumericVoiceDigits(number.ToString(), ref digitVoices, null);
                ParseDigits(number.ToString(), ref digitVoices);
                if (suffix.HasValue)
                {
                    digitVoices[digitVoices.Count - 1] += suffix.ToString();
                }

                foreach (object digitVoice in digitVoices)
                {
                    AddVoice(digitVoice.ToString());
                }
            }
        }

        private void AddVoiceByID(string id)
        {
            Guid voiceGuid;
            if (Guid.TryParse(id, out voiceGuid))
                AddVoice(voiceGuid);
            else
                Logger.Write(LogType.Exception, "Voice Guid is invalid:{0}");
        }

        public void AddVoice(Guid id)
        {
            byte[] data = null;
            try
            {
                using (UmsDataContext dc = new UmsDataContext())
                {
                    Voice result = dc.Voices.FirstOrDefault(p => p.ID.ToString() == id.ToString());
                    if (result != null)
                        data = result.Data.ToArray();//voiceAdapter.GetByID(id);
                }
            }
            catch (SqlException ex)
            {
                Logger.Write(ex, "Voice ID:{0}", id.ToString());
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }

            if (data == null || data.Length == 0)
            {
                Logger.Write(LogType.Error, "AddVoice, Voice message not found, ID: '{0}'", id);
            }
            else
            {
                if (data.Length == 1)
                {

                }
                else
                    stream.Write(data, 0, data.Length);
            }
        }

        public void AddVoice(Voice voice, VoiceGroup group)
        {
            if (voice.ID != null)
            {
                AddVoice(voice.ID);
            }
            else
            {
                AddVoice(voice.Name, group);
            }
        }

        #endregion

        public void AddPlayNodeVoices(PlayNode node, object invokeResult)
        {
            AddPlayNodeVoices(node, invokeResult, (VoiceGroup)2);
        }

        public void AddPlayNodeVoices(PlayNode node, object invokeResult, VoiceGroup group)
        {
            foreach (Schema.Voice voice in node.Voice)
            {

                if (!string.IsNullOrEmpty(voice.ID))
                    AddVoiceByID(voice.ID);
                else if (!string.IsNullOrEmpty(voice.Name))
                {
                    if (voice.Group.HasValue)
                        AddVoiceByType(voice.Name, (VoiceType)voice.Type, (VoiceGroup)voice.Group.Value);
                    else
                        AddVoiceByType(voice.Name, (VoiceType)voice.Type);
                }
                else
                {
                    Logger.Write(LogType.Warning, "Empty voice name, node:{0}", node.ID);
                }
            }
        }
    }
}
