namespace RadioClock

open System
open System.IO
open System.Net
open System.Text
open System.Xml
open MonoTouch.UIKit

type NewsItem = {head: string; desc: string;}

module NewsStation = 

    let timer = new System.Timers.Timer(10000.0)
    let NextHeadline    = new Event<string>()
    let NextDescription = new Event<string>()
    let mutable nitems  = list<NewsItem>.Empty

    let headFont  = UIFont.FromName("Helvetica-Bold", 30.0f)
    let storyFont = UIFont.FromName("Helvetica", 20.0f)

    let getNews () =
        let url = "http://www.tagesanzeiger.ch/rss.html"
        let req = HttpWebRequest.Create(url) :?> HttpWebRequest
        let resp = req.GetResponse() // throws 501.. etc -> ERROR HANDLING!!!
        let stream = resp.GetResponseStream()
        let reader = new StreamReader(stream)
        let xml = reader.ReadToEnd()

        let doc = new XmlDocument()
        doc.LoadXml xml // throws XmlException -> ERROR HANDLING!!
        doc

    let xmlItems(doc: XmlDocument): list<XmlNode> = 
        let nodes = doc.SelectNodes("/rss/channel/item")
        let iter = nodes.GetEnumerator()
        let mutable l: list<XmlNode> = List.Empty
        while iter.MoveNext() do l <- (iter.Current :?> XmlNode) :: l
        l

    let item(xml: XmlNode): NewsItem = 
        let v1 = xml.SelectSingleNode("title").InnerText
        let v2 = xml.SelectSingleNode("description").InnerText
        let ni = { head = v1; desc = v2; }
        ni

    let items xml: list<NewsItem> =
        xmlItems xml |> List.map (fun i -> item i)

    let loadNewsItems(): Unit = 
        // execute load in background and add news items once they are available
        let newestItems = items(getNews()) // TODO: DO IN BACKGROUND!
        lock nitems (fun _ -> nitems <- nitems |> List.append(newestItems))

    let nextItem(): Unit =
        // produce the next item, if available, empty item if currently no items are available
        if nitems.Length <= 2 then loadNewsItems()
        let next = lock nitems (fun _ -> 
            if nitems.Length > 0 then 
                let n = nitems.Head
                nitems <- nitems.Tail 
                n
            else 
                {head="NONE";desc="NONE";}) // TODO: is there a headOption in F#?
        NextHeadline.Trigger(next.head)
        NextDescription.Trigger(next.desc)

    do
        timer.Elapsed.Add(fun _ -> nextItem())
        timer.Start()

//    member t.NextHeadline = nextHeadline
//    member t.NextDescription = nextDescription

    // TODO: other cleanup needed? Is this needed at all??
//    interface IDisposable with
//        member x.Dispose() = timer.Dispose()

