using System.Collections.Generic;
using System.Linq;
using Folder.Commands;
using Plugin.Mailbox.Views;
using System.Collections;
using UMSV;
using Folder;
using System;
using Enterprise;

namespace Plugin.Mailbox.ViewModels
{
    public class MailBoxViewModel : DataDrivenViewModel, Folder.IEditableForm
    {
        private int PageSize = 1000;
        private int PageNumber = 1;

        private string mailBoxFilter = string.Empty;

        public IEnumerable<UMSV.Mailbox> Mailboxes
        {
            get
            {
                if (User.Current.AllRoles.Select(t => t.ID).Contains(UMSV.Constants.Role_MailboxAccess))
                    return DB.Mailboxes.Where(m => m.BoxNo.Contains(mailBoxFilter.Trim())).Take(PageNumber * PageSize);

                return DB.Mailboxes.Where(m => m.BoxNo.Contains(mailBoxFilter.Trim()) && User.Current.AllRoles.Select(t => t.ID).Contains(m.AccessId)).Take(PageNumber * PageSize);
            }
        }

        public int AllMailboxesCount
        {
            get
            {
                if (User.Current.AllRoles.Select(t => t.ID).Contains(UMSV.Constants.Role_MailboxAccess))
                    return DB.Mailboxes.Count();

                return DB.Mailboxes.Count(m => User.Current.AllRoles.Select(t => t.ID).Contains(m.AccessId));
            }
        }

        public int RetrievedMailboxesCount
        {
            get
            {
                return AllMailboxesCount < PageSize ? AllMailboxesCount : PageNumber * PageSize;
            }
        }

        public bool CanRetrieveMore
        {
            get
            {
                return !(RetrievedMailboxesCount >= AllMailboxesCount);
            }
        }

        public UMSV.Mailbox SelectedMailbox
        {
            get;
            set;
        }

        private void Refresh()
        {
            DB = new UmsDataContext();
            SendPropertyChanged();
        }

        private bool isGeneral;
        public bool IsGeneral
        {
            get
            {
                return isGeneral;
            }
            set
            {
                isGeneral = value;
                SendPropertyChanged("Mailboxes");
            }
        }

        public IEnumerable Items
        {
            get;
            set;
        }

        #region Commands

        private DelegateCommand<string> _SearchCommand;
        public DelegateCommand<string> SearchCommand
        {
            get
            {
                if (_SearchCommand == null)
                {
                    _SearchCommand = new DelegateCommand<string>(delegate(string boxNo)
                    {
                        mailBoxFilter = boxNo;
                        PageNumber = 1;
                        SendPropertyChanged("RetrievedMailboxesCount");
                        SendPropertyChanged("CanRetrieveMore");
                        SendPropertyChanged("Mailboxes");
                    });
                }
                return _SearchCommand;
            }
        }

        private DelegateCommand _MoreResultCommand;
        public DelegateCommand MoreResultCommand
        {
            get
            {
                if (_MoreResultCommand == null)
                {
                    _MoreResultCommand = new DelegateCommand(delegate
                    {
                        PageNumber++;
                        SendPropertyChanged("AllMailboxesCount");
                        SendPropertyChanged("RetrievedMailboxesCount");
                        SendPropertyChanged("CanRetrieveMore");
                        SendPropertyChanged("Mailboxes");
                    });
                }
                return _MoreResultCommand;
            }
        }


        private DelegateCommand _NewMailboxCommand;
        public DelegateCommand NewMailboxCommand
        {
            get
            {
                if (_NewMailboxCommand == null)
                {
                    _NewMailboxCommand = new DelegateCommand(delegate
                    {
                        MailboxEditView dlg = new MailboxEditView(new MailboxEditViewModel(null, MailboxType.Private));
                        if (dlg.ShowDialog() == true)
                            Refresh();
                    });
                }
                return _NewMailboxCommand;
            }
        }

        private DelegateCommand _NewPublicMailboxCommand;
        public DelegateCommand NewPublicMailboxCommand
        {
            get
            {
                if (_NewPublicMailboxCommand == null)
                {
                    _NewPublicMailboxCommand = new DelegateCommand(delegate
                    {
                        PublicMailboxEditView dlg = new PublicMailboxEditView(new MailboxEditViewModel(null, MailboxType.Public));
                        if (dlg.ShowDialog() == true)
                            Refresh();
                    });
                }
                return _NewPublicMailboxCommand;
            }
        }

