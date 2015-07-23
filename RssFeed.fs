namespace RadioClock

open System
open System.IO
open System.Net
open System.Xml

type NewsItem = {head: string; desc: string;}

// Read news from an rss feed an turn them into a list of "news items"
// which represent the headlines and their descriptions.
module RssFeed = 

    let separator: list<NewsItem> = [{head = "******"; desc = "******"}]

    let xmlItems (doc: XmlDocument): list<XmlNode> = 
        let nodes = doc.SelectNodes("/rss/channel/item")
        let iter  = nodes.GetEnumerator()
        let mutable l: list<XmlNode> = List.Empty
        while iter.MoveNext() do l <- (iter.Current :?> XmlNode) :: l
        l

    let item (xml: XmlNode) : NewsItem = 
        let v1 = xml.SelectSingleNode("title").InnerText
        let v2 = xml.SelectSingleNode("description").InnerText
        { head = v1; desc = v2; }

    let items (url: string) : list<NewsItem> =
        try
            let req    = HttpWebRequest.Create(url)
            use resp   = req.GetResponse()
            use stream = resp.GetResponseStream()
            let xml    = (new StreamReader(stream)).ReadToEnd()
            let doc    = new XmlDocument()
            doc.LoadXml xml
            xmlItems doc 
            |> List.map (fun i -> item i) 
            |> List.append separator
        with
            // in case something goes wrong catch the error and turn it into a news item
            | e -> [{ head = "Could not read rss feed " + url; desc = e.Message; }]
           
    let newsSeq (url: string) : seq<NewsItem> =
        Seq.unfold(fun its ->
            match its with                                 // consume current batch of news
                | n::ns -> 
                    Some(Some(n), ns)
                | []    ->
                   match items url with                    // once we run out, load a fresh batch of news
                    | n::ns -> Some(Some(n), ns)
                    | []    -> Some(None,   items(url))    // shouldn't happen, but need to cover all match cases
        ) []
        |> Seq.choose id

