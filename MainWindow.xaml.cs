using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Media;

namespace wpf_timer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Timer : Window
    {
        enum State { Stopped, Started };

        SoundPlayer sound = new SoundPlayer(Properties.Resources.timerSound);

        DispatcherTimer dispatcherTimer = new DispatcherTimer();

        State state = State.Stopped;

        private short hours, minutes, seconds;

        public Timer()
        {
            InitializeComponent();

            textBox_hours.Focus();
            text_timeLeft.TextAlignment = TextAlignment.Center;
            dispatcherTimer.Tick += new EventHandler(UpdateTimeLeft);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
        }
        private void UpdateTimeLeft(object sender, EventArgs e)
        {
            if (hours > 0 || minutes > 0 || seconds > 0)
            {
                seconds--;
                if (seconds < 0)
                {
                    minutes--;
                    seconds += 60;
                }
                if (minutes < 0)
                {
                    hours--;
                    minutes += 60;
                }
            }
            else
            {
                sound.PlayLooping();
                button.Content = "Stop";
            }
            UpdateTimeLeftText();
        }
        private void UpdateTimeLeftText()
        {
            text_timeLeft.Text =
                ((hours > 9) ? (hours.ToString()) : ('0' + hours.ToString())) + ':' +
                ((minutes > 9) ? (minutes.ToString()) : ('0' + minutes.ToString())) + ':' +
                ((seconds > 9) ? (seconds.ToString()) : ('0' + seconds.ToString()));
        }
        private void CorrectTextInput(TextBox textBox)
        {
            if (textBox.Text.Length > 0)
            {
                if (textBox.Text.Last() < 48 && textBox.Text.Last() > 57)
                {
                    textBox.Text = textBox.Text.Remove(textBox.Text.Length - 1);
                    textBox.CaretIndex = textBox.Text.Length;
                }
                if (textBox.Text.Length > 1)
                    textBox.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }
            button.Content = "Start";
            text_timeLeft.Text = "00:00:00";
        }
        private void HandleTextInput_hours(object sender, TextChangedEventArgs e)
        {
            CorrectTextInput(textBox_hours);
        }
        private void HandleTextInput_minutes(object sender, TextChangedEventArgs e)
        {
            CorrectTextInput(textBox_minutes);
        }
        private void HandleTextInput_seconds(object sender, TextChangedEventArgs e)
        {
            CorrectTextInput(textBox_seconds);
        }
        private void HandleClick_button(object sender, RoutedEventArgs e)
        {
            switch (state)
            {
                case State.Stopped:
                    if (button.Content.ToString() == "Start")
                    {
                        if (textBox_hours.Text == "") textBox_hours.Text = "00";
                        if (textBox_minutes.Text == "") textBox_minutes.Text = "00";
                        if (textBox_seconds.Text == "") textBox_seconds.Text = "00";
                        if (textBox_hours.Text.Length <= 1) textBox_hours.Text = textBox_hours.Text.Insert(0, "0");
                        if (textBox_minutes.Text.Length <= 1) textBox_minutes.Text = textBox_minutes.Text.Insert(0, "0");
                        if (textBox_seconds.Text.Length <= 1) textBox_seconds.Text = textBox_seconds.Text.Insert(0, "0");
                        hours = short.Parse(textBox_hours.Text);
                        minutes = short.Parse(textBox_minutes.Text);
                        seconds = short.Parse(textBox_seconds.Text);
                        UpdateTimeLeftText();
                    }
                    textBox_hours.IsEnabled = false;
                    textBox_minutes.IsEnabled = false;
                    textBox_seconds.IsEnabled = false;
                    button.Content = "Pause";
                    dispatcherTimer.Start();
                    state = State.Started;
                    break;
                case State.Started:
                    textBox_hours.IsEnabled = true;
                    textBox_minutes.IsEnabled = true;
                    textBox_seconds.IsEnabled = true;
                    if (button.Content.ToString() == "Stop")
                        button.Content = "Start";
                    else
                        button.Content = "Continue";
                    dispatcherTimer.Stop();
                    sound.Stop();
                    state = State.Stopped;
                    break;
            }
        }
        private void HandleWindowSizeChanged(object sender, SizeChangedEventArgs e)
        {
            viewBox_timeLeft.Width = window.Width;
            viewBox_timeLeft.Height = Math.Max(window.Height - 130, 0);
        }
    }
}