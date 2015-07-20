namespace RadioClock

open System
open System.Drawing
open MonoTouch.CoreAnimation
open MonoTouch.CoreGraphics
open MonoTouch.Foundation
open MonoTouch.UIKit

type Digit(digit: String) as self =
    inherit UILabel()

    let duration = 1.0
    let random = System.Random()

    do
        self.TranslatesAutoresizingMaskIntoConstraints <- false  // important for auto layout!
        self.Font <- Layout.bigFont
        self.Text <- digit
        self.TextAlignment <- UITextAlignment.Center
        self.BackgroundColor <- UIColor.Clear// new UIColor(float32(random.NextDouble()), 0.0f, 0.0f, 0.5f)
        self.TextColor <- UIColor.White

    member this.Blink() =
        // --- scale animation
        let scaleAnim = CABasicAnimation.FromKeyPath("transform.scale")
        scaleAnim.TimingFunction <- CAMediaTimingFunction.FromName(CAMediaTimingFunction.EaseInEaseOut)
        scaleAnim.Duration <- 0.5
        scaleAnim.RepeatCount <- 1000.0f // howto: Endless repeat?
        scaleAnim.AutoReverses <- true
        //scaleAnim.RemovedOnCompletion <- true
        scaleAnim.To <- NSNumber.FromDouble(1.5)
        self.Layer.AddAnimation(scaleAnim, "blinkAnim")

    member this.StopBlink() =
        self.Layer.RemoveAnimation("blinkAnim")


    member this.AnimateIn() =

        // --- rotate animation
        let rotAnim = CABasicAnimation.FromKeyPath("transform.rotation")
        rotAnim.TimingFunction <- CAMediaTimingFunction.FromName(CAMediaTimingFunction.EaseInEaseOut)
        rotAnim.Duration <- 1.0
        //rotAnim.RepeatCount <- 8.0f
        rotAnim.AutoReverses <- false
        //rotAnim.RemovedOnCompletion <- true
        rotAnim.From <- NSNumber.FromDouble(Math.PI/2.0)
        rotAnim.To <- NSNumber.FromDouble(0.0)
        self.Layer.AddAnimation(rotAnim, "rotAnim")

        // --- animate along curve
        let path = new UIBezierPath()
        let dx = float32((random.Next() % 800) - 400) // -300.0f
        let dy = float32((random.Next() % 500) - 250) // +200.0f
        path.MoveTo(new PointF(this.Center.X + dx, this.Center.Y + dy))
        path.AddQuadCurveToPoint(this.Center, new PointF(this.Center.X, this.Center.Y + dy))
        let anim = CAKeyFrameAnimation.GetFromKeyPath("position")
        anim.Duration <- 1.0
        anim.Path <- path.CGPath
        anim.TimingFunction <- CAMediaTimingFunction.FromName(CAMediaTimingFunction.EaseOut)
        //anim.RotationMode <- CAKeyFrameAnimation.RotateModeAuto.ToString()
        //anim.RemovedOnCompletion <- true
        self.Layer.AddAnimation(anim, "moveAnim")

        // --- fade in
        self.Alpha <- 0.0f
        UIView.Animate(0.9, 0.0, UIViewAnimationOptions.CurveEaseIn, (fun () -> self.Alpha <- 1.0f), null)

  
    member this.AnimateOut() =

        // --- scale animation
        let scaleAnim = CABasicAnimation.FromKeyPath("transform.scale")
        scaleAnim.TimingFunction <- CAMediaTimingFunction.FromName(CAMediaTimingFunction.EaseInEaseOut)
        scaleAnim.Duration <- 0.25
        scaleAnim.RepeatCount <- 1.0f
        scaleAnim.AutoReverses <- true
        //scaleAnim.RemovedOnCompletion <- true
        scaleAnim.To <- NSNumber.FromDouble(1.8)
        self.Layer.AddAnimation(scaleAnim, "scaleAnim")

        // --- rotate animation
        let rotAnim = CABasicAnimation.FromKeyPath("transform.rotation")
        rotAnim.TimingFunction <- CAMediaTimingFunction.FromName(CAMediaTimingFunction.EaseInEaseOut)
        rotAnim.Duration <- 1.0
        //rotAnim.RepeatCount <- 8.0f
        rotAnim.AutoReverses <- false
        rotAnim.RemovedOnCompletion <- true
        rotAnim.From <- NSNumber.FromDouble(0.0)
        rotAnim.To <- NSNumber.FromDouble(Math.PI/2.0)
        self.Layer.AddAnimation(rotAnim, "rotAnim")

        // --- animate along curve
        let path = new UIBezierPath()
        let dx = float32((random.Next() % 800) - 400) // +300.0f
        let dy = float32((random.Next() % 500) - 250) // -200.0f
        path.MoveTo(this.Center)
        path.AddQuadCurveToPoint(new PointF(this.Center.X + dx, this.Center.Y + dy), new PointF(this.Center.X, this.Center.Y + dy))
        let anim = CAKeyFrameAnimation.GetFromKeyPath("position")
        anim.Duration <- 1.0
        anim.Path <- path.CGPath
        anim.TimingFunction <- CAMediaTimingFunction.FromName(CAMediaTimingFunction.EaseIn)
        //anim.RotationMode <- CAKeyFrameAnimation.RotateModeAuto.ToString()
        anim.RemovedOnCompletion <- true
        self.Layer.AddAnimation(anim, "moveAnim")

        // --- fade out
        self.Alpha <- 1.0f
        UIView.Animate(0.9, 0.0, UIViewAnimationOptions.CurveEaseOut, (fun () -> self.Alpha <- 0.0f), null)


