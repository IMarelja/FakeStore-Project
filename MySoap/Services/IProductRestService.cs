using System.ServiceModel;
using FakeStore.ViewModel;
using MySoap.Models;

namespace MySoap.Services;

public interface IProductRestService
{
    Task<List<ReadProductXml>> GetAll();
    Task<bool> VerifyXmlFile();
    Task ToXmlFile(List<ReadProductXml> products);
}
