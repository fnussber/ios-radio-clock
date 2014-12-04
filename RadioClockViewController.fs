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

        let digit = new Digit ("4")
        let toolbar = new Toolbar (this.View.Frame)
        this.View.AddSubview (digit)
        this.View.AddSubview (toolbar)

        let metrics = new NSDictionary()
        let views = new NSDictionary("toolbar", toolbar, "digit", digit)
        let ch = NSLayoutConstraint.FromVisualFormat("H:|[toolbar]|", NSLayoutFormatOptions.DirectionLeadingToTrailing, metrics, views) 
        let cv = NSLayoutConstraint.FromVisualFormat("V:[toolbar]|", NSLayoutFormatOptions.AlignAllLeft, metrics, views) 
        this.View.AddConstraints(ch)
        this.View.AddConstraints(cv)

        digit.AnimateIn()


    // Return true for supported orientations
    override this.ShouldAutorotateToInterfaceOrientation(orientation) = true

