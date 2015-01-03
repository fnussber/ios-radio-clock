namespace RadioClock

open System
open System.Drawing
open MonoTouch.UIKit
open MonoTouch.Foundation

type Toolbar(clock: Clock) as this = 
    inherit UIToolbar()

    let btn = new UIBarButtonItem ()

    do 
        btn.Image <- new UIImage("timer-32.png")
        btn.Style <- UIBarButtonItemStyle.Plain
        btn.Clicked.Add(this.SetAlarm)

        this.TranslatesAutoresizingMaskIntoConstraints <- false  // important for auto layout!
        this.Items <- [|btn|]

    member this.SetAlarm(eventArgs: EventArgs): Unit =

        let types = UIUserNotificationType.Alert
        let settings = UIUserNotificationSettings.GetSettingsForTypes(types, null)
        UIApplication.SharedApplication.RegisterUserNotificationSettings(settings)

        Console.WriteLine "Clicked!"
        if clock.IsStopped() then 
            Alarm.Start(new TimeSpan())
            clock.StopBlink()
            clock.Start() 
        else 
            clock.Stop()
            clock.Blink()



