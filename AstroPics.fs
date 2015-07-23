namespace RadioClock

open System
open System.Net
open System.Text.RegularExpressions
open MonoTouch.UIKit
open MonoTouch.Foundation

/// Provides an endless stream of background images.
/// The images are taken from an rss feed which contains url links the most recent astronomy pictures
/// of the day in the description of the new items. See also http://apod.nasa.gov/apod/astropix.html.
type AstroPics(view: UIImageView) =

    let AstroRssFeed = "http://www.acme.com/jef/apod/rss.xml"

    /// Displays an image in the background.
    let updatePicture (img: UIImage) : Unit =
        view.InvokeOnMainThread(fun _ ->
            UIView.Transition(view, 3.0, UIViewAnimationOptions.TransitionCrossDissolve, (fun _ -> view.Image <- img), null)
        )

    /// Downloads a picture.
    let nextPicture (url: String) : option<UIImage> = 
        try
            use webClient = new WebClient()    
            let bytes     = webClient.DownloadData(new Uri(url))
            Some(new UIImage(NSData.FromArray(bytes)))
        with
            | _ -> None

    /// Gets a single image url from the given string. 
    let urlFromDesc (input: string) : list<string> =
        let m = Regex.Match(input, "img src=\"([^\"]*)") 
        // group 0 is the whole matched expression, first group is 1
        if (m.Success) then [m.Groups.Item(1).Value] else [] 

    /// Gets a batch of background image urls from the items in an RSS feed.
    let nextUrls () : list<string> =   
        let items = RssFeed.items(AstroRssFeed)
        List.map (fun i -> urlFromDesc(i.desc)) items 
        |> List.concat // flatten list in case regexp didn't match

    /// Creates an endless sequence of background image urls.
    let imageUrlSeq : seq<option<string>> =
        Seq.unfold (fun urls ->
            match urls with                                 // consume current batch of urls
                | u::us -> 
                    Some(Some u, us)
                | []    ->
                   match nextUrls() with                    // once we run out, load a fresh batch of urls
                    | u::us -> Some(Some u, us)
                    | []    -> Some(None,   nextUrls())     // (fresh batch can be empty in case of an error)
        ) []

    /// Creates an endless sequence of background images.
    let imageSeq : seq<option<UIImage>> =
        Seq.map (fun url ->
            match url with
                | Some u -> nextPicture(u)
                | None   -> None
        ) imageUrlSeq

    do
        Async.Start (async {
            let images = imageSeq.GetEnumerator()
            while true do
                if (images.MoveNext()) then
                    match images.Current with
                        | Some i -> updatePicture(i)
                        | None   -> ()
                do! Async.Sleep 10000
        }) |> ignore

