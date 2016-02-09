using System;
using MvvmHelpers;
using System.Timers;

namespace Sample.Shared.ViewModels
{
    public class HomeViewModel 
        : BaseViewModel
    {
        Timer timer;


        DateTime signalTime;
        public DateTime SignalTime
        {
            get
            {
                return signalTime;
            }
            set
            {
                SetProperty(ref signalTime, value);
            }
        }

        public HomeViewModel()
        {
            timer = new Timer(1000);
            timer.Elapsed += TimerElapsed;
            timer.AutoReset = true;
            timer.Start();
        }

        void TimerElapsed(object sender, ElapsedEventArgs e) => SignalTime = e.SignalTime;
    }
}

