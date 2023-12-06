using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Enterprise;
using UMSV.Schema;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Speech.AudioFormat;
using System.Speech.Synthesis;

namespace UMSV.Engine
{
    public partial class VoipService
    {
        void TrackPlayNode(SipDialog sipDialog)
        {
            var node = sipDialog.CurrentNode.AsPlayNode;

            VoiceStream voiceStream = new VoiceStream();
            if (node != null)
                foreach (Schema.Voice voice in node.Voice)
                {
                    if (CheckForTtsVoice(sipDialog, voice, voiceStream))
                        continue;

                    else if (CheckForPropertyVoice(sipDialog, voice, voiceStream))
                        continue;

                    if (!string.IsNullOrEmpty(voice.ID))
                    {
                        voiceStream.AddVoice(new Guid(voice.ID));
                    }
                    else if (!string.IsNullOrEmpty(voice.Name) && !voice.Name.Contains('['))
                    {
                        if (voice.Group.HasValue)
                        {
                            voiceStream.AddVoice(voice.Name, (VoiceGroup)voice.Group);
                        }
                        else
                        {
                            voiceStream.AddVoice(voice.Name);
                        }
                    }
                }

            SipServer.PlayVoice(sipDialog, voiceStream.stream.ToArray());
        }

        bool CheckForTtsVoice(SipDialog sipDialog, Schema.Voice voice, VoiceStream voiceStream)
        {
            if (voice.Name == null || !Regex.IsMatch(voice.Name, @"\["".+""\]"))
                return false;

            System.Speech.Synthesis.SpeechSynthesizer speech = new System.Speech.Synthesis.SpeechSynthesizer();
            var format = new SpeechAudioFormatInfo(EncodingFormat.ALaw, 8000, 8, 1, 8000, 1, null);
            speech.SetOutputToAudioStream(voiceStream.stream, format);

            PromptBuilder pbuilder = new PromptBuilder();
            PromptStyle pStyle = new PromptStyle();

            pStyle.Emphasis = PromptEmphasis.NotSet;
            pStyle.Rate = PromptRate.Slow;
            pStyle.Volume = PromptVolume.ExtraLoud;

            pbuilder.StartStyle(pStyle);
            pbuilder.StartVoice(VoiceGender.Female, VoiceAge.Teen, 2);
            pbuilder.StartSentence();
            pbuilder.AppendText(voice.Name);
            pbuilder.EndSentence();
            pbuilder.EndVoice();
            pbuilder.EndStyle();

            speech.Speak(pbuilder);

            return true;
        }

        bool CheckForPropertyVoice(SipDialog dialog, Schema.Voice voice, VoiceStream voiceStream)
        {
            if (voice.Name == null || !Regex.IsMatch(voice.Name, @"\[.+\]"))
                return false;

            string voicePropertyName = voice.Name.Trim('[').Trim(']');
            Logger.Write(LogType.Info, "(T{0}:{1})->Invoking property '{2}'", dialog.CalleeID, dialog.CallerID, voicePropertyName);

            PropertyInfo addinsProperty = null;
            object result = null;

            if (dialog.GraphAddins != null)
            {
                try
                {
                    addinsProperty = dialog.GraphAddins.GetType().GetProperty(voicePropertyName);
                    if (addinsProperty != null)
                        result = addinsProperty.GetValue(dialog.GraphAddins, null);
                }
                catch (Exception ex)
                {
                    Logger.Write(ex, "Exception in calling property '{0}' Addins '{1}'", voicePropertyName, dialog.GraphAddins.GetType().FullName);
                }
            }

            if (addinsProperty == null)
                result = GetCorePropertyVoice(dialog, voicePropertyName);

            if (result == null)
                Logger.WriteInfo("({0}:{1})->Invoking property '{2}' on plugin, result is null.", dialog.CalleeID, dialog.CallerID, voicePropertyName);
            else
                TryPlayVoiceProperty(voiceStream, result);

            return true;
        }

        void TryPlayVoiceProperty(VoiceStream voiceStream, object result)
        {
            if (result is DateTime)
                voiceStream.AddDateVoice((DateTime)result);

            else if (result is int)
                voiceStream.AddNumericVoice((int)result);

            else if (result is long)
                voiceStream.AddNumericVoice((long)result);

            else if (result is string)
            {
                if (result.ToString().Contains(','))
                {
                    string[] codes = result.ToString().Split(',');
                    foreach (string code in codes)
                    {
                        long res;
                        if (!string.IsNullOrEmpty(code) && long.TryParse(code, out res))
                            voiceStream.AddCodeVoice(code);
                        else
                            voiceStream.AddVoice(code);
                    }
                }
                else
                {
                    long res;
                    if (!string.IsNullOrEmpty(result.ToString()) && long.TryParse(result.ToString(), out res))
                        voiceStream.AddCodeVoice(result.ToString());
                    else
                        voiceStream.AddVoice(result.ToString());
                }
            }
            else if (result is byte[])
                voiceStream.AddVoice((byte[])result);

            else if (result is System.Data.Linq.Binary)
                voiceStream.AddVoice((result as System.Data.Linq.Binary).ToArray());

            else if (result is TimeSpan)
                voiceStream.AddTimeVoice((TimeSpan)result);

            else if (result is IEnumerable<object>)
            {
                foreach (object str in (IEnumerable<object>)result)
                {
                    TryPlayVoiceProperty(voiceStream, str);
                }
            }
            else if (result is NumberVoice)
                voiceStream.AddVoice(int.Parse(((NumberVoice)result).Value), ((NumberVoice)result).Suffix);

            else
                Logger.Write(LogType.Exception, "Invalid property value type, value:{0}, type:{1}", result, result.GetType());
        }

        void SipServer_OnPlayFinished(object sender, PlayFinishedEventArgs e)
        {

            //if (e.Dialog.Extension == "pfax" && e.Dialog.MediaType == SdpFieldMedia.MediaType.Audio)
            //{
            //    Logger.WriteImportant("Sending sample fax...");
            //    var FAXStream = this.GetType().Assembly.GetManifestResourceStream(this.GetType().Namespace + ".Stuff." + "sample.tif");
            //    SipServer.PlayVoice(e.Dialog, ReadToEnd(FAXStream).ToArray());
            //}
            if (e.Dialog.CurrentNode == null)
            {
                using (UmsDataContext context = new UmsDataContext())
                {
                    Mailbox mailbox = context.Mailboxes.Where(t => t.BoxNo == e.Dialog.CalleeID).SingleOrDefault();

                    if (e.Dialog.Status == DialogStatus.Connect && e.Dialog.RecordedVoice.Length != 0)
                    {
                        Logger.WriteInfo("Going to disconnect the call after recording in direct mailbox calling mode.");
                        SipServer.DisconnectCall(e.Dialog);
                        return;
                    }

                    if (mailbox != null && e.Dialog.Status != DialogStatus.Recording)
                    {
                        Logger.WriteInfo("Going to start recording in direct mailbox calling mode.");
                        SipServer.StartRecord(e.Dialog.DialogID);
                    }
                }
            }
            else if (e.Dialog.CurrentNode.Type == NodeType.Play)
            {
                e.Dialog.CurrentNodeID = e.Dialog.CurrentNode.AsPlayNode.TargetNode;
                TrackCurrentNode(e.Dialog);
            }
        }
    }
}
