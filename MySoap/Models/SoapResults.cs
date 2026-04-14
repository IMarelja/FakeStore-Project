using System.Runtime.Serialization;

namespace MySoap.Models;

[DataContract]
public class ProductSoap
{
    [DataMember] public int Id { get; set; }
    [DataMember] public string Name { get; set; } = string.Empty;
    [DataMember] public string Description { get; set; } = string.Empty;
    [DataMember] public decimal Price { get; set; }
    [DataMember] public string Category { get; set; } = string.Empty;
    [DataMember] public string Brand { get; set; } = string.Empty;
    [DataMember] public double Rating { get; set; }
}

[DataContract]
public class SearchResult
{
    [DataMember] public List<ProductSoap> Products { get; set; } = [];
    [DataMember] public int Count { get; set; }
}
