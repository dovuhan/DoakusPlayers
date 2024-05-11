using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace MediaPlayer
{
    public partial class MainWindow : Window
    {
        private List<string> playlist = new List<string>();
        private DispatcherTimer timer;

        public MainWindow()
        {
            InitializeComponent();
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
        }

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Media files (*.mp4;*.mp3;*.wav)|*.mp4;*.mp3;*.wav|All files (*.*)|*.*";
            openFileDialog.Multiselect = true;
            if (openFileDialog.ShowDialog() == true)
            {
                foreach (var file in openFileDialog.FileNames)
                {
                    playlist.Add(file);
                }
                playlistBox.ItemsSource = null;
                playlistBox.ItemsSource = playlist;
            }
        }

        private void CloseFile_Click(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Stop();
            mediaPlayer.Source = null;
            playlist.Clear();
            playlistBox.ItemsSource = null;
        }

        private void PlaylistBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (playlistBox.SelectedItem != null)
            {
                mediaPlayer.Source = new Uri(playlistBox.SelectedItem.ToString());
                mediaPlayer.Play();
                timer.Start();
            }
        }

        private void Play_Click(object sender, RoutedEventArgs e)
        {
            if (mediaPlayer.Source != null)
            {
                mediaPlayer.Play();
                timer.Start();
            }
        }

        private void Pause_Click(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Pause();
            timer.Stop();
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Stop();
            mediaPlayer.Position = TimeSpan.Zero;
            timer.Stop();
            progressSlider.Value = 0;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (mediaPlayer.NaturalDuration.HasTimeSpan)
            {
                progressSlider.Maximum = mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds;
                progressSlider.Value = mediaPlayer.Position.TotalSeconds;
            }
        }

        private void ProgressSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (Math.Abs(mediaPlayer.Position.TotalSeconds - progressSlider.Value) > 1)
            {
                mediaPlayer.Position = TimeSpan.FromSeconds(progressSlider.Value);
            }
        }
    }
}
