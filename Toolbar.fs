namespace RadioClock

open System
open System.Drawing
open MonoTouch.UIKit

type Toolbar(rect) as this = 
    inherit UIToolbar(rect: RectangleF)

    let r = new Radio()
    let clickAction(eventArgs: EventArgs): Unit = Console.WriteLine "Clicked!"
    let btn = new UIBarButtonItem ()

    do btn.Image <- new UIImage("timer-32.png")
       btn.Style <- UIBarButtonItemStyle.Plain
       btn.Clicked.Add(clickAction)

       this.Frame <- new RectangleF(0.0f, rect.Bottom - 44.0f, rect.Width, 44.0f)
       this.Items <- [|btn|]




