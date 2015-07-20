namespace RadioClock

open System
open System.Drawing
open MonoTouch.CoreAnimation
open MonoTouch.CoreGraphics
open MonoTouch.Foundation
open MonoTouch.UIKit

module Clock =

    let mutable stopped = true // can we just pause the timer instead?

    let face  = new UIView()
    let timer = new System.Timers.Timer(1000.0)

    let hh = new TwoDigits(24)
    let mm = new TwoDigits(60)
    let ss = new TwoDigits(60)

    let setBtn = UIButton.FromType(UIButtonType.RoundedRect)
    setBtn.TranslatesAutoresizingMaskIntoConstraints <- false
    setBtn.SetTitle("Set", UIControlState.Normal)
    setBtn.BackgroundColor <- UIColor.Green
    setBtn.SetTitleColor(UIColor.White, UIControlState.Normal)
//    setBtn.SetTitleShadowColor(UIColor.Black, UIControlState.Normal)
    setBtn.Font <- Layout.smallFont

    let cancelBtn = UIButton.FromType(UIButtonType.RoundedRect)
    cancelBtn.TranslatesAutoresizingMaskIntoConstraints <- false
    cancelBtn.SetTitle("Cancel", UIControlState.Normal)
    cancelBtn.BackgroundColor <- UIColor.Red
    cancelBtn.SetTitleColor(UIColor.White, UIControlState.Normal)
//    cancelBtn.SetTitleShadowColor(UIColor.Black, UIControlState.Normal)
    cancelBtn.Font <- Layout.smallFont

    let time () =
        new TimeSpan(hh.Value, mm.Value, ss.Value)

    let timeHHMM () =
        new TimeSpan(hh.Value, mm.Value, 0)

    let isStopped () =
        stopped

    let start () =
        timer.Start()   // sync needed?
        stopped <- false

    let stop () =
        timer.Stop()
        stopped <- true

    let blink () =
        hh.Blink()
        mm.Blink()
        ss.Blink()
        face.SetNeedsLayout()
        face.LayoutIfNeeded()

    let stopBlink () =
        hh.StopBlink()
        mm.StopBlink()
        ss.StopBlink()
        face.SetNeedsLayout()
        face.LayoutIfNeeded()

    let pulse () =
        let now = System.DateTime.Now
        hh.Next(now.Hour)
        mm.Next(now.Minute)
        ss.Next(now.Second)

    let is24Hours () =
        let format = NSDateFormatter.GetDateFormatFromTemplate("j", uint32(0), NSLocale.CurrentLocale)
        format.IndexOf('a') = -1

    let showButtons show =
        setBtn.Hidden    <- not show
        cancelBtn.Hidden <- not show
        ss.Hidden        <- show

    let startUserAlarm () = 
        let types = UIUserNotificationType.Alert
        let settings = UIUserNotificationSettings.GetSettingsForTypes(types, null)
        UIApplication.SharedApplication.RegisterUserNotificationSettings(settings)
        hh.Next(Config.alarmTimes.Head.Hours)
        mm.Next(Config.alarmTimes.Head.Minutes)
        ss.Next(Config.alarmTimes.Head.Seconds)
        showButtons true
        stop()
        blink()

    let stopUserAlarm() = 
        stopBlink()
        start() 
        showButtons false

    do
        face.BackgroundColor <- UIColor.Clear
        face.TranslatesAutoresizingMaskIntoConstraints <- false  // important for auto layout!

        setBtn.Hidden <- true
        cancelBtn.Hidden <- true

        let views = [
            "hh", hh :> UIView
            "mm", mm :> UIView
            "ss", ss :> UIView
            "set", setBtn :> UIView
            "cancel", cancelBtn :> UIView
        ]
        let formats = [
            "H:|[hh]-25-[mm(==hh)]-25-[ss(==hh)]|"
//            "H:|[hh]-25-[mm(==hh)][set(==hh)]|"
//            "H:|[hh]-25-[mm(==hh)][cancel(==hh)]|"
            "H:[set(150)]|"
            "H:[cancel(150)]|"
            "V:|[hh]|"
            "V:|[mm]|"
            "V:|[ss]|"
            "V:|-[set]-[cancel(==set)]-|"
        ]

        Layout.layout face formats views

        // install a timer which triggers an updates the clock digits
        timer.Elapsed.Add(fun _ -> pulse())

        // register toolbar interactions
        Toolbar.userAlarmCreate.Add(fun _ -> startUserAlarm())

        // register handlers for setting alarm
        setBtn.TouchDown.Add(fun _ ->
            Alarm.setAlarm(timeHHMM())
            stopUserAlarm()
        )

        cancelBtn.TouchDown.Add(fun _ ->
            stopUserAlarm()
        )

