using System.Linq;
using Enterprise;
using Folder;
using System;
using System.Collections.Generic;
using UMSV;
using System.Data.Linq;

namespace Plugin.Mailbox.ViewModels
{
    public class MailboxEditViewModel : DataDrivenViewModel
    {
        public MailboxEditViewModel(UMSV.Mailbox mailbox, UMSV.MailboxType mailboxType)
        {
            if (mailbox != null)
            {
                IsNew = false;
                this.Mailbox = DB.Mailboxes.FirstOrDefault(mb => mb == mailbox);
            }
            else
            {
                IsNew = true;
                this.Mailbox = new UMSV.Mailbox()
                {
                    FollowupCodeStart = 1000,
                    Type = (byte)mailboxType,
                    FollowupCodeEnd = 100000,
                    PagingEnable = false,
                    EmailNewMessages = false,
                    AutoArchive = false,
                    PlayCallerIDBeforeMessage = false,
                    AccessId = System.Guid.NewGuid()
                };
                DB.Mailboxes.InsertOnSubmit(this.Mailbox);
            }
        }

        public bool IsNew
        {
            get;
            set;
        }

        public UMSV.Mailbox Mailbox
        {
            get;
            set;
        }        

        protected override bool OnSave()
        {
            bool saved;
            try
            {
                // this.Mailbox.PagingSchedule = Mailbox.Schedule.ToXElement();
                saved = base.OnSave();
                //if (this.Mailbox != null && IsNew && saved)
                //    CreateAccessForNewMailbox();
                //else if (this.Mailbox != null && !IsNew && saved)
                //    ModifyUpdatedMailboxRole(this.Mailbox);
            }
            catch (System.Exception ex)
            {
                Logger.Write(ex);
                saved = false;
                Folder.MessageBox.ShowError("اطلاعات دارای اشکال می باشد و امکان ثبت آن وجود ندارد.");
            }
            if (saved)
            {
                IsNew = false;
                DialogResult = true;
                SendPropertyChanged();
                return true;
            }
            return false;
        }

        private void CreateAccessForNewMailbox()
        {
            using (FolderDataContext folder = new FolderDataContext())
            {
                var baseRole = folder.Roles.FirstOrDefault(p => p.ID == UMSV.Constants.Role_MailboxAccess);
                if (baseRole == null)
                {
                    baseRole = new Role()
                    {
                        ID = UMSV.Constants.Role_MailboxAccess,
                        Name = "صندوق های صوتی",
                        Type = Role.RoleType_Simple,
                        ParentID = Guid.Empty
                    };
                    folder.Roles.InsertOnSubmit(baseRole);
                    folder.SubmitChanges();
                }

                Role role = new Role()
                {
                    ID = this.Mailbox.AccessId,
                    Name = (this.Mailbox.Name ?? "") + " - " + this.Mailbox.BoxNo,
                    ParentID = baseRole.ID,
                    Type = Role.RoleType_Simple
                };

                folder.Roles.InsertOnSubmit(role);


                string[] roleNames = User.Current.RoleNames.Split(',', '،');

                if (roleNames.Count() == 1)
                {
                    RoleMember roleMember = new RoleMember()
                    {
                        RoleID = role.ID,
                        ParentID = folder.Roles.Where(t => t.Name.Contains(roleNames[0])).Select(t => t.ID).FirstOrDefault()
                    };
                    folder.RoleMembers.InsertOnSubmit(roleMember);
                }
                else if (roleNames.Count() > 1)
                {
                    UserRole userRole = new UserRole()
                    {
                        RoleID = role.ID,
                        UserID = User.Current.ID
                    };
                    folder.UserRoles.InsertOnSubmit(userRole);
                }

                folder.SubmitChanges();
                User.Current.AllRoles.Add(role);
            }
        }

        private void ModifyUpdatedMailboxRole(UMSV.Mailbox selectedMailbox)
        {
            try
            {
                using (FolderDataContext dc = new FolderDataContext())
                {
                    Role role = dc.Roles.First(r => r.ID == selectedMailbox.AccessId);
                    if (role.Name != ((selectedMailbox.Name ?? "") + " - " + selectedMailbox.BoxNo))
                    {
                        //Role roleInDb = dc.Roles.First(p => p.ID == selectedMailbox.AccessId);
                        role.Name = (selectedMailbox.Name ?? "") + " - " + selectedMailbox.BoxNo;
                        dc.SubmitChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
        }
    }
}
