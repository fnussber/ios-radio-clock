namespace RadioClock

open System
open MonoTouch.UIKit
open MonoTouch.Foundation
open MonoTouch.AVFoundation

[<Register("AppDelegate")>]
type AppDelegate() = 
    inherit UIApplicationDelegate()

    do
        NSNotificationCenter.DefaultCenter.AddObserver(UIApplication.DidBecomeActiveNotification, fun _ ->
            UIApplication.SharedApplication.IdleTimerDisabled <- true
        ) |> ignore

        // this will keep audio playing when app is in the background
        AVAudioSession.SharedInstance().SetCategory(new NSString("AVAudioSessionCategoryPlayback")) |> ignore


    member val Window = null with get, set

    // This method is invoked when the application is ready to run.
    override this.FinishedLaunching(app, options) = 
        this.Window <- new UIWindow(UIScreen.MainScreen.Bounds)
        let viewController = new RadioClockViewController()
        viewController.View.BackgroundColor <- UIColor.White
        let navController = new UINavigationController(viewController)
        this.Window.RootViewController <- navController
        this.Window.MakeKeyAndVisible()
        true

    override this.ReceivedLocalNotification(app, notification) =
        // Note: notifications will only be received if application is in the foreground!
        Alarm.handleNotification notification


module Main = 
    [<EntryPoint>]
    let main args = 
        UIApplication.Main(args, null, "AppDelegate")
        0

