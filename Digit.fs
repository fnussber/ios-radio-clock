namespace RadioClock

open System
open System.Drawing
open MonoTouch.CoreAnimation
open MonoTouch.CoreGraphics
open MonoTouch.CoreText
open MonoTouch.Foundation
open MonoTouch.UIKit

/// Single digit with some animations: blink, animate to final position and animate away from final position.
type Digit(digit: String) as this =
    inherit UILabel()

    let duration = 1.0
    let random = System.Random()

    do
        this.TranslatesAutoresizingMaskIntoConstraints <- false
        this.Font            <- Layout.bigFont
        this.Text            <- digit
        this.TextAlignment   <- UITextAlignment.Center
        this.BackgroundColor <- UIColor.Clear
        this.TextColor       <- UIColor.White

    member this.blink () =
        // --- scale animation
        let scaleAnim = CABasicAnimation.FromKeyPath("transform.scale")
        scaleAnim.TimingFunction <- CAMediaTimingFunction.FromName(CAMediaTimingFunction.EaseInEaseOut)
        scaleAnim.Duration <- 0.5
        scaleAnim.RepeatCount <- 1000.0f
        scaleAnim.AutoReverses <- true
        scaleAnim.To <- NSNumber.FromDouble(1.5)
        this.Layer.AddAnimation(scaleAnim, "blinkAnim")

    member this.stopBlink() =
        this.Layer.RemoveAnimation("blinkAnim")


    member this.animateIn() =

        // --- rotate animation
        let rotAnim = CABasicAnimation.FromKeyPath("transform.rotation")
        rotAnim.TimingFunction <- CAMediaTimingFunction.FromName(CAMediaTimingFunction.EaseInEaseOut)
        rotAnim.Duration <- 1.0
        rotAnim.AutoReverses <- false
        rotAnim.From <- NSNumber.FromDouble(Math.PI/2.0)
        rotAnim.To <- NSNumber.FromDouble(0.0)
        this.Layer.AddAnimation(rotAnim, "rotAnim")

        // --- animate along curve
        let path = new UIBezierPath()
        let dx = float32((random.Next() % 800) - 400)
        let dy = float32((random.Next() % 500) - 250)
        path.MoveTo(new PointF(this.Center.X + dx, this.Center.Y + dy))
        path.AddQuadCurveToPoint(this.Center, new PointF(this.Center.X, this.Center.Y + dy))
        let anim = CAKeyFrameAnimation.GetFromKeyPath("position")
        anim.Duration <- 1.0
        anim.Path <- path.CGPath
        anim.TimingFunction <- CAMediaTimingFunction.FromName(CAMediaTimingFunction.EaseOut)
        this.Layer.AddAnimation(anim, "moveAnim")

        // --- fade in
        this.Alpha <- 0.0f
        UIView.Animate(0.9, 0.0, UIViewAnimationOptions.CurveEaseIn, (fun () -> this.Alpha <- 1.0f), null)

  
    member this.animateOut() =

        // --- scale animation
        let scaleAnim = CABasicAnimation.FromKeyPath("transform.scale")
        scaleAnim.TimingFunction <- CAMediaTimingFunction.FromName(CAMediaTimingFunction.EaseInEaseOut)
        scaleAnim.Duration <- 0.25
        scaleAnim.RepeatCount <- 1.0f
        scaleAnim.AutoReverses <- true
        scaleAnim.To <- NSNumber.FromDouble(1.8)
        this.Layer.AddAnimation(scaleAnim, "scaleAnim")

        // --- rotate animation
        let rotAnim = CABasicAnimation.FromKeyPath("transform.rotation")
        rotAnim.TimingFunction <- CAMediaTimingFunction.FromName(CAMediaTimingFunction.EaseInEaseOut)
        rotAnim.Duration <- 1.0
        //rotAnim.RepeatCount <- 8.0f
        rotAnim.AutoReverses <- false
        rotAnim.RemovedOnCompletion <- true
        rotAnim.From <- NSNumber.FromDouble(0.0)
        rotAnim.To <- NSNumber.FromDouble(Math.PI/2.0)
        this.Layer.AddAnimation(rotAnim, "rotAnim")

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
        this.Layer.AddAnimation(anim, "moveAnim")

        // --- fade out
        this.Alpha <- 1.0f
        UIView.Animate(0.9, 0.0, UIViewAnimationOptions.CurveEaseOut, (fun () -> this.Alpha <- 0.0f), null)


/// Animated digit that uses two digits to create the effet of a "new" digit 
/// moving into place while the "old" one moves away.
type AnimatedDigit (up: NSAction, down: NSAction) as this =
    inherit UIView()

    let mutable d = 0
    let label0 = new Digit(d.ToString())
    let label1 = new Digit(d.ToString())

    do

        this.TranslatesAutoresizingMaskIntoConstraints <- false  // important for auto layout!

        let views = [
            "l0", label0 :> UIView
            "l1", label1 :> UIView
        ]
        let formats = [
            "H:|[l0]|"
            "H:|[l1]|"
            "V:|[l0]|"
            "V:|[l1]|"
        ]
        Layout.layout this formats views

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


    member this.Digit = d
    member this.Label0 = label0
    member this.Label1 = label1

    member this.stopBlink () =
        label1.stopBlink ()

    member this.blink () =
        label1.blink ()

    member this.next(next: int) =
        label0.InvokeOnMainThread((fun _ ->
                label0.animateOut()
                label1.animateIn()
                label0.Text <- d.ToString()
                label1.Text <- (next.ToString())
                d <- next
                )
            )


/// Combine two animated digits to allow representing hours, minutes and seconds.
type DoubleDigit (maxValue: int) as this =
    inherit UIView()

    let downF x  = new NSAction(fun _ -> this.down(x))
    let upF   x  = new NSAction(fun _ -> this.up(x))

    let d1      = new AnimatedDigit(downF(10), upF(10))
    let d0      = new AnimatedDigit(downF(1),  upF(1))

    do
        this.TranslatesAutoresizingMaskIntoConstraints <- false

        let views = [
            "d0", d0 :> UIView
            "d1", d1 :> UIView
        ]
        let formats = [
            "H:|[d1][d0(==d1)]|"
            "V:|[d1]|" 
            "V:|[d0]|"       
        ]

        Layout.layout this formats views

    member this.blink () =
        d1.blink ()
        d0.blink ()

    member this.stopBlink () =
        d1.stopBlink ()
        d0.stopBlink ()

    member this.value () = 
        d1.Digit * 10 + d0.Digit

    member this.next (d: int) = 
        if d1.Digit <> d/10 then d1.next (d/10)
        if d0.Digit <> d%10 then d0.next (d%10)

    member this.up (x: int) =
        if this.value() + x < maxValue then this.next(this.value() + x) else this.next 0

    member this.down (x: int) = 
        if this.value() - x >= 0 then this.next(this.value() - x) else  this.next(maxValue - 1)
