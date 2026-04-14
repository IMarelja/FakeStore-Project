using System.Xml.Serialization;

namespace MySoap.Models;

[XmlRoot("Product")]
[XmlType("Product")]
public class ReadProductXml
{
    [XmlElement("product_id")]
    public int product_id { get; set; }

    [XmlElement("name")]
    public string name { get; set; } = string.Empty;

    [XmlElement("description")]
    public string description { get; set; } = string.Empty;

    [XmlElement("price")]
    public double price { get; set; }

    [XmlElement("unit")]
    public string unit { get; set; } = string.Empty;

    [XmlElement("image")]
    public string image { get; set; } = string.Empty;

    [XmlElement("discount")]
    public int discount { get; set; }

    [XmlElement("availability")]
    public bool availability { get; set; }

    [XmlElement("brand")]
    public string brand { get; set; } = string.Empty;

    [XmlElement("category")]
    public string category { get; set; } = string.Empty;

    [XmlElement("rating")]
    public double rating { get; set; }
}
