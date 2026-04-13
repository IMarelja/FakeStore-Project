using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace FakeStore.WebApp.Service;

public class HomeService : IHomeService
{
    private const string HomeFeedbackMessageKey = "HomeFeedbackMessage";
    private const string HomeFeedbackIsErrorKey = "HomeFeedbackIsError";

    public IActionResult RedirectToHomeTab(string tabKey)
    {
        return new RedirectToActionResult("Index", "Home", new { tab = tabKey });
    }

    public void SetFeedback(ITempDataDictionary tempData, string message, bool isError)
    {
        tempData[HomeFeedbackMessageKey] = message;
        tempData[HomeFeedbackIsErrorKey] = isError;
    }

    public string BuildHomeErrorMessage(string prefix, Exception ex)
    {
        if (ex is HttpRequestException requestException && !string.IsNullOrWhiteSpace(requestException.Message))
        {
            return $"{prefix} {requestException.Message}";
        }

        return prefix;
    }
}
