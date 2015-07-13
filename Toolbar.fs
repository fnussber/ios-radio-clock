namespace RadioClock

open System
open System.Drawing
open MonoTouch.UIKit
open MonoTouch.Foundation

type Toolbar(clock: Clock) as this = 
    inherit UIToolbar()

    let addButton(image: string, handler) =
        let btn = new UIBarButtonItem ()
        btn.Image <- new UIImage(image)
        btn.Style <- UIBarButtonItemStyle.Plain
        btn.Clicked.Add(handler)
        btn

    do 
        let btn0 = addButton("timer-32.png", this.SetTimer)
        let btn1 = addButton("alarm_clock-32.png", this.SetAlarm)
        let btn2 = addButton("radio-32.png", this.ToggleRadio)

        // make toolbar transparent
        this.SetBackgroundImage(new UIImage(), UIToolbarPosition.Any, UIBarMetrics.Default)
        this.SetShadowImage(new UIImage(), UIToolbarPosition.Any)

        this.TranslatesAutoresizingMaskIntoConstraints <- false  // important for auto layout!
        this.Items <- [|btn0; btn1; btn2|]


    member this.SetTimer(eventArgs: EventArgs): Unit =
        Console.WriteLine("set timer")

    member this.SetAlarm(eventArgs: EventArgs): Unit =
        let types = UIUserNotificationType.Alert
        let settings = UIUserNotificationSettings.GetSettingsForTypes(types, null)
        UIApplication.SharedApplication.RegisterUserNotificationSettings(settings)
        if clock.IsStopped() then 
            Alarm.Start(new TimeSpan())
            clock.StopBlink()
            clock.Start() 
        else 
            clock.Stop()
            clock.Blink()

    member this.ToggleRadio(eventArgs: EventArgs): Unit =
        if Radio.IsPlaying() then Radio.Stop() else Radio.Play()



