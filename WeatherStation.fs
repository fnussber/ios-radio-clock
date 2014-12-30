namespace RadioClock

open System
open System.IO
open System.Net
open System.Text
open System.Xml
open MonoTouch.UIKit

// Unfortunately we can't use the tools from FSharp.Data like type providers etc. because including
// the dll inflates the binary beyond the maximum size allowed for the free Xamarin Studio version :(
// Credits for weather icons: http://icons8.com/web-app/category/ios7/Weather

/// Simple weather station that uses our current location to get weather information from openeweathermap.org
type WeatherStation() =

    // Defines a translation from openweather icon names to the ones we are using here
    // See http://bugs.openweathermap.org/projects/api/wiki/Weather_Condition_Codes for details
    let icons =
        [ "01d.png", "sun-50.png";                      // sky is clear
          "01n.png", "moon-50.png";
          "02d.png", "partly_cloudy_day-50.png";        // few clouds
          "02n.png", "partly_cloudy_night-50.png";
          "03d.png", "clouds-50.png";                   // scattered clouds
          "03n.png", "clouds-50.png";
          "04d.png", "clouds-50.png";                   // broken clouds
          "04n.png", "clouds-50.png";
          "09d.png", "rain-50.png";                     // shower rain
          "09n.png", "rain-50.png";
          "10d.png", "partly_cloudy_rain-50.png";       // rain
          "10n.png", "rain-50.png";
          "11d.png", "storm-50.png";                    // thunderstorm
          "11n.png", "storm-50.png";
          "13d.png", "snow-50.png";                     // snow    
          "13n.png", "snow-50.png";
          "50d.png", "fog_day-50.png";                  // mist
          "50n.png", "fog_night-50.png";
          ]
        |> Map.ofList 
        |> Map.map (fun key name -> new UIImage("weather/" + name))

    /// Provides a fallback icon in case we receive an unknown weather id
    let unknownIcon = new UIImage("weather/unknown.png")

    /// Gets the icon for the given weather id
    let iconForId name = defaultArg (icons.TryFind name) unknownIcon

    let getWeather (lat: float, long: float) =
        let url = sprintf "http://api.openweathermap.org/data/2.5/weather?lat=%f&lon=%f&mode=xml" lat long
        let req = HttpWebRequest.Create(url) :?> HttpWebRequest
        let resp = req.GetResponse()
        let stream = resp.GetResponseStream()
        let reader = new StreamReader(stream)
        let xml = reader.ReadToEnd()

        let doc = new XmlDocument()
        doc.LoadXml xml
        doc

    let value (xml: XmlDocument, value: string) = float((xml.SelectSingleNode("/current/" + value + "/@value")).InnerText)

    let temperature xml = value (xml, "temperature")

    let humidity xml = value (xml, "humidity")

    do
        let xml = getWeather(35.0, 39.0)
        Console.WriteLine("temperature = " + temperature(xml).ToString())
        Console.WriteLine("humidity    = " + humidity(xml).ToString())
        ()


