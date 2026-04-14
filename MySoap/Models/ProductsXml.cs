using System.Xml.Serialization;

namespace MySoap.Models;

[XmlRoot("Products")]
public class ProductsXml
{
    [XmlElement("Product")]
    public List<ReadProductXml> Products { get; set; } = [];
}
