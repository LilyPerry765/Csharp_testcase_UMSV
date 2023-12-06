using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UMSV.Engine
{
    internal class Constants
    {
        internal const string VoiceName_PlayRecordNotFound = "PlayRecordNotFound";
        internal const string VoiceName_EnterYourCode = "EnterYourCode";
        internal const string VoiceName_PlayInfoTable = "PlayInfoTable";
        internal const string VoiceName_YourFollowupCodeIs = "YourFollowupCodeIs";
        internal const string VoiceName_ConfirmEnterKey = "تائید شماره وارد شده";
        internal const string VoiceName_YourMessageSaved = "YourMessageSaved";
        internal const string VoiceName_GiveYourFollowupCode = "GiveYourFollowupCode";
        internal const string VoiceName_InvalidFollowupCode = "InvalidFollowupCode";
        internal const string VoiceName_NoAnswerYet = "NoAnswerYet";
        internal const string VoiceName_LeaveMessage = "LeaveMessage";
        internal const string VoiceName_LeaveMessageAndPressNine = "LeaveMessageAndPressNine";
        internal const string VoiceName_Bye = "Bye";
        internal const string VoiceName_Beep = "Beep";
        internal const string VoiceName_RingingVoice = "RingingVoice";

        internal const string VoiceName_Daqiqeye = "Daqiqeye";
        internal const string VoiceName_Daqiqe = "Daqiqe";
        internal const string VoiceName_Saate = "Saate";
        internal const string VoiceName_Saat = "Saat";
        internal const string VoiceName_NimeShab = "NimeShab";
        internal const string VoiceName_NimeShabe = "NimeShabe";
        internal const string VoiceName_WaitMusic = "WaitMusic";
        internal const string VoiceName_WaitMusic2 = "WaitMusic#2";
        internal const string VoiceName_SilentHalfSecond = "SilentHalfSecond";
        internal const string VoiceName_PlayAnswerMessage = "[PlayAnswerMessage]";

        internal const string DialogState_CodeStatusRecord = "CodeStatusRecord";
        internal const string DialogState_InfoTableFoundedRecord = "InfoTableFoundedRecord";
        internal const string DialogState_InfoTableFoundedRecordColumnIndex = "InfoTableFoundedRecordColumnIndex";
        internal const string DialogState_InfoTableID = "InfoTableID";
        internal const string DialogState_FollowupCode = "FollowupCode";
        internal const string DialogState_AnswerVoice = "AnswerVoice";
        internal const string DialogState_InformingRecord = "InformingRecord";

        internal const string NodeName_Bye = "Bye";
        internal const string NodeName_DisconnectCall = "DisconnectCall";
        internal const int VoiceCodeMaxLengthAsNumber = 9;
        public const string RegexSpecialVoice_SpecialVoice = "SpecialVoice";
        public const string RegexSpecialVoice = "((?<SpecialVoice>\\w+)@)?(?<Value>.*)";
        public const string VoiceName_InvalidKey = "InvalidKey";

        public const long InformingTimerInterval = 1000;
        public static Guid TeamsRole = new Guid("{3CED18BF-D71A-4A1E-BD8D-503ACC339BF9}");

        public const string GraphTrackNodeSeprator = ">";
        public const char GraphTrackDtmfSeprator = '"';
        public const int TransferFailureMaxTime = 3;


    }
}
