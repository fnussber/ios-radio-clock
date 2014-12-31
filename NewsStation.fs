namespace RadioClock

open System
open System.IO
open System.Net
open System.Text
open System.Xml

type NewsStation() = 

    let getNews () =
        let url = "http://www.tagesanzeiger.ch/rss.html"
        let req = HttpWebRequest.Create(url) :?> HttpWebRequest
        let resp = req.GetResponse()
        let stream = resp.GetResponseStream()
        let reader = new StreamReader(stream)
        let xml = reader.ReadToEnd()

        let doc = new XmlDocument()
        doc.LoadXml xml
        doc

    let items(xml: XmlDocument) = xml.SelectNodes("/rss/channel/item")

    let nextStories(): list<string> =
        let xml = getNews()
        let itms = items(xml)
        let iter = itms.GetEnumerator()
        let mutable l: list<string> = List.Empty
        while iter.MoveNext() do 
            let item = iter.Current :?> XmlNode
            let title = item.SelectSingleNode("title").InnerText
            let desc  = item.SelectSingleNode("description").InnerText
            //Console.WriteLine(
            Console.WriteLine("title = " + title)
            Console.WriteLine("desc  = " + desc)
            l <- title :: l
        l
       
    member this.Stories(): seq<string> =
        Seq.unfold(fun (stories: list<string>) -> if (stories.IsEmpty) then Some("NEW SET", nextStories()) else Some(stories.Head, stories.Tail)) (nextStories())

    
