namespace RadioClock

open System
open System.Net
open System.Text.RegularExpressions
open MonoTouch.UIKit
open MonoTouch.Foundation

/// Provides an endless stream of background images.
/// The images are taken from an RSS feed which contains URL links to the most recent astronomy pictures
/// of the day in the description of the news items. See also http://apod.nasa.gov/apod/astropix.html.
module AstroPics =

    let private nextPictureEvent   = new Event<UIImage>()

    let AstroRssFeed = "http://www.acme.com/jef/apod/rss.xml"
    let NextPicture  = nextPictureEvent.Publish

    /// Downloads a picture from the given URL
    let nextPicture (url: String) : option<UIImage> = 
        try
            use webClient = new WebClient()    
            let bytes     = webClient.DownloadData(new Uri(url))
            Some(new UIImage(NSData.FromArray(bytes)))
        with
            | _ -> None

    /// Gets a single image URL from the given string. 
    let urlFromDesc (input: string) : list<string> =
        let m = Regex.Match(input, "img src=\"([^\"]*)") 
        // group 0 is the whole matched expression, first group is 1
        if (m.Success) then [m.Groups.Item(1).Value] else [] 

    /// Gets a batch of background image URLs from the items in an RSS feed.
    let nextUrls () : list<string> =   
        let items = RssFeed.items AstroRssFeed
        List.map (fun i -> urlFromDesc(i.desc)) items 
        |> List.concat // flatten list in case regexp didn't match

    /// Creates an endless sequence of background image URLs.
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

    /// Initialise the module, start producing images.
    do
        // provide images as a background task and trigger an event when next one is available
        Async.Start (async {
            let images = imageSeq.GetEnumerator()
            while true do
                if (images.MoveNext()) then
                    match images.Current with
                        | Some img -> nextPictureEvent.Trigger img
                        | None     -> ()
                do! Async.Sleep 10000
        }) |> ignore

