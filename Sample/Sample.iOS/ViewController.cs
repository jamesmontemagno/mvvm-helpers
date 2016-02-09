using System;

using UIKit;
using Sample.Shared.ViewModels;
using CoreGraphics;

namespace Sample.iOS
{
    public partial class ViewController : UIViewController
    {
        HomeViewModel viewModel;

        public ViewController(IntPtr handle)
            : base(handle)
        {
            viewModel = new HomeViewModel();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            TickView.Layer.AnchorPoint = new CGPoint(0.5, 1);
            // Perform any additional setup after loading the view, typically from a nib.
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            viewModel.WatchProperty<DateTime>(nameof(viewModel.SignalTime), SignalTimeChanged);
        }

        public override void ViewDidDisappear(bool animated)
        {
            viewModel.ClearWatchers();
            base.ViewDidDisappear(animated);
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }

        void SignalTimeChanged(DateTime time)
        {
            InvokeOnMainThread(() =>
                {
                    TimeLabel.Text = time.ToLongTimeString();
                    UIView.Animate(0.5, () => {
                    nfloat degrees = ((360 / 60) * time.Second); //the value in degrees

                    var transform = CGAffineTransform.MakeRotation(degrees * (nfloat)Math.PI / 180f);

                    TickView.Transform = transform;
                    });
                });
        }
    }
}

