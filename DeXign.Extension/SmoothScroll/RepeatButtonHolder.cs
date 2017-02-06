using System;
using System.Windows.Controls.Primitives;

namespace DeXign.Extension
{
    class RepeatButtonHolder
    {
        public event EventHandler OnClick;

        public RepeatButton Target { get; }
        public DateTime BeginTime { get; private set; }

        public double Duration
        {
            get
            {
                return (DateTime.Now - BeginTime).TotalMilliseconds;
            }
        }

        public bool IsBlocked
        {
            get
            {
                return Duration < Target.Delay;
            }
        }

        private DateTime nextRaiseTime;

        public RepeatButtonHolder(RepeatButton button)
        {
            Target = button;
        }

        public void Start()
        {
            BeginTime = DateTime.Now;
            nextRaiseTime = BeginTime.AddMilliseconds(Target.Delay + Target.Interval);

            OnClick?.Invoke(this, null);
        }

        public void Update()
        {
            if (nextRaiseTime < DateTime.Now)
            {
                OnClick?.Invoke(this, null);
                nextRaiseTime = DateTime.Now.AddTicks(Target.Interval);
            }
        }
    }
}
