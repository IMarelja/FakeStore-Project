using System.ServiceModel;
using FakeStore.ViewModel;

namespace MySoap.Services;

public interface IProductRestService
{
    Task<List<ProductRead>> GetAll();
    Task<bool> VerifyXmlFile();
    Task ToXmlFile(List<ProductRead> products);
}