        //private DelegateCommand<TicketServiceStructure> ticketDetailCommand;
        //public DelegateCommand<TicketServiceStructure> TicketDetailCommand
        //{
        //    get
        //    {
        //        if (ticketDetailCommand == null)
        //        {
        //            ticketDetailCommand = new DelegateCommand<TicketServiceStructure>(DetailofTicket);
        //        }
        //        return ticketDetailCommand;
        //    }
        //}

        //private void DetailofTicket(TicketServiceStructure tss)
        //{
        //    NOC.Pages.Ticket.TicketDetail Sv = new NOC.Pages.Ticket.TicketDetail()
        //    {
        //        DataContext = new TicketDetailViewModel(tss.ID, tss.ServiceID, tss.TicketID)
        //    };
        //    Folder.Console.Navigate(Sv, "");
        //}

        private DelegateCommand _EditMailboxCommand;
        public DelegateCommand EditMailboxCommand
        {
            get
            {
                if (_EditMailboxCommand == null)
                {
                    _EditMailboxCommand = new DelegateCommand(delegate
                    {
                        var type = (MailboxType)SelectedMailbox.Type;
                        var viewModel = new MailboxEditViewModel(SelectedMailbox, type);
                        if (type == MailboxType.Private)
                        {
                            if (new MailboxEditView(viewModel).ShowDialog() == true)
                                Refresh();
                        }
                        else
                            if (new PublicMailboxEditView(viewModel).ShowDialog() == true)
                                Refresh();
                    });
                }
                return _EditMailboxCommand;
            }
        }

        private DelegateCommand _MessagesCommand;
        public DelegateCommand MessagesCommand
        {
            get
            {
                if (_MessagesCommand == null)
                {
                    _MessagesCommand = new DelegateCommand(delegate
                    {
                        var form = new MailboxMessagesForm();
                        form.BoxNo = SelectedMailbox.BoxNo;
                        Folder.Console.Navigate(form, string.Format("پيام های صندوق شماره {0}", SelectedMailbox.BoxNo));
                    });
                }
                return _MessagesCommand;
            }
        }


        #endregion

        public bool Save()
        {
            //CreateAccessForNewMailboxes();

            //foreach (UMSV.Mailbox mailbox in DB.GetChangeSet().Updates)
            //    ModifyUpdatedMailboxesRole(mailbox);

            //foreach (UMSV.Mailbox mailbox in DB.GetChangeSet().Deletes)
            //    SweepDeletedMailboxesRole(mailbox);

            DB.SubmitChanges();
            return true;
        }

        private void CreateAccessForNewMailboxes()
        {
            using (FolderDataContext folder = new FolderDataContext())
            {
                var baseRole = folder.Roles.FirstOrDefault(p => p.ID == UMSV.Constants.Role_MailboxAccess);
                if (baseRole == null)
                {
                    baseRole = new Role()
                    {
                        ID = Constants.Role_MailboxAccess,
                        Name = "صندوق های صوتی",
                        Type = Role.RoleType_Simple,
                        ParentID = Guid.Empty
                    };
                    folder.Roles.InsertOnSubmit(baseRole);
                    folder.SubmitChanges();
                }

                List<Guid> allMailboxAccess = folder.Roles.Where(t => t.ParentID == Constants.Role_MailboxAccess).Select(t => t.ID).ToList();
                List<UMSV.Mailbox> newMailboxes;
                using (UmsDataContext context = new UmsDataContext())
                {
                    newMailboxes = Mailboxes.Where(t => !allMailboxAccess.Contains(t.AccessId)).ToList();
                }

                foreach (UMSV.Mailbox mailbox in newMailboxes)
                {
                    Role role = new Role()
                    {
                        ID = mailbox.AccessId,
                        Name = (mailbox.Name ?? "") + " - " + mailbox.BoxNo,
                        ParentID = baseRole.ID,
                        Type = Role.RoleType_Simple
                    };

                    folder.Roles.InsertOnSubmit(role);
                }

                folder.SubmitChanges();
            }
        }

        private void SweepDeletedMailboxesRole(UMSV.Mailbox selectedMailbox)
        {
            using (FolderDataContext dc = new FolderDataContext())
            {
                Role role = dc.Roles.FirstOrDefault(p => p.ID == selectedMailbox.AccessId);
                dc.Roles.DeleteOnSubmit(role);
                UserRole ur = dc.UserRoles.FirstOrDefault(p => p.RoleID == role.ID && p.UserID == Folder.User.Current.ID);
                if (ur != null)
                    dc.UserRoles.DeleteOnSubmit(ur);
                dc.SubmitChanges();
                Folder.User.Current.AllRoles.Remove(role);
            }
        }

        private void ModifyUpdatedMailboxesRole(UMSV.Mailbox selectedMailbox)
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
