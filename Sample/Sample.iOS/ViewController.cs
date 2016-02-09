using System;

using UIKit;
using Sample.Shared.ViewModels;
using CoreGraphics;

namespace Sample.iOS
{
    public partial class ViewController : UIViewController
    {
        HomeViewModel viewModel;
        UIView minuteHand;
        UIView secondHand;

        public ViewController(IntPtr handle)
            : base(handle)
        {
            viewModel = new HomeViewModel();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            secondHand = CreateHand(false);
            minuteHand = CreateHand(true);

           secondHand.Layer.AnchorPoint = new CGPoint(0.5, 1);
           minuteHand.Layer.AnchorPoint = new CGPoint(0.5, 1);

            Add(secondHand);
            Add(minuteHand);

        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            viewModel.WatchProperty<DateTime>(nameof(viewModel.SignalTime), SignalTimeChanged, t => (t.Second % 5) == 0);

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
                UIView.Animate(0.5, () =>
                    {
                        TimeLabel.Text = time.ToLongTimeString();
                        nfloat secondDegrees = ((360 / 60) * time.Second);
                        //the value in degrees
                        secondHand.Transform = CGAffineTransform.MakeRotation(secondDegrees * (nfloat)Math.PI / 180f);
                        nfloat minuteDegrees = ((360 / 60) * time.Minute);
                        //the value in degrees
                        minuteHand.Transform = CGAffineTransform.MakeRotation(minuteDegrees * (nfloat)Math.PI / 180f);
                    }));
        }

        UIView CreateHand(bool minute)
        {
            var width = View.Frame.Width * (minute ? 0.03 : 0.01);
            var height = View.Frame.Height * (minute ? 0.2 : 0.4); 

            var view = new UIView(new CGRect((View.Bounds.Width / 2) - (width / 2), (View.Bounds.Height / 2 - height) + (minute ? 0 : height * .25), width, height));
            view.Layer.ShadowOffset = new CGSize(0.0, 5.0);
            view.Layer.ShadowOpacity = 0.8f;
            view.Layer.ShadowRadius = 4f;
            view.Layer.ShadowColor = UIColor.LightGray.CGColor;
            view.BackgroundColor = minute ? UIColor.Blue : UIColor.Green;

            return view;
        }

    }
}

