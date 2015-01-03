namespace RadioClock

open System
open MonoTouch.UIKit
open MonoTouch.Foundation

[<Register("AppDelegate")>]
type AppDelegate() = 
    inherit UIApplicationDelegate()
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
        Console.WriteLine("RECEIVED LOCAL NOTIFICATION! - WE WERE IN FOREGROUND")
        Alarm.Do notification

module Main = 
    [<EntryPoint>]
    let main args = 
        UIApplication.Main(args, null, "AppDelegate")
        0

