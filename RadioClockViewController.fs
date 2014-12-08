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

//        let digit0 = new Digit2 (-1.0f)
//        let digit1 = new Digit2 ( 0.0f)
//        let digit2 = new Digit2 ( 1.0f)
        let toolbar = new Toolbar (this.View.Frame)
        let clock = new Clock()
//        this.View.AddSubview (digit0.Label0)
//        this.View.AddSubview (digit0.Label1)
//        this.View.AddSubview (digit1.Label0)
//        this.View.AddSubview (digit1.Label1)
//        this.View.AddSubview (digit2.Label0)
//        this.View.AddSubview (digit2.Label1)
        
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

//        this.DoIt(digit0)
//        this.DoIt(digit1)
//        this.DoIt(digit2)
        //digit.Next()


    // Return true for supported orientations
    override this.ShouldAutorotateToInterfaceOrientation(orientation) = true



