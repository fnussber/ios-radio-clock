namespace RadioClock

open System
open System.Text
open System.Xml
open MonoTouch.UIKit

/// Provides an endless stream of news from an RSS feed.
module NewsStation = 

    let NextHeadline    = new Event<UIView>()
    let NextDescription = new Event<UIView>()

    let headFont = UIFont.FromName("Helvetica-Bold", 30.0f)
    let descFont = UIFont.FromName("Helvetica", 20.0f)

    let label (str: String, font: UIFont): UILabel =
        new UILabel(Text = str, Font = font, TranslatesAutoresizingMaskIntoConstraints = false, TextColor = UIColor.White)

    let newsSeq : seq<NewsItem> =
        RssFeed.newsSeq "http://www.tagesanzeiger.ch/rss.html"

    do
        Async.Start (async {
            let news = newsSeq.GetEnumerator()
            while true do
                // create labels and update UI on main thread
                if (news.MoveNext()) then
                    let n = news.Current
                    headFont.InvokeOnMainThread(fun _ ->
                        NextHeadline.Trigger   (label(n.head, headFont))
                        NextDescription.Trigger(label(n.desc, descFont))
                    )
                do! Async.Sleep 10000

        }) |> ignore
