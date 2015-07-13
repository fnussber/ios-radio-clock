namespace RadioClock

open System
open System.Drawing
open MonoTouch.CoreAnimation
open MonoTouch.CoreGraphics
open MonoTouch.Foundation
open MonoTouch.UIKit

[<AbstractClass>]
type Ticker(event: Event<string>, inSpeed: float, outSpeed: float) as this = 
    inherit UIView()

    let mutable message = new UIView()

//    let scrollOut() =
//        UIView.Animate(outSpeed, 0.0, UIViewAnimationOptions.CurveLinear, new NSAction(fun () -> message.Center <- new PointF (message.Center.X - 1000.0f, message.Center.Y)), null)

    let scrollIn() =
        let center = message.Center
        message.Center <- new PointF (message.Center.X + 1000.0f, message.Center.Y)
        UIView.Animate(inSpeed, 0.0, UIViewAnimationOptions.CurveLinear, new NSAction(fun () -> message.Center <- center), null)

    do
        this.TranslatesAutoresizingMaskIntoConstraints <- false  // important for auto layout!
        this.BackgroundColor <- new UIColor(0.0f, 0.0f, 0.0f, 0.4f)
        Event.add (fun s -> Console.WriteLine("next"); this.nextMessage(s)) event.Publish
 
    abstract font: UIFont

    member this.nextMessage(msg) =
        this.InvokeOnMainThread(new NSAction(fun _ ->
//            scrollOut()
            let label = new UILabel(TranslatesAutoresizingMaskIntoConstraints = false, Text = msg)
            label.Font <- this.font
            label.TextColor <- UIColor.White
            label.BackgroundColor <- UIColor.Clear
            // replace current message with new one, relayout everything
            message.RemoveFromSuperview()
            message <- label
            this.AddSubview(message)
            let metrics = new NSDictionary()
            let views = new NSDictionary("m", message)
            let h = NSLayoutConstraint.FromVisualFormat("H:|-20-[m]-20-|", NSLayoutFormatOptions.DirectionLeadingToTrailing, metrics, views) 
            let v = NSLayoutConstraint.FromVisualFormat("V:|[m]|", NSLayoutFormatOptions.DirectionLeadingToTrailing, metrics, views) 
            this.AddConstraints(h)
            this.AddConstraints(v)
            scrollIn()
        ))  
 

type HeadlineTicker(event: Event<string>) = 
    inherit Ticker(event, 0.5, 0.5)
    override this.font = UIFont.FromName("Helvetica-Bold", 30.0f)

type DescriptionTicker(event: Event<string>) = 
    inherit Ticker(event, 1.0, 6.0)
    override this.font = UIFont.FromName("Helvetica", 20.0f)

type MetaTicker(event: Event<string>) = 
    inherit Ticker(event, 0.5, 0.5)
    override this.font = UIFont.FromName("Helvetica", 20.0f)

