namespace RadioClock

open System
open System.Drawing
open MonoTouch.CoreAnimation
open MonoTouch.CoreGraphics
open MonoTouch.Foundation
open MonoTouch.UIKit

type Message() as m =
    inherit UILabel()

    do
        m.TranslatesAutoresizingMaskIntoConstraints <- false  // important for auto layout!
        //m.Font <- font
        //m.Text <- digit
        m.TextAlignment <- UITextAlignment.Left
        //m.BackgroundColor <- new UIColor(0.0f, 0.0f, 0.0f, 0.0f)
        m.TextColor <- UIColor.Black

type Ticker(msgStream: seq<string>) as t = 
    inherit UIView()

    let message = new Message()

    let msgStreamEnum = msgStream.GetEnumerator()

    let nextMessage() = if msgStreamEnum.MoveNext() then msgStreamEnum.Current else "EOF"

    let rec scrollOut() =
        UIView.Animate(2.0, 2.0, UIViewAnimationOptions.CurveLinear, new NSAction(fun () -> message.Center <- new PointF (message.Center.X - 1000.0f, message.Center.Y)), new NSAction(fun () -> scrollIn()))

    and scrollIn() =
        // set text and relayout so that we get the text at the right position (depending on text length etc)
        message.Text <- nextMessage()
        t.SetNeedsLayout()
        t.LayoutIfNeeded()
        let center = message.Center
        message.Center <- new PointF (message.Center.X + 1000.0f, message.Center.Y)
        UIView.Animate(2.0, 0.0, UIViewAnimationOptions.CurveLinear, new NSAction(fun () -> message.Center <- center), new NSAction(fun () -> scrollOut()))

    do
        t.TranslatesAutoresizingMaskIntoConstraints <- false  // important for auto layout!
        t.BackgroundColor <- UIColor.White
        t.AddSubview(message)
        let metrics = new NSDictionary()
        let views = new NSDictionary("m", message)
        let h = NSLayoutConstraint.FromVisualFormat("H:|[m]|", NSLayoutFormatOptions.DirectionLeadingToTrailing, metrics, views) 
        let v = NSLayoutConstraint.FromVisualFormat("V:|[m]|", NSLayoutFormatOptions.DirectionLeadingToTrailing, metrics, views) 
        t.AddConstraints(h)
        t.AddConstraints(v)
        scrollIn() // start consuming messages..

