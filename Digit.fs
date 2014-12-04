namespace RadioClock

open System
open System.Drawing
open MonoTouch.CoreAnimation
open MonoTouch.CoreGraphics
open MonoTouch.Foundation
open MonoTouch.UIKit

type Digit(digit: String) as self =
    inherit UILabel()

    do
        //this.TranslatesAutoresizingMaskIntoConstraints <- false  // important for auto layout!
        self.Frame <- new RectangleF(10.0f, 100.0f, 100.0f, 100.0f)
        self.Text <- digit
        self.TextColor <- UIColor.Blue
        self.BackgroundColor <- UIColor.Yellow

    member this.AnimateIn() =

        // --- scale animation
        let scaleAnim = CABasicAnimation.FromKeyPath("transform.scale")
        scaleAnim.TimingFunction <- CAMediaTimingFunction.FromName(CAMediaTimingFunction.EaseInEaseOut)
        scaleAnim.Duration <- 0.5
        scaleAnim.RepeatCount <- 8.0f
        scaleAnim.AutoReverses <- true
        scaleAnim.RemovedOnCompletion <- true
        scaleAnim.To <- NSNumber.FromDouble(1.2) //new NSObject(CATransform3D.MakeScale(1.2f, 1.2f, 1.0f))
        self.Layer.AddAnimation(scaleAnim, "scaleAnim")

        // --- animate along curve
        let path = new UIBezierPath()
        path.MoveTo(new PointF(100.0f, 100.0f))
        path.AddQuadCurveToPoint(new PointF(600.0f, 800.0f), new PointF(600.0f, 100.0f))
        let anim = CAKeyFrameAnimation.GetFromKeyPath("position")
        anim.Duration <- 4.0
        anim.Path <- path.CGPath
        anim.RemovedOnCompletion <- true
        self.Layer.AddAnimation(anim, "moveAnim")

        // --- some very basic animations fade in/out and change pos
        UIView.Animate(1.5, 2.0, UIViewAnimationOptions.CurveEaseIn, new NSAction(fun () -> self.Alpha <- 0.0f), null)

  
    member this.AnimateOut() =
        UIView.Animate(0.5, 0.0, UIViewAnimationOptions.CurveEaseIn, new NSAction(fun () -> ()), new NSAction(fun () -> ()))



