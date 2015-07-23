namespace RadioClock

open System
open MonoTouch.Foundation
open MonoTouch.UIKit

// Deal with two types of alarms: a sleep timer, which will turn the
// radio off after a given time, and the actual alarm clock.
module Alarm = 

    let mutable timer: option<UILocalNotification> = None
    let mutable alarm: option<UILocalNotification> = None

    let sleepIco = Layout.icon Layout.SleepIcon
    let alarmIco = Layout.icon Layout.AlarmIcon
    let sleepLbl = Layout.label "" Layout.TinyFont
    let alarmLbl = Layout.label "" Layout.TinyFont
    let alarmRem = Layout.label "" Layout.TinyFont

    let StatusBar : UIView =
        let view = new UIView(TranslatesAutoresizingMaskIntoConstraints = false)

        let views = [
            "sleepIco", sleepIco :> UIView
            "sleepLbl", sleepLbl :> UIView
            "alarmIco", alarmIco :> UIView
            "alarmLbl", alarmLbl :> UIView
            "alarmRem", alarmRem :> UIView
        ]   
        let formats = [
            "V:|[sleepIco]"
            "V:|[sleepLbl]|"
            "V:|[alarmIco]"
            "V:|[alarmLbl][alarmRem]|"
            "H:|[sleepIco]-[sleepLbl]-[alarmIco]-[alarmLbl]|"
            "H:[alarmRem]|"
        ]
        Layout.layout view formats views
        
        view

    let cancelNotification maybeNotification =
        match maybeNotification with
            | Some n -> UIApplication.SharedApplication.CancelLocalNotification(n)
            | None   -> ()

    let cancelAlarm () =
        cancelNotification alarm
        alarmIco.Hidden <- true
        alarmLbl.Hidden <- true
        alarmRem.Hidden <- true
        alarm <- None 

    let cancelTimer () =
        cancelNotification timer
        sleepIco.Hidden <- true
        sleepLbl.Hidden <- true
        timer <- None 

    let createNotification nsdate =
        let notif  = new UILocalNotification(FireDate = nsdate)
        notif.AlertAction <- "Wecki, wecki"
        notif.AlertBody <- "Hey, an alert went off"
        UIApplication.SharedApplication.ScheduleLocalNotification(notif)
        notif

    let setAlarm2 alarmTime =
        // TODO: split up in several functions, lock on alarm(needed?)
        let time   = System.DateTime.Now//.UtcNow
        let span   = TimeSpan(time.Hour, time.Minute, time.Second)
        let alert  = if (span.CompareTo(alarmTime)) < 0 then time.Date.AddTicks(alarmTime.Ticks) else time.Date.AddDays(1.0).AddTicks(alarmTime.Ticks)
        let alert2 = DateTime.SpecifyKind(alert, DateTimeKind.Utc)
        let reference = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(2001, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc))
//        let reference = new DateTime(2001, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)
        // TODO: find a better way, how do we deal with local time vs utc here??? need to subtract two hours for time zone and daylight savings
        NSDate.FromTimeIntervalSinceReferenceDate((alert2 - reference).TotalSeconds).AddSeconds(-1.0*3600.0)

    let remainingTime (n: UILocalNotification) =
        let s = n.FireDate.SecondsSinceReferenceDate - (new NSDate()).SecondsSinceReferenceDate
        TimeSpan(0, 0, int(s))

    let updateRemaining (l: UILabel) n =
        match n with
            | Some(n) ->
                l.InvokeOnMainThread(fun _ -> 
                    l.Text <- (remainingTime n).ToString()
                )
            | None    -> ()

    let setAlarm alarmTime =
        if (not (List.exists (fun (t: TimeSpan) -> alarmTime.Equals(t)) Config.alarmTimes)) then Config.alarmTimes <-  Seq.take 3 (alarmTime :: Config.alarmTimes) |> Seq.toList
        alarmIco.Hidden <- false
        alarmLbl.Hidden <- false
        alarmRem.Hidden <- false
        alarmLbl.Text   <- alarmTime.ToString()
        alarm <- Some(createNotification (setAlarm2 alarmTime))
        updateRemaining alarmRem alarm

    let setTimer (timerTime: TimeSpan) =
        sleepIco.Hidden <- false
        sleepLbl.Hidden <- false
        timer <- Some(createNotification ((new NSDate()).AddSeconds(timerTime.TotalSeconds)))
        updateRemaining sleepLbl timer

    // Handle incoming system notifications
    let handleNotification (incoming: UILocalNotification) =
//        UIApplication.SharedApplication.IdleTimerDisabled <- false
        if (alarm.IsSome && alarm.Value.FireDate.IsEqual(incoming.FireDate)) then 
            cancelAlarm()
            Radio.fadeIn()
            UIScreen.MainScreen.Brightness <- 1.0f // reset original value!
        if (timer.IsSome && timer.Value.FireDate.IsEqual(incoming.FireDate)) then 
            cancelTimer()
            Radio.fadeOut()
            UIScreen.MainScreen.Brightness <- 0.0f

    do 
        // init: deactive/hide sleep and alarm UI elements
        cancelAlarm()
        cancelTimer()

        // hide/show sleep state
        Toolbar.timerButton.Add (fun maybeSpan ->
            cancelTimer()
            match maybeSpan with
                | Some(span) -> setTimer span
                | None       -> ()
        )

        // hide/show alarm state
        Toolbar.alarmButton.Add (fun maybeSpan ->
            cancelAlarm()
            match maybeSpan with
                | Some(span) -> setAlarm span
                | None       -> ()
        )

        Toolbar.dayModeSelected.Add (fun _ ->
            UIScreen.MainScreen.Brightness <- 1.0f
        )

        Toolbar.nightModeSelected.Add (fun _ ->
            UIScreen.MainScreen.Brightness <- 0.0f
        )

        // install a timer that updates the remaining time labels
        let ttt = new System.Timers.Timer(1000.0)
        ttt.Elapsed.Add(fun _ -> 
            updateRemaining sleepLbl timer
            updateRemaining alarmRem alarm
        )
        ttt.Start()

            


