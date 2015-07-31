namespace RadioClock

open System
open System.Drawing
open MonoTouch.CoreAnimation
open MonoTouch.CoreGraphics
open MonoTouch.Foundation
open MonoTouch.UIKit

module Clock =

    let face  = new UIView()
    let timer = new System.Timers.Timer(1000.0)

    let hh        = new DoubleDigit(24)
    let c1        = Layout.label ":" Layout.bigFont
    let mm        = new DoubleDigit(60)
    let c2        = Layout.label ":" Layout.bigFont
    let ss        = new DoubleDigit(60)

    let setBtn    = Layout.button "Set"    UIColor.Green
    let cancelBtn = Layout.button "Cancel" UIColor.Red

    let time () =
        new TimeSpan(hh.value(), mm.value(), ss.value())

    let timeHHMM () =
        new TimeSpan(hh.value(), mm.value(), 0)

    let isStopped () =
        not timer.Enabled

    let start () =
        timer.Start()

    let stop () =
        timer.Stop()

    let blink () =
        hh.blink()
        mm.blink()
        ss.blink()
        face.SetNeedsLayout()
        face.LayoutIfNeeded()

    let stopBlink () =
        hh.stopBlink()
        mm.stopBlink()
        ss.stopBlink()
        face.SetNeedsLayout()
        face.LayoutIfNeeded()

    let pulse () =
        let now = System.DateTime.Now
        hh.next(now.Hour)
        mm.next(now.Minute)
        ss.next(now.Second)

    let showButtons show =
        setBtn.Hidden    <- not show
        cancelBtn.Hidden <- not show
        ss.Hidden        <- show

    let startUserAlarm () = 
        let types = UIUserNotificationType.Alert
        let settings = UIUserNotificationSettings.GetSettingsForTypes(types, null)
        UIApplication.SharedApplication.RegisterUserNotificationSettings(settings)
        hh.next(Config.alarmTimes.Head.Hours)
        mm.next(Config.alarmTimes.Head.Minutes)
        ss.next(Config.alarmTimes.Head.Seconds)
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
            "c1", c1 :> UIView
            "mm", mm :> UIView
            "c2", c2 :> UIView
            "ss", ss :> UIView
            "set", setBtn :> UIView
            "cancel", cancelBtn :> UIView
        ]
        let formats = [
            "H:|[hh]-[c1(25)]-[mm(==hh)]-[c2(25)]-[ss(==hh)]|"
            "H:[set(150)]|"
            "H:[cancel(150)]|"
            "V:|[hh]|"
            "V:|[c1]|"
            "V:|[mm]|"
            "V:|[c2]|"
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

        // start timer
        start()

