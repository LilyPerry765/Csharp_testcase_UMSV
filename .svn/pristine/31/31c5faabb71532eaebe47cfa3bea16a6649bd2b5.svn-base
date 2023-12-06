using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Folder;
using UMSV;
using Folder.Commands;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows;
using Enterprise;


namespace UMSV.ViewModels
{
    public class GraphsListViewModel : DataDrivenViewModel, IEditableForm, IFolderForm
    {
        public GraphsListViewModel()
        {

        }

        public bool Save()
        {
            foreach (Graph gr in DB.GetChangeSet().Inserts)
                CreateAccess(gr);

            foreach (Graph gr in DB.GetChangeSet().Updates)
                CheckAccess(gr);

            foreach (Graph gr in DB.GetChangeSet().Deletes)
                RemoveAccess(gr);

            DB.SubmitChanges();

            foreach (var graph in AssemblyChanged)
            {
                VoIPServiceClient_Plugin_UMSV.Default.ReloadGraphAssembly(graph.ID);
            }
            AssemblyChanged = new SafeCollection<Graph>();

            return true;
        }

        private void RemoveAccess(Graph gr)
        {
            using (FolderDataContext dc = new FolderDataContext())
            {
                Role role = dc.Roles.FirstOrDefault(p => p.ID == gr.ID);
                dc.Roles.DeleteOnSubmit(role);
                UserRole ur = dc.UserRoles.FirstOrDefault(p => p.RoleID == role.ID && p.UserID == Folder.User.Current.ID);
                if (ur != null)
                    dc.UserRoles.DeleteOnSubmit(ur);
                dc.SubmitChanges();
                Folder.User.Current.AllRoles.Remove(role);
            }
        }

        public IEnumerable<Graph> Graphs
        {
            get
            {
                try
                {
                    foreach (var graph in DB.Graphs)
                    {
                        CheckAccess(graph);
                        graph.PropertyChanged += GraphsListViewModel_PropertyChanged;
                    }

                    var graphs = DB.Graphs.ToList().Where(g => Folder.User.IsInRole(g.ID)).OrderBy(g => g.Code);

                    System.Collections.ObjectModel.ObservableCollection<Graph> c = new System.Collections.ObjectModel.ObservableCollection<Graph>(graphs);
                    c.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(c_CollectionChanged);
                    return c;
                }
                catch (System.Data.SqlClient.SqlException ex)
                {
                    Logger.WriteException("Loading graphs from UMSV database failed: {0}", ex.Message);
                    return null;
                }
            }
        }

        void GraphsListViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Assembly")
            {
                if (!AssemblyChanged.Contains(sender as Graph))
                    AssemblyChanged.Add(sender as Graph);
            }
        }

        SafeCollection<Graph> AssemblyChanged = new SafeCollection<Graph>();

        void c_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                //case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                //    InitGraph(e.NewItems[0] as Graph);
                //    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    DB.Graphs.DeleteAllOnSubmit(e.OldItems.Cast<Graph>());
                    break;
            }
        }

        //private void InitGraph(Graph newGraph)
        //{
        //    newGraph.ID = Guid.NewGuid();
        //    newGraph.Data = System.Xml.Linq.XElement.Parse(GraphViewModel.CreateGraph(newGraph.ID).Serialize());
        //    DB.Graphs.InsertOnSubmit(newGraph);
        //}

        public Graph SelectedGraph
        {
            get;
            set;
        }

        public void NewGraph(object sender, RoutedEventArgs e)
        {
            GraphViewModel graphVM = new GraphViewModel(Guid.Empty);
            GraphView dlg = new GraphView(graphVM);
            Folder.Console.Navigate(dlg, "درختواره جدید");
        }

        //public bool CanUserAddRows
        //{
        //    get
        //    {
        //        return Folder.User.HasEditAccess("UMSV.GraphsListView");
        //    }
        //}

        private ICommand showGraphCommand;
        private ICommand getAssemblyFile;
        public ICommand GetAssemblyFile
        {
            get
            {
                if (getAssemblyFile == null)
                    getAssemblyFile = new DelegateCommand<DependencyObject>((view) => {
                        if (SelectedGraph != null)
                        {
                            Cursor = Cursors.Wait;
                            System.Windows.Forms.Application.DoEvents();

                            System.Windows.Forms.OpenFileDialog dlg = new System.Windows.Forms.OpenFileDialog();
                            dlg.Filter = "DLLs|*.dll";
                            dlg.Multiselect = false;
                            dlg.ShowHelp = false;
                            dlg.Title = "Addins Files";
                            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                                SelectedGraph.Assembly = System.IO.File.ReadAllBytes(dlg.FileName);

                            Cursor = Cursors.Arrow;
                        }
                    });
                return getAssemblyFile;
            }

        }

        public ICommand ShowGraphCommand
        {
            get
            {
                if (showGraphCommand == null)
                    showGraphCommand = new DelegateCommand<DependencyObject>((view) => {
                        if (SelectedGraph != null)
                        {
                            try
                            {
                                Folder.Console.ShowProgress();
                                GraphViewModel graphVM = new GraphViewModel(SelectedGraph.ID);
                                GraphView dlg = new GraphView(graphVM);
                                Folder.Console.Navigate(dlg, string.Format("درختواره '{0}'", SelectedGraph.Name));
                            }
                            finally
                            {
                                Folder.Console.HideProgress();
                            }
                        }
                    });
                return showGraphCommand;
            }
        }

        private void CheckAccess(Graph selectedGraph)
        {
            try
            {
                if (User.IsInRole(Guid.Empty))
                {
                    if (!User.IsInRole(selectedGraph.ID))
                        CreateAccess(selectedGraph);
                    else
                    {
                        var role = User.Current.AllRoles.First(r => r.ID == selectedGraph.ID);
                        if (role.Name != selectedGraph.Name)
                        {
                            using (FolderDataContext dc = new FolderDataContext())
                            {
                                Role roleInDb = dc.Roles.First(p => p.ID == selectedGraph.ID);
                                roleInDb.Name = selectedGraph.Name;
                                dc.SubmitChanges();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
        }

        private void CreateAccess(Graph selectedGraph)
        {
            using (FolderDataContext dc = new FolderDataContext())
            {
                var baseRole = dc.Roles.FirstOrDefault(p => p.ID == Constants.Role_GraphAccess);
                if (baseRole == null)
                {
                    baseRole = new Role() {
                        ID = Constants.Role_GraphAccess,
                        Name = "دسترسی درختواره ها",
                        Type = Role.RoleType_Simple,
                        ParentID = Guid.Empty
                    };
                    dc.Roles.InsertOnSubmit(baseRole);
                    dc.SubmitChanges();
                }
                Role role = new Role() {
                    ID = selectedGraph.ID,
                    Name = selectedGraph.Name ?? "",
                    ParentID = baseRole.ID,
                    Type = Role.RoleType_Simple
                };

                dc.Roles.InsertOnSubmit(role);
                dc.SubmitChanges();
            }
        }

        #region IFolderForm Members

        public void Initialize(FolderFormHelper helper)
        {
            helper.Refresh += new EventHandler<RefreshEventArgs>(helper_Refresh);
        }

        void helper_Refresh(object sender, RefreshEventArgs e)
        {
            DB = new UmsDataContext();
            SendPropertyChanged("Graphs");
        }

        #endregion
    }
}
