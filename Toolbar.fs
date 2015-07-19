namespace RadioClock

open System
open MonoTouch.UIKit
open MonoTouch.Foundation

module Toolbar =

    let timerEvent = new Event<option<TimeSpan>>()
    let alarmEvent = new Event<option<TimeSpan>>()
    let radioEvent = new Event<string>()

    let timerButton = timerEvent.Publish
    let alarmButton = alarmEvent.Publish
    let radioButton = radioEvent.Publish

    let button image handler =
        let btn = new UIBarButtonItem()
        btn.Image <- new UIImage(image: string)
        btn.Style <- UIBarButtonItemStyle.Plain
        btn.Clicked.Add(handler(btn)) // note: handler needs to know button in order to display popup menu
        btn

    let timerMenuEntry str span =
        UIAlertAction.Create(str, UIAlertActionStyle.Default, (fun _ -> timerEvent.Trigger span))

    let timerMenu ctrl btn = 
        let m = UIAlertController.Create("Timer", "Select Remaining Time", UIAlertControllerStyle.ActionSheet)  
        m.PopoverPresentationController.BarButtonItem <- btn 
        m.AddAction(timerMenuEntry "60 minutes" (Some(new TimeSpan(1,  0, 0)))) 
        m.AddAction(timerMenuEntry "30 minutes" (Some(new TimeSpan(0, 30, 0)))) 
        m.AddAction(timerMenuEntry "20 minutes" (Some(new TimeSpan(0, 20, 0)))) 
        m.AddAction(timerMenuEntry "1 minute" (Some(new TimeSpan(0, 1, 0)))) 
        m.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Default, (fun _ -> timerEvent.Trigger None))) 
        (ctrl: UIViewController).PresentViewController(m, true, null)

    let alarmMenuEntry str span =
        UIAlertAction.Create(str, UIAlertActionStyle.Default, (fun _ -> alarmEvent.Trigger span))

    let alarmMenu ctrl btn = 
        let m = UIAlertController.Create("Alarm", "Select Alarm Time", UIAlertControllerStyle.ActionSheet)  
        m.PopoverPresentationController.BarButtonItem <- btn 
        m.AddAction(alarmMenuEntry "07:00" (Some(new TimeSpan(7,  0 ,0)))) 
        m.AddAction(alarmMenuEntry "07:30" (Some(new TimeSpan(7, 30 ,0)))) 
        m.AddAction(alarmMenuEntry "08:00" (Some(new TimeSpan(8,  0 ,0)))) 
        m.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Default, (fun _ -> alarmEvent.Trigger None))) 
        (ctrl: UIViewController).PresentViewController(m, true, null)

//    let SetAlarm (eventArgs: EventArgs): Unit =
//        let types = UIUserNotificationType.Alert
//        let settings = UIUserNotificationSettings.GetSettingsForTypes(types, null)
//        UIApplication.SharedApplication.RegisterUserNotificationSettings(settings)
////        if clock.IsStopped() then 
////            Alarm.Start(clock.Time())
////            clock.StopBlink()
////            clock.Start() 
////        else 
////            clock.Stop()
////            clock.Blink()
//        //this.NavigationController.ToolbarHidden <- true

//    let SetTimer(eventArgs: EventArgs): Unit =
//        Console.WriteLine("set timer")
//        //this.NavigationController.ToolbarHidden <- true

    let toolbarItems (ctrl)  = [|
            button Layout.SleepIcon (fun btn _ -> timerMenu ctrl btn) 
            button Layout.AlarmIcon (fun btn _ -> alarmMenu ctrl btn)
            button Layout.RadioIcon (fun btn _ -> radioEvent.Trigger "")
        |]


