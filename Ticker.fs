namespace RadioClock

open System
open System.Drawing
open MonoTouch.CoreAnimation
open MonoTouch.CoreGraphics
open MonoTouch.Foundation
open MonoTouch.UIKit

type Ticker(msgStream: seq<UIView>) as t = 
    inherit UIView()

    let mutable message = new UIView()

    let msgStreamEnum = msgStream.GetEnumerator()

    let nextMessage() = if msgStreamEnum.MoveNext() then msgStreamEnum.Current else (new UILabel(Text = "EOF") :> UIView)

    let nextView() =
        // replace current message with new one, relayout everything
        message.RemoveFromSuperview()
        message <- nextMessage()
        message.TranslatesAutoresizingMaskIntoConstraints <- false // important for auto layout
        t.AddSubview(message)
        let metrics = new NSDictionary()
        let views = new NSDictionary("m", message)
        let h = NSLayoutConstraint.FromVisualFormat("H:|[m]|", NSLayoutFormatOptions.DirectionLeadingToTrailing, metrics, views) 
        let v = NSLayoutConstraint.FromVisualFormat("V:|[m]|", NSLayoutFormatOptions.DirectionLeadingToTrailing, metrics, views) 
        t.AddConstraints(h)
        t.AddConstraints(v)


    let rec scrollOut() =
        UIView.Animate(0.5, 3.0, UIViewAnimationOptions.CurveLinear, new NSAction(fun () -> message.Center <- new PointF (message.Center.X - 1000.0f, message.Center.Y)), new NSAction(fun () -> scrollIn()))

    and scrollIn() =
        nextView()
        t.SetNeedsLayout()
        t.LayoutIfNeeded()
        let center = message.Center
        message.Center <- new PointF (message.Center.X + 1000.0f, message.Center.Y)
        UIView.Animate(0.5, 0.0, UIViewAnimationOptions.CurveLinear, new NSAction(fun () -> message.Center <- center), new NSAction(fun () -> scrollOut()))

    do
        t.TranslatesAutoresizingMaskIntoConstraints <- false  // important for auto layout!
        t.BackgroundColor <- UIColor.White
        scrollIn() // start consuming messages..

