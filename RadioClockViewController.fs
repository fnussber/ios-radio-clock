namespace RadioClock

open System
open System.Drawing
open MonoTouch.UIKit
open MonoTouch.Foundation

[<Register("RadioClockViewController")>]
type RadioClockViewController() = 
    inherit UIViewController()

    member this.Radio = new Radio()

    // Release any cached data, images, etc that aren't in use.
    override this.DidReceiveMemoryWarning() = base.DidReceiveMemoryWarning()

    // Perform any additional setup after loading the view, typically from a nib.
    override this.ViewDidLoad() = 
        base.ViewDidLoad()

        let clock = new Clock()
        let toolbar = new Toolbar (clock)
        let weather = new WeatherStation()

        this.View.BackgroundColor <- UIColor.Black
        this.View.AddSubview (clock)
        this.View.AddSubview (toolbar)
  

        let metrics = new NSDictionary()
        let views = new NSDictionary("toolbar", toolbar, "clock", clock)
        let ch0 = NSLayoutConstraint.FromVisualFormat("H:|[toolbar]|", NSLayoutFormatOptions.DirectionLeadingToTrailing, metrics, views) 
        let ch1 = NSLayoutConstraint.FromVisualFormat("H:|[clock]|", NSLayoutFormatOptions.DirectionLeadingToTrailing, metrics, views) 
        let cv = NSLayoutConstraint.FromVisualFormat("V:|[clock][toolbar(44)]|", NSLayoutFormatOptions.DirectionLeadingToTrailing, metrics, views) 
        this.View.AddConstraints(ch0)
        this.View.AddConstraints(ch1)
        this.View.AddConstraints(cv)

        clock.Start()

    // Return true for supported orientations
    override this.ShouldAutorotateToInterfaceOrientation(orientation) = true



