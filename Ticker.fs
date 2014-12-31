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

type Ticker() as t = 
    inherit UIView()

    let message = new Message()

    let scrollOut() =
        UIView.Animate(5.0, 5.0, UIViewAnimationOptions.CurveLinear, new NSAction(fun () -> message.Center <- new PointF (message.Center.X - 1000.0f, message.Center.Y)), null)

    let scrollIn() =
        let center = message.Center
        message.Center <- new PointF (message.Center.X + 1000.0f, message.Center.Y)
        UIView.Animate(5.0, 0.0, UIViewAnimationOptions.CurveLinear, new NSAction(fun () -> message.Center <- center), new NSAction(fun () -> scrollOut()))

    let animate text = 
        message.Text <- text
        scrollIn()

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

    member t.Text with set(text) = animate(text)

