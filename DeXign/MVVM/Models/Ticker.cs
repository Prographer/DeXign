using System;
using System.Windows.Threading;

namespace DeXign.Models
{
    public class Ticker : BaseNotifyModel
    {
        public DateTime Now => DateTime.Now;

        DispatcherTimer timer;

        public Ticker() : base()
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
            RaisePropertyChanged(nameof(Now));
        }
    }
}