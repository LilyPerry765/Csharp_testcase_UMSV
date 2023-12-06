using System.Windows.Controls;
using UMSV;
using System.Text.RegularExpressions;
using UMSV.ViewModels;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Pendar.Ums.CompositeNodes
{
    [UMSV.CompositeNode(Tag = "Divert", Icon = "images/divert.png", Title = "دایورت", GroupIndex = 3, Index = 2, ChildMode = ChildModes.Childless)]
    public partial class DivertNode : UserControl, IValidatable
    {
        public DivertNode()
        {
            InitializeComponent();

            List<OperatorTeam> items = new List<OperatorTeam>();
            
            using (Folder.FolderDataContext dc = new Folder.FolderDataContext())
            {
                items = dc.Roles.Where(r => r.ParentID == Constants.TeamsRole).Select(r
                    => new OperatorTeam() {
                        ID = r.ID,
                        Name = r.Name,
                    }).ToList();
            }

            operatorsList.ItemsSource = items;
        }

        public class OperatorTeam
        {
            public Guid ID
            {
                get;
                set;
            }

            public string Name
            {
                get;
                set;
            }
        }

        public ValidationResult Validate()
        {
            ValidationResult result = ValidationResult.ValidResult;
            return result;
        }

        private void TargetTeam_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((this.DataContext as CompositeNodeViewModel).NodeData.DivertNodes[0].TargetTeam != null)
            {
                (this.DataContext as CompositeNodeViewModel).NodeData.DivertNodes[0].TargetPhone = string.Empty;
                (this.DataContext as CompositeNodeViewModel).NodeData.DivertNodes[0].DynamicTargetPhone = string.Empty;
            }
        }

        private void TargetPhone_TextChanged(object sender, TextChangedEventArgs e)
        {
            if ((this.DataContext as CompositeNodeViewModel).NodeData.DivertNodes[0].TargetPhone != string.Empty)
            {
                (this.DataContext as CompositeNodeViewModel).NodeData.DivertNodes[0].TargetTeam = null;
                (this.DataContext as CompositeNodeViewModel).NodeData.DivertNodes[0].DynamicTargetPhone = string.Empty;
            }
        }

        private void DynamicTargetPhone_TextChanged(object sender, TextChangedEventArgs e)
        {
            if ((this.DataContext as CompositeNodeViewModel).NodeData.DivertNodes[0].DynamicTargetPhone != string.Empty)
            {
                (this.DataContext as CompositeNodeViewModel).NodeData.DivertNodes[0].TargetTeam = null;
                (this.DataContext as CompositeNodeViewModel).NodeData.DivertNodes[0].TargetPhone = string.Empty;
            }
        }
    }
}
