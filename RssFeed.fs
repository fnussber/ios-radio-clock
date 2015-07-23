﻿namespace RadioClock

open System
open System.IO
open System.Net
open System.Xml

type NewsItem = {head: string; desc: string;}

// Read news from an rss feed an turn them into a list of "news items"
// which represent the headlines and their descriptions.
module RssFeed = 

    let xmlItems (doc: XmlDocument): list<XmlNode> = 
        let nodes = doc.SelectNodes("/rss/channel/item")
        let iter  = nodes.GetEnumerator()
        let mutable l: list<XmlNode> = List.Empty
        while iter.MoveNext() do l <- (iter.Current :?> XmlNode) :: l
        l

    let item (xml: XmlNode): NewsItem = 
        let v1 = xml.SelectSingleNode("title").InnerText
        let v2 = xml.SelectSingleNode("description").InnerText
        { head = v1; desc = v2; }

    let items (url: string): list<NewsItem> =
        try
            let req    = HttpWebRequest.Create(url) :?> HttpWebRequest
            let resp   = req.GetResponse()
            let stream = resp.GetResponseStream()
            let xml    = (new StreamReader(stream)).ReadToEnd()
            let doc    = new XmlDocument()
            doc.LoadXml xml
            xmlItems doc |> List.map (fun i -> item i)
        with
            // in case something goes wrong catch the error and turn it into a news item
            | e -> [{ head = "Could not read rss feed " + url; desc = e.Message; }]
           
    let newsSeq (url: string) : seq<NewsItem> =
        Seq.unfold(fun its ->
            match its with                                 // consume current batch of urls
                | n::ns -> 
                    Some(Some(n), ns)
                | []    ->
                   match items url with                    // once we run out, load a fresh batch of urls
                    | n::ns -> Some(Some(n), ns)
                    | []    -> Some(None,   items(url))     // (fresh batch can be empty in case of an error)
        ) []
        |> Seq.choose id

