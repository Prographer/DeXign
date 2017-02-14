using System;
using System.ComponentModel;
using System.Windows.Threading;

namespace DeXign.Models
{
    public class Ticker : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public DateTime Now => DateTime.Now;

        DispatcherTimer timer;

        public Ticker()
        {
            timer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromMilliseconds(500)
            };

            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Now)));
        }
    }
}