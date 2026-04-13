using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace FakeStore.WebApp.Service;

public interface IHomeService
{
    IActionResult RedirectToHomeTab(string tabKey);
    void SetFeedback(ITempDataDictionary tempData, string message, bool isError);
    string BuildHomeErrorMessage(string prefix, Exception ex);
}
