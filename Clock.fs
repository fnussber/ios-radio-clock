namespace RadioClock

open System
open System.Drawing
open MonoTouch.CoreAnimation
open MonoTouch.CoreGraphics
open MonoTouch.Foundation
open MonoTouch.UIKit

module Clock =

    let mutable stopped = true

    let face  = new UIView()
    let timer = new System.Timers.Timer(1000.0)

    let hh = new TwoDigits(24)
    let mm = new TwoDigits(60)
    let ss = new TwoDigits(60)

    let time() =
        new TimeSpan(hh.Value, mm.Value, ss.Value)

    let timeHHMM() =
        new TimeSpan(hh.Value, mm.Value, 0)

    let isStopped() =
        stopped

    let start() =
        timer.Start()   // sync needed?
        stopped <- false

    let stop() =
        timer.Stop()
        stopped <- true

    let blink() =
        hh.Blink()
        mm.Blink()
        ss.Blink()
        face.SetNeedsLayout()
        face.LayoutIfNeeded()

    let stopBlink() =
        hh.StopBlink()
        mm.StopBlink()
        ss.StopBlink()
        face.SetNeedsLayout()
        face.LayoutIfNeeded()

    let pulse() =
        let now = System.DateTime.Now
        hh.Next(now.Hour)
        mm.Next(now.Minute)
        ss.Next(now.Second)

    let is24Hours() =
        let format = NSDateFormatter.GetDateFormatFromTemplate("j", uint32(0), NSLocale.CurrentLocale)
        format.IndexOf('a') = -1

    let userAlarm() = 
        let types = UIUserNotificationType.Alert
        let settings = UIUserNotificationSettings.GetSettingsForTypes(types, null)
        UIApplication.SharedApplication.RegisterUserNotificationSettings(settings)
        if isStopped() then 
            Alarm.setAlarm(time())
            stopBlink()
            start() 
        else 
            stop()
            blink()

    do
        face.BackgroundColor <- UIColor.Clear
        face.TranslatesAutoresizingMaskIntoConstraints <- false  // important for auto layout!

        let views = [
            "hh", hh :> UIView
            "mm", mm :> UIView
            "ss", ss :> UIView
        ]
        let formats = [
            "H:|[hh]-25-[mm(==hh)]-25-[ss(==mm)]|"
            "V:|[hh]|"
            "V:|[mm]|"
            "V:|[ss]|"
        ]

        Layout.layout face formats views

        // install a timer which will trigger an update of the clock
        timer.Elapsed.Add(fun _ -> pulse())

        // register for toolbar interactions
        Toolbar.usralEvent.Publish.Add(fun _ -> userAlarm())

