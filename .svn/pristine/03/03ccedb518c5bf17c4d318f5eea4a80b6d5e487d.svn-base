using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using UMSV.Schema;
using Folder.Audio;

namespace Pendar.Ums.CompositeNodes.UserControls
{
    public partial class VoiceSelector : UserControl
    {
        UMSV.UmsDataContext db = new UMSV.UmsDataContext();
        private string lastVoiceID;

        public VoiceSelector()
        {
            InitializeComponent();
        }

        public string SelectedVoiceID
        {
            get
            {
                return (string)GetValue(SelectedVoiceIDProperty);
            }
            set
            {
                SetValue(SelectedVoiceIDProperty, value);
            }
        }

        public static readonly DependencyProperty SelectedVoiceIDProperty =
            DependencyProperty.Register("SelectedVoiceID", typeof(string), typeof(VoiceSelector), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnPropertChanged))
            {
                BindsTwoWayByDefault = true
            });

        public Voice SelectedVoice
        {
            get
            {
                return (Voice)GetValue(SelectedVoiceProperty);
            }
            set
            {
                SetValue(SelectedVoiceProperty, value);
            }
        }

        public static readonly DependencyProperty SelectedVoiceProperty =
            DependencyProperty.Register("SelectedVoice", typeof(Voice), typeof(VoiceSelector), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnPropertChanged))
            {
                BindsTwoWayByDefault = true
            });

        protected static void OnPropertChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            VoiceSelector me = sender as VoiceSelector;
            if (e.Property == SelectedVoiceProperty)
            {
                Voice val = e.NewValue as Voice;
                me.SelectedVoiceID = val.ID;
                if (val.ID != null)
                {
                    var voice = me.db.Voices.FirstOrDefault(v => v.ID.ToString() == val.ID);
                    if (voice != null && voice.Data != null && voice.Data.Length > 0)
                    {
                        me.soundControl.Voice = new byte[voice.Data.Length];
                        Array.Copy(voice.Data.ToArray(), me.soundControl.Voice, voice.Data.Length);
                    }

                    //me.soundControl.Sound = new Sound(me.db.Voices.Single(v => v.ID.ToString() == val.ID).Data.ToArray());
                    me.customRadioButton.IsChecked = true;
                }
                else if (me.soundControl !=null && me.soundControl.Voice != null)
                {
                    me.soundControl.Stop();
                    me.soundControl.Voice = null;
                }
            }
            else if (e.Property == SelectedVoiceIDProperty)
            {
                string id = e.NewValue as string;
                if (me.SelectedVoice == null)
                    me.SelectedVoice = new Voice()
                    {
                        ID = id
                    };
                else if (me.SelectedVoice.ID != id)
                {
                    me.SelectedVoice.ID = id;
                }
            }
        }

        private bool showOptions = true;
        public bool ShowOptions
        {
            get
            {
                return showOptions;
            }
            set
            {
                showOptions = value;
                if (showOptions)
                {
                    if (this.Content != groupBox)
                    {
                        this.Content = groupBox;
                        stackPannel.Children.Add(soundControl);
                    }
                }
                else
                {
                    if (this.Content != soundControl)
                    {
                        stackPannel.Children.Remove(soundControl);
                        this.Content = soundControl;
                        customRadioButton.IsChecked = true;
                    }
                }
            }
        }

        private void defaultRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (IsInitialized)
            {
                if (soundControl != null)
                    soundControl.Stop();
                SelectedVoice.ID = null;
            }
        }

        private void soundControl_SoundRemoved(object sender, EventArgs e)
        {
            //UMSV.Voice voiceToDelete = db.Voices.Single(v => v.ID.ToString() == SelectedVoice.ID);
            //db.Voices.DeleteOnSubmit(voiceToDelete);
            //db.SubmitChanges();
            //SelectedVoiceID = null;
        }

        private void soundControl_VoiceChanged(object sender, VoiceChangedEventArgs e)
        {
            UMSV.Voice dbVoice = db.Voices.FirstOrDefault(v => v.ID.ToString() == SelectedVoiceID);
            if (dbVoice == null)
            {
                dbVoice = new UMSV.Voice()
                {
                    ID = string.IsNullOrEmpty(SelectedVoiceID) ? Guid.NewGuid() : new Guid(SelectedVoiceID),
                    VoiceGroup = 5
                };
                db.Voices.InsertOnSubmit(dbVoice);
            }
            dbVoice.Data = soundControl.Voice;
            db.SubmitChanges();
            SelectedVoiceID = lastVoiceID = dbVoice.ID.ToString();
        }

        private void customRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (lastVoiceID != null)
                SelectedVoice.ID = lastVoiceID;
        }

    }
}