//[<AbstractClass>]
type Digit2 (up: NSAction, down: NSAction) as this =
    inherit UIView()

    let mutable d = 0
    let label0 = new Digit(d.ToString())
    let label1 = new Digit(d.ToString())

    do
        this.TranslatesAutoresizingMaskIntoConstraints <- false  // important for auto layout!
        this.AddSubview(label0)
        this.AddSubview(label1)
        let metrics = new NSDictionary()
        let views = new NSDictionary("l0", label0, "l1", label1)
        let ch0 = NSLayoutConstraint.FromVisualFormat("H:|[l0]|", NSLayoutFormatOptions.DirectionLeadingToTrailing, metrics, views) 
        let ch1 = NSLayoutConstraint.FromVisualFormat("H:|[l1]|", NSLayoutFormatOptions.DirectionLeadingToTrailing, metrics, views) 
        let cv0 = NSLayoutConstraint.FromVisualFormat("V:|[l0]|", NSLayoutFormatOptions.DirectionLeadingToTrailing, metrics, views) 
        let cv1 = NSLayoutConstraint.FromVisualFormat("V:|[l1]|", NSLayoutFormatOptions.DirectionLeadingToTrailing, metrics, views) 
        this.AddConstraints(ch0)
        this.AddConstraints(ch1)
        this.AddConstraints(cv0)
        this.AddConstraints(cv1)

        let swipeUp = new UISwipeGestureRecognizer(up)
        swipeUp.Direction <- UISwipeGestureRecognizerDirection.Up 
        let swipeDown = new UISwipeGestureRecognizer(down)
        swipeDown.Direction <- UISwipeGestureRecognizerDirection.Down 
        let swipeLeft = new UISwipeGestureRecognizer(up)
        swipeLeft.Direction <- UISwipeGestureRecognizerDirection.Left 
        let swipeRight = new UISwipeGestureRecognizer(down)
        swipeRight.Direction <- UISwipeGestureRecognizerDirection.Right 
        this.AddGestureRecognizer(swipeUp)
        this.AddGestureRecognizer(swipeDown)
        this.AddGestureRecognizer(swipeLeft)
        this.AddGestureRecognizer(swipeRight)


//    abstract Bgc: unit -> UIColor

    member this.Digit = d
    member this.Label0 = label0
    member this.Label1 = label1

    member this.StopBlink() =
        label1.StopBlink()

    member this.Blink() =
        label1.Blink()

    member this.Next(next: int) =
        label0.InvokeOnMainThread((fun _ ->
                label0.AnimateOut()
                label1.AnimateIn()
                label0.Text <- d.ToString()
                label1.Text <- (next.ToString())
                d <- next
//                let cb = this.Bgc()
//                this.BackgroundColor <- cb
//                let c = this.Bgc()
//                label0.BackgroundColor <- c
//                label1.BackgroundColor <- c
                )
            )


//type DigitH() =
//    inherit Digit2()
//    let random = System.Random()
//    override this.Bgc() = new UIColor(float32(random.NextDouble()) + 0.3f, 0.0f, 0.0f, 0.5f)

//type DigitM() =
//    inherit Digit2()
//    let random = System.Random()
//    override this.Bgc() = new UIColor(0.0f, float32(random.NextDouble()) + 0.3f, 0.0f, 0.5f)

//type DigitS() =
//    inherit Digit2()
//    let random = System.Random()
//    override this.Bgc() = new UIColor(0.0f, 0.0f, float32(random.NextDouble()) + 0.3f, 0.5f)

//[<AbstractClass>]
type TwoDigits(maxValue: int) as this =
    inherit UIView()

    let down x  = new NSAction(fun _ -> this.Down(x))
    let up   x  = new NSAction(fun _ -> this.Up(x))

    let d1      = new Digit2(down(10), up(10))
    let d0      = new Digit2(down(1), up(1))


    do
        this.TranslatesAutoresizingMaskIntoConstraints <- false  // important for auto layout!
        this.AddSubview(d1) 
        this.AddSubview(d0)

        let metrics = new NSDictionary()
        let views = new NSDictionary("d1", d1, "d0", d0)
        let ch = NSLayoutConstraint.FromVisualFormat("H:|[d1][d0(==d1)]|", NSLayoutFormatOptions.DirectionLeadingToTrailing, metrics, views) 
        let cv1 = NSLayoutConstraint.FromVisualFormat("V:|[d1]|", NSLayoutFormatOptions.DirectionLeadingToTrailing, metrics, views) 
        let cv2 = NSLayoutConstraint.FromVisualFormat("V:|[d0]|", NSLayoutFormatOptions.DirectionLeadingToTrailing, metrics, views) 
        this.AddConstraints(ch)
        this.AddConstraints(cv1)
        this.AddConstraints(cv2)

    member this.Blink() =
        d1.Blink()
        d0.Blink()

    member this.StopBlink() =
        d1.StopBlink()
        d0.StopBlink()

    member this.Value = d1.Digit * 10 + d0.Digit

    member this.Next(d: int) = 
        if d1.Digit <> d/10 then d1.Next(d/10)
        if d0.Digit <> d%10 then d0.Next(d%10)

    member this.Up(x: int)   =
        if this.Value + x < maxValue then this.Next(this.Value + x) else this.Next(0)

    member this.Down(x: int) = 
        if (this.Value - x) >= 0 then this.Next(this.Value - x) else this.Next(maxValue - 1)
