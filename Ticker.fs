namespace RadioClock

open System
open System.Drawing
open MonoTouch.CoreAnimation
open MonoTouch.CoreGraphics
open MonoTouch.Foundation
open MonoTouch.UIKit

type Ticker(event: Event<string>, inSpeed: float, outSpeed: float) as t = 
    inherit UIView()

    let mutable message = new UIView()

    let scrollOut() =
        UIView.Animate(outSpeed, 0.0, UIViewAnimationOptions.CurveLinear, new NSAction(fun () -> message.Center <- new PointF (message.Center.X - 1000.0f, message.Center.Y)), null)

    let scrollIn() =
        let center = message.Center
        message.Center <- new PointF (message.Center.X + 1000.0f, message.Center.Y)
        UIView.Animate(inSpeed, 0.0, UIViewAnimationOptions.CurveLinear, new NSAction(fun () -> message.Center <- center), null)

    let nextMessage(msg) =
        t.InvokeOnMainThread(new NSAction(fun _ ->
            scrollOut()
            // replace current message with new one, relayout everything
            message.RemoveFromSuperview()
            message <- new UILabel(Text = msg)
            message.TranslatesAutoresizingMaskIntoConstraints <- false // important for auto layout
            t.AddSubview(message)
            let metrics = new NSDictionary()
            let views = new NSDictionary("m", message)
            let v = NSLayoutConstraint.FromVisualFormat("V:|[m]|", NSLayoutFormatOptions.DirectionLeadingToTrailing, metrics, views) 
            t.AddConstraints(v)
            scrollIn()
        ))   
    do
        t.TranslatesAutoresizingMaskIntoConstraints <- false  // important for auto layout!
        t.BackgroundColor <- UIColor.White
        Event.add (fun s -> Console.WriteLine("next"); nextMessage(s)) event.Publish
 
