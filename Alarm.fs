namespace RadioClock

open System
open MonoTouch.Foundation
open MonoTouch.UIKit


module Alarm = 

    let mutable timer: option<UILocalNotification> = None
    let mutable alarm: option<UILocalNotification> = None

    let sleepIco = Layout.coloredIcon Layout.SleepIcon UIColor.White
    let alarmIco = Layout.coloredIcon Layout.AlarmIcon UIColor.White
    let sleepLbl = Layout.coloredText "00:00" UIColor.White
    let alarmLbl = Layout.coloredText "00:00" UIColor.White

    let StatusBar (): UIView =
        let view = new UIView(TranslatesAutoresizingMaskIntoConstraints = false)

        let views = [
            "sleepIco", sleepIco :> UIView
            "sleepLbl", sleepLbl :> UIView
            "alarmIco", alarmIco :> UIView
            "alarmLbl", alarmLbl :> UIView
        ]   
        let formats = [
            "V:|[sleepIco]"
            "V:|[sleepLbl]"
            "V:|[alarmIco]"
            "V:|[alarmLbl]"
            "H:|[sleepIco]-[sleepLbl(100)]-[alarmIco]-[alarmLbl(100)]|"
        ]
        Layout.layout view formats views
        
        view

    let StartTimer () =
        Console.WriteLine("start timer")
        Radio.Play

//    let CancelTimer () =
//        Console.WriteLine("cancel timer")

//    let DoTimer timer =
//        Console.WriteLine("do timer")
//        removeTimer timer
//        Radio.Stop


    let cancelNotification maybeNotification =
        match maybeNotification with
            | Some n -> 
                UIApplication.SharedApplication.CancelLocalNotification(n)
                UIApplication.SharedApplication.IdleTimerDisabled <- false
            | None   ->
                ()

    let cancelAlarm () =
        cancelNotification alarm
        alarm <- None 

    let cancelTimer () =
        cancelNotification timer
        timer <- None 

    let createNotification nsdate =
        let notif  = new UILocalNotification()
        notif.FireDate <- nsdate
        notif.AlertAction <- "Wecki, wecki!"
        notif.AlertBody <- "Hey, an alert went off."
        UIApplication.SharedApplication.IdleTimerDisabled <- true
        UIApplication.SharedApplication.ScheduleLocalNotification(notif)
        let xxxx = new NSDate()
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

    let setAlarm alarmTime =
        createNotification (setAlarm2 alarmTime)

    let setTimer (timerTime: TimeSpan) =
        createNotification ((new NSDate()).AddSeconds(timerTime.TotalMilliseconds/1000.0))

    let Do (incoming: UILocalNotification) =
        Console.WriteLine("do alarm")
        UIApplication.SharedApplication.IdleTimerDisabled <- false
        if (alarm.IsSome && alarm.Value.FireDate.IsEqual(incoming.FireDate)) then 
            alarm <- None
            Radio.Play()
        if (timer.IsSome && timer.Value.FireDate.IsEqual(incoming.FireDate)) then 
            timer <- None
            Radio.Stop()

    do 
        // init: sleep and alarm icons are not active/hidden
        sleepIco.Hidden <- true
        sleepLbl.Hidden <- true
        alarmIco.Hidden <- true
        alarmLbl.Hidden <- true

        // hide/show sleep state
        Toolbar.timerButton.Add (fun maybeSpan ->
            match maybeSpan with
                | Some(span) ->
                    sleepIco.Hidden <- false
                    sleepLbl.Hidden <- false
                    sleepLbl.Text   <- span.ToString()
                    cancelTimer ()
                    timer <- Some(setTimer span)
                | None      ->
                    sleepIco.Hidden <- true
                    sleepLbl.Hidden <- true
                    cancelTimer ()
        )

        // hide/show alarm state
        Toolbar.alarmButton.Add (fun maybeSpan ->
            match maybeSpan with
                | Some(span) ->
                    alarmIco.Hidden <- false
                    alarmLbl.Hidden <- false
                    alarmLbl.Text   <- span.ToString()
                    cancelAlarm ()
                    alarm <- Some(setAlarm span)
                | None      ->
                    alarmIco.Hidden <- true
                    alarmLbl.Hidden <- true
                    cancelAlarm ()
        )

        // install a timer that updates the sleep timer remaining time
        let ttt = new System.Timers.Timer(1000.0)
        ttt.Elapsed.Add(fun _ -> 
            match timer with
                | Some(t) -> 
                    sleepLbl.InvokeOnMainThread(fun _ ->
                        let remaining = t.FireDate.SecondsSinceReferenceDate - (new NSDate()).SecondsSinceReferenceDate
                        sleepLbl.Text <- (new TimeSpan(0,0,int(remaining))).ToString()
                    )
                | None    -> 
                    ()
        )
        ttt.Start()

            


