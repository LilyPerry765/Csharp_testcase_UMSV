using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using UMSV.Schema;
using Folder.Audio;

namespace Pendar.Ums.CompositeNodes
{

    public partial class VoiceDialog : Window
    {
        UMSV.UmsDataContext db = new UMSV.UmsDataContext();
        private UMSV.Voice dbVoice;

        public VoiceDialog(Voice editingVoice)
        {
            InitializeComponent();
            if (editingVoice != null)
            {
                Voice = editingVoice;
                nameTextBox.Text = (Voice.Name ?? "").Trim('[', ']', ' ');
                dbVoice = db.Voices.FirstOrDefault(v => v.ID.ToString() == editingVoice.ID);
                if (dbVoice != null)
                    soundControl.Voice = dbVoice.Data.ToArray();
            }
            else
                Voice = new Voice();
        }

        public VoiceDialog()
            : this(null)
        {
        }

        public Voice Voice
        {
            private set;
            get;
        }

        private string CreateVoiceName()
        {
            string name;
            name = nameTextBox.Text.Trim('[', ']', ' ');
            if (string.IsNullOrWhiteSpace(name) && soundControl.FileInfo != null)
                name = System.IO.Path.GetFileNameWithoutExtension(soundControl.FileInfo.Name);
            return name;
        }

        private bool Validate()
        {
            if (IsForInfoTable)
            {
                if (soundControl.Voice == null)
                {
                    Folder.MessageBox.ShowWarning("لطفا صدای مربوطه را انتخاب يا ضبط نماييد.");
                    return false;
                }
            }
            else if (string.IsNullOrWhiteSpace(Voice.Name))
            {
                Folder.MessageBox.ShowWarning("لطفا نام صدا را وارد کنيد.");
                nameTextBox.Focus();
                return false;
            }
            return true;
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            Voice.Name = CreateVoiceName();
            if (Validate())
            {
                UpdateDatabase();
                DialogResult = true;
            }
        }

        private void UpdateDatabase()
        {
            if (soundControl.Voice == null)
            {
                Voice.ID = null;
                Voice.Name = string.Format("[{0}]", nameTextBox.Text);
                if (dbVoice != null)
                {
                    db.Voices.DeleteOnSubmit(dbVoice);
                    db.SubmitChanges();
                }
            }
            else
            {
                if (dbVoice == null)
                {
                    if (Voice.ID == null)
                        Voice.ID = Guid.NewGuid().ToString();

                    dbVoice = new UMSV.Voice()
                    {
                        ID = new Guid(Voice.ID),
                        VoiceGroup = 5
                    };
                    db.Voices.InsertOnSubmit(dbVoice);
                }
                dbVoice.Description = dbVoice.Name = Voice.Name;
                dbVoice.Data = soundControl.Voice;
                db.SubmitChanges();
            }
        }

        public bool IsForInfoTable
        {
            get;
            set;
        }
    }
}
