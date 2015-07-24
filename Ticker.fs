namespace RadioClock

open System
open System.Drawing
open MonoTouch.CoreAnimation
open MonoTouch.CoreGraphics
open MonoTouch.Foundation
open MonoTouch.UIKit

type Ticker(event: Event<UIView>, inSpeed: float, outSpeed: float) as this = 
    inherit UIView(
        TranslatesAutoresizingMaskIntoConstraints = false,
        BackgroundColor = new UIColor(0.0f, 0.0f, 0.0f, 0.3f)
    )

    let mutable message = new UIView()

    let scrollIn () =
        let center = message.Center
        message.Center <- new PointF (message.Center.X + 1000.0f, message.Center.Y)
        UIView.Animate(inSpeed, 0.0, UIViewAnimationOptions.CurveLinear, (fun () -> message.Center <- center), null)

    let nextMessage msg =
        message.RemoveFromSuperview()
        message.Dispose() // TODO: needed??
        message <- msg

        let views = [
            "m", message
        ]
        let formats = [
            "H:|-20-[m]-20-|"
            "V:|[m]|"
        ]
        Layout.layout this formats views

        scrollIn()

    do
        event.Publish.Add (fun s -> nextMessage(s)) 
 
