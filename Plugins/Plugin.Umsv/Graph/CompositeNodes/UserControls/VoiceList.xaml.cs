using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UMSV.Schema;
using System.IO;
using Folder.Audio;
using Folder;

using Enterprise;

namespace Pendar.Ums.CompositeNodes.UserControls
{

    public partial class VoiceList : ListView
    {

        public VoiceList()
        {
            InitializeComponent();
           // Items.Filter = (i) => (i as Voice).Name != Model.Constants.InfoTableRecordDataPropertyName;
        }

        private void addMenuItem_Click(object sender, RoutedEventArgs e)
        {
            VoiceDialog dlg = new VoiceDialog()
            {
                //Owner = Window.GetWindow(this),
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                IsForInfoTable = IsForInfoTable
            };
            
            if (dlg.ShowDialog() == true)
            {
                if (ItemsSource == null)
                {
                    ItemsSource = new List<Voice>();
                }
                if (IsForInfoTable && Voices.Any())
                {
                    Voices.Add(new Voice()
                    {
                        Name = UMSV.Constants.InfoTableRecordDataPropertyName
                    });
                }
                Voices.Add(dlg.Voice);

                RefreshItemsSource(true);
            }
        }

        private List<Voice> Voices
        {
            get
            {
                return ItemsSource as List<Voice>;
            }
        }

        private void RefreshItemsSource(bool selectLast = false)
        {
            var tmp = ItemsSource;
            int i = SelectedIndex;
            ItemsSource = null;
            ItemsSource = tmp;
            if (selectLast)
                SelectedIndex = Items.Count - 1;
            else
                SelectedIndex = Math.Min(i, Items.Count - 1);
        }

        private void editMenuItem_Click(object sender, RoutedEventArgs e)
        {
            EditVoice();
        }


        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            EditVoice();
        }

        private void EditVoice()
        {
            if (SelectedIndex > -1 && (!IsForInfoTable || (SelectedItem as Voice).ID != null))
            {

                VoiceDialog dlg = new VoiceDialog(SelectedItem as Voice)
                {
                    //Owner = Window.GetWindow(this),
                    WindowStartupLocation = WindowStartupLocation.CenterScreen,
                    IsForInfoTable = IsForInfoTable
                };
                if (dlg.ShowDialog() == true)
                {
                    Voices[SelectedIndex] = dlg.Voice;
                    RefreshItemsSource();
                }
            }
        }

        Voice voiceToMove;
        private void ListViewItem_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                voiceToMove = (sender as ListViewItem).Content as Voice;
            }
        }

        private void ListViewItem_MouseEnter(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && sender != voiceToMove && voiceToMove != null)
            {
                int sourceIndex = Voices.IndexOf(voiceToMove);
                int destIndex = Voices.IndexOf((sender as ListViewItem).Content as Voice);
                Voices.Remove(voiceToMove);
                if (sourceIndex < destIndex && destIndex < Voices.Count)
                    Voices.Insert(destIndex + 1, voiceToMove);
                else
                    Voices.Insert(destIndex, voiceToMove);
                RefreshItemsSource();
            }
        }

        private void deleteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            RemoveVoice();
        }

        private void RemoveVoice()
        {
            if (SelectedIndex > -1)
            {
                if (Folder.MessageBox.ShowQuestion("صدا(ها)ی انتخاب شده حذف شود؟") == MessageBoxResult.Yes)
                {
                    foreach (var item in SelectedItems)
                    {
                        Voices.Remove(item as Voice);
                    }
                    RefreshItemsSource();
                }
            }
        }

        // MouseUp doesn't raise here,(I don't know why!) so I used mouseMove instead.
        private void ListViewItem_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released && voiceToMove != null)
            {
                voiceToMove = null;
            }
        }

        private void ToolBar_Loaded(object sender, RoutedEventArgs e)
        {
            ToolBar toolBar = sender as ToolBar;
            var overflowGrid = toolBar.Template.FindName("OverflowGrid", toolBar) as FrameworkElement;
            if (overflowGrid != null)
            {
                overflowGrid.Visibility = Visibility.Collapsed;
            }
        }

        //Sound playingSound;
        //Button playButton;
        private void playButton_Click(object sender, RoutedEventArgs e)
        {
            //if (playButton == null)
            //    playButton = sender as Button;

            //if (playingSound == null)
            //{
            //    List<byte> buffer = new List<byte>();
            //    UMSV.UmsDataContext db = new UMSV.UmsDataContext();
            //    foreach (var voice in Voices.Where(v => v.ID != null))
            //    {
            //        var dbVoice = db.Voices.Single(v => v.ID.ToString() == voice.ID);
            //        buffer.AddRange(dbVoice.Data.ToArray().Skip(80));
            //    }
            //    if (buffer.Count > 0)
            //    {
            //        playingSound = new Sound(buffer.ToArray());
            //        playingSound.StateChanged += new Sound.StateChangedHandler(playingSound_StateChanged);
            //        playingSound.Play();
            //    }
            //}
            //else
            //    playingSound.Stop();
        }

        //void playingSound_StateChanged(object sender, SoundState preState)
        //{
        //    if (playingSound.State == SoundState.Stopped)
        //    {
        //        (playButton.Content as Image).Source = ImageSourceExtension.GetImageSource("../../play16.png");
        //        playButton.ToolTip = "پخش پيام";
        //        playingSound = null;
        //    }
        //    else if (playingSound.State == SoundState.Playing)
        //    {
        //        playButton.ToolTip = "توقف";
        //        (playButton.Content as Image).Source = ImageSourceExtension.GetImageSource("../../stop16.png");
        //    }
        //}

        private void ListView_Unloaded(object sender, RoutedEventArgs e)
        {
            //if (playingSound != null && playingSound.State != SoundState.Stopped)
            //{
            //    playingSound.Stop();
            //}
        }

        private void ListView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                RemoveVoice();
            }
        }

        public bool IsForInfoTable
        {
            get;
            set;
        }

    }
}
