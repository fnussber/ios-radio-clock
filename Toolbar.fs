namespace RadioClock

open System
open System.Drawing
open MonoTouch.UIKit

type Toolbar(clock: Clock) as this = 
    inherit UIToolbar()

    let r = new Radio()
    let btn = new UIBarButtonItem ()

    do 
        btn.Image <- new UIImage("timer-32.png")
        btn.Style <- UIBarButtonItemStyle.Plain
        btn.Clicked.Add(this.SetAlarm)

        this.TranslatesAutoresizingMaskIntoConstraints <- false  // important for auto layout!
        this.Items <- [|btn|]

    member this.SetAlarm(eventArgs: EventArgs): Unit =
         Console.WriteLine "Clicked!"
         if clock.IsStopped() then 
             clock.StopBlink()
             clock.Start() 
         else 
             clock.Stop()
             clock.Blink()



