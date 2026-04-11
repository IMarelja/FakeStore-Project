using Microsoft.AspNetCore.Mvc;

namespace FakeStore.WebApp.Controllers
{
    public class ProductController : Controller
    {
        // GET: ProductController
        public ActionResult Index()
        {
            return View();
        }

    }
}
