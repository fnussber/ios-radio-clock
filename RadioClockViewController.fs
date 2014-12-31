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
        let ticker = new Ticker()
        let weather = new WeatherStation()

        this.View.BackgroundColor <- UIColor.Black
        this.View.AddSubview (clock)
        this.View.AddSubview (ticker)
        this.View.AddSubview (toolbar)
  

        let metrics = new NSDictionary()
        let views = new NSDictionary("toolbar", toolbar, "ticker", ticker, "clock", clock)
        let ch0 = NSLayoutConstraint.FromVisualFormat("H:|[clock]|", NSLayoutFormatOptions.DirectionLeadingToTrailing, metrics, views) 
        let ch1 = NSLayoutConstraint.FromVisualFormat("H:|[ticker]|", NSLayoutFormatOptions.DirectionLeadingToTrailing, metrics, views) 
        let ch2 = NSLayoutConstraint.FromVisualFormat("H:|[toolbar]|", NSLayoutFormatOptions.DirectionLeadingToTrailing, metrics, views) 
        let cv = NSLayoutConstraint.FromVisualFormat("V:|[clock][ticker(44)][toolbar(44)]|", NSLayoutFormatOptions.DirectionLeadingToTrailing, metrics, views) 
        this.View.AddConstraints(ch0)
        this.View.AddConstraints(ch1)
        this.View.AddConstraints(ch2)
        this.View.AddConstraints(cv)

        clock.Start()
        ticker.Text <- "Hello world.....       This is a message....."

    // Return true for supported orientations
    override this.ShouldAutorotateToInterfaceOrientation(orientation) = true



