using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Folder.EMQ;
using UMSV.Schema;
using Enterprise;
using System.Threading;

namespace UMSV
{
    [ServiceModule("UMSV", ServiceModuleSide.Client)]
    public class VoIPServiceClient_Plugin_UMSV : ServiceModuleClient
    {
        public static VoIPServiceClient_Plugin_UMSV Default;

        static VoIPServiceClient_Plugin_UMSV()
        {
            Default = ServiceModule.InstantiateServiceModule<VoIPServiceClient_Plugin_UMSV>();
        }

        public void RequestDialogs()
        {
            Call("RequestDialogs");
        }

        public void RequestAccounts()
        {
            Call("RequestAccounts");
        }

        public void RequestAccountsAndDialogs()
        {
            Call("RequestAccountsAndDialogs");
        }

        public void RequestVoice(string voiceName)
        {
            Call("RequestVoice", voiceName);
        }

        public void RequestVoiceByUser(string voiceName, Guid userID)
        {
            Call("RequestVoiceByUser", voiceName, userID);
        }

        public void ReloadGatewayConfig()
        {
            Call("ReloadGatewayConfig");
        }

        public void StartScheduledOutcall()
        {
            Call("StartScheduledOutcall");
        }

        public void StopScheduledOutcall()
        {
            Call("StopScheduledOutcall");
        }

        public void SetAccountEavesdropper(string eavesdropperId, string targetId)
        {
            Call("SetAccountEavesdropper", eavesdropperId, targetId);
        }

        public void FlushEavesdropper(string eavesdropperId)
        {
            Call("FlushEavesdropper", eavesdropperId);
        }

        public void DisconnectEavesdroppingCall(string eavesdropperId)
        {
            Call("DisconnectEavesdroppingCall", eavesdropperId);
        }

        public void ReleaseEavesdropper(string accountUserId)
        {
            Call("ReleaseEavesdropper", accountUserId);
        }

        //public void DisconnectEavesdropperDialog(string eavesdropperId)
        //{
        //    Call("DisconnectEavesdropperDialog", eavesdropperId);
        //}

        //#region SetDialogExtension
        //public bool SetDialogExtension(string dialogID, string extension)
        //{
        //    Call("SetDialogExtension", dialogID, extension);
        //    lock (SetDialogExtensionSyncObject)
        //    {
        //        return Monitor.Wait(SetDialogExtensionSyncObject, 20000);
        //    }
        //}

        //private object SetDialogExtensionSyncObject = new object();

        //[RemoteMethod]
        //public void SetDialogExtensionConfirm(CallInfo callInfo)
        //{
        //    lock (SetDialogExtensionSyncObject)
        //    {
        //        Monitor.Pulse(SetDialogExtensionSyncObject);
        //    }
        //}
        //#endregion

        //#region SetDialogDivertPartnerExtension
        //public bool SetDialogDivertPartnerExtension(string dialogID, string extension)
        //{
        //    Call("SetDialogDivertPartnerExtension", dialogID, extension);
        //    lock (SetDialogDivertPartnerExtensionSyncObject)
        //    {
        //        return Monitor.Wait(SetDialogDivertPartnerExtensionSyncObject, 20000);
        //    }
        //}

        //private object SetDialogDivertPartnerExtensionSyncObject = new object();

        //[RemoteMethod]
        //public void SetDialogDivertPartnerExtensionConfirm(CallInfo callInfo)
        //{
        //    lock (SetDialogDivertPartnerExtensionSyncObject)
        //    {
        //        Monitor.Pulse(SetDialogDivertPartnerExtensionSyncObject);
        //    }
        //}
        //#endregion

        public delegate void ResponseInquiryCiscoLinkStateHandler(Guid deviceID, int slot, int port, string result);
        public event ResponseInquiryCiscoLinkStateHandler OnResponseInquiryCiscoLinkState;

        [RemoteMethod]
        public void ResponseInquiryCiscoLinkState(CallInfo callInfo, Guid deviceID, int slot, int port, string result)
        {
            if (OnResponseInquiryCiscoLinkState != null)
                OnResponseInquiryCiscoLinkState(deviceID, slot, port, result);
        }

        public delegate void ResponseAccountsAndDialogsHandler(DateTime serverTime, List<SipAccount> accounts, List<SipDialog> dialogs);
        public event ResponseAccountsAndDialogsHandler OnResponseAccountsAndDialogs;

        [RemoteMethod]
        public void ResponseAccountsAndDialogs(CallInfo callInfo, DateTime serverTime, List<SipAccount> accounts, List<SipDialog> dialogs)
        {
            if (OnResponseAccountsAndDialogs != null)
                OnResponseAccountsAndDialogs(serverTime, accounts, dialogs);
        }

        public delegate void ResponseVoiceHandler(string voiceName, byte[] voice);
        public event ResponseVoiceHandler OnResponseVoice;

        [RemoteMethod]
        public void ResponseVoice(CallInfo callInfo, string voiceName, byte[] voice)
        {
            if (OnResponseVoice != null)
                OnResponseVoice(voiceName, voice);
        }

        public delegate void ResponseDialogsHandler(List<SipDialog> dialogs);
        public event ResponseDialogsHandler OnResponseDialogs;

        [RemoteMethod]
        public void ResponseDialogs(CallInfo callInfo, List<SipDialog> dialogs)
        {
            if (OnResponseDialogs != null)
                OnResponseDialogs(dialogs);
        }

        public delegate void ResponseAccountsHandler(List<SipAccount> accounts);
        public event ResponseAccountsHandler OnResponseAccounts;

        [RemoteMethod]
        public void ResponseAccounts(CallInfo callInfo, List<SipAccount> accounts)
        {
            if (OnResponseAccounts != null)
                OnResponseAccounts(accounts);
        }

        internal void ReloadGraphAssembly(Guid graphID)
        {
            Logger.WriteInfo("Client: ReloadGraphAssembly tell to server, Graph: {0}", graphID);
            Call("ReloadGraphAssembly", graphID);
        }

        internal void ReloadUmsvConfig()
        {
            Call("ReloadUmsvConfig");
        }
    }
}
