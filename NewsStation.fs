﻿namespace RadioClock

open System
open System.IO
open System.Net
open System.Text
open System.Xml
open MonoTouch.UIKit

type NewsStation() = 

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

    let items(xml: XmlDocument) = 
        let nodes = xml.SelectNodes("/rss/channel/item")
        let iter = nodes.GetEnumerator()
        let mutable l: list<XmlNode> = List.Empty
        while iter.MoveNext() do l <- (iter.Current :?> XmlNode) :: l
        l

    let texts xml node =
        items xml |> List.map (fun i -> i.SelectSingleNode(node).InnerText)

    let nextHeads(): list<string> = 
        let xml = getNews()
        texts xml "title"

    let nextStories(): list<string> = 
        let xml = getNews()
        texts xml "description"

    let labelFor text = new UILabel(Text = text) :> UIView

    // TODO: Heads and Stories should read from same backing data structure in order to make sure they're always in sync

    member this.Heads(): seq<UIView> =
        Seq.unfold(fun (stories: list<string>) -> if (stories.IsEmpty) then Some((labelFor("*** *** ***")), nextHeads()) else Some(labelFor(stories.Head), stories.Tail)) (nextHeads())

    member this.Stories(): seq<UIView> =
        Seq.unfold(fun (stories: list<string>) -> if (stories.IsEmpty) then Some((labelFor("*** *** ***")), nextStories()) else Some(labelFor(stories.Head), stories.Tail)) (nextStories())
    
