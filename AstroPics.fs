namespace RadioClock

open System
open System.Net
open System.Text.RegularExpressions
open MonoTouch.UIKit
open MonoTouch.Foundation

type AstroPic = {title: string; image: UIImage;}

/// Provides an endless stream of background images.
/// The images are taken from an RSS feed which contains URL links to the most recent astronomy pictures
/// of the day in the description of the news items. Once all images from the RSS feed have been cycled
/// through the RSS feed is reloaded which may or may not result in the same set of pictures.
/// See also http://apod.nasa.gov/apod/astropix.html.
module AstroPics =

    let private nextPictureEvent = new Event<AstroPic>()

    let AstroRssFeed = "http://www.acme.com/jef/apod/rss.xml"
    let NextPicture  = nextPictureEvent.Publish

    /// Downloads a picture from the given URL
    let downloadPicture picTitle picUrl : option<AstroPic> = 
        try
            use webClient = new WebClient()    
            let bytes     = webClient.DownloadData(new Uri(picUrl))
            Some( {title = picTitle; image = new UIImage(NSData.FromArray(bytes))} )
        with
            | _ -> None

    /// Gets the image URL from the description of the news item. 
    let urlFromNewsItem (news: NewsItem) : option<string> =
        let m = Regex.Match(news.desc, "img src=\"([^\"]*)") 
        // group 0 is the whole matched expression, first group is 1
        if (m.Success) then Some (m.Groups.Item(1).Value) else None 

    /// Gets the title from the head of the news item and cleans it a bit.
    let titleFromNewsItem (news: NewsItem) : string =
        news.head.Replace("</b>", "").Replace("<br>", "").Trim()

    /// Gets the title and the URL of the picture represented by this news item.
    let titleAndUrlFromNewsItem (news: NewsItem) : option<string * string> =
        match urlFromNewsItem news with
            | Some (url)-> Some (titleFromNewsItem(news), url)
            | None     ->  None

    /// Creates an endless sequence of background images based on the news feed.
    let imageSeq : seq<AstroPic> =
        // a maybe monad would be useful for chaining here..
        RssFeed.newsSeq AstroRssFeed
        |> Seq.map (fun news         -> titleAndUrlFromNewsItem news)
        |> Seq.choose id
        |> Seq.map (fun (title, url) -> downloadPicture title url)
        |> Seq.choose id

    /// Initialize the module, start producing images.
    do
        // provide images as a background task and trigger an event when next one is available
        Async.Start (async {
            let images = imageSeq.GetEnumerator()
            while true do
                if (images.MoveNext()) then 
                    nextPictureEvent.Trigger images.Current
                do! Async.Sleep 10000
        }) |> ignore

