using System;
using System.Xml.Serialization;

[XmlRoot("Price")]
public class Price : PriceBase
{
    [XmlElement("EventType")]
    public string EventType { get; set; } // e

    [XmlElement("EventTime")]
    public long EventTime { get; set; } // E

    [XmlElement("Symbol")]
    public string Symbol { get; set; } // s

    [XmlElement("AveragePriceInterval")]
    public string AveragePriceInterval { get; set; } // i

    [XmlElement("AveragePrice")]
    public string AveragePrice { get; set; } // w

    [XmlElement("LastTradeTime")]
    public long LastTradeTime { get; set; } // T
}

[XmlRoot("PriceBase")]
public class PriceBase
{
    [XmlElement("Id")]
    public int Id { get; set; }

    [XmlElement("Symbol")]
    public string Symbol { get; set; }

    [XmlElement("PriceValue")]
    public decimal PriceValue { get; set; }

    [XmlElement("Timestamp")]
    public DateTime Timestamp { get; set; }
}
