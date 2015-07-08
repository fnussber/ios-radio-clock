namespace RadioClock

open System
open System.IO
open System.Net
open System.Xml

type NewsItem = {head: string; desc: string;}

module RssFeed = 

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

    let getFeed(url: String): XmlDocument =  // TODO: DO IN BACKGROUND!
        let req = HttpWebRequest.Create(url) :?> HttpWebRequest
        let resp = req.GetResponse() // throws 501.. etc -> ERROR HANDLING!!!
        let stream = resp.GetResponseStream()
        let reader = new StreamReader(stream)
        let xml = reader.ReadToEnd()

        let doc = new XmlDocument()
        doc.LoadXml xml // throws XmlException -> ERROR HANDLING!!
        doc

    let items(url: String): list<NewsItem> =
        let xml = getFeed(url)
        xmlItems xml |> List.map (fun i -> item i)



