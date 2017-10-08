using Microsoft.AspNetCore.Mvc;

namespace PharmaBook.ViewComponents
{
    public class HeaderViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {

            return View("Default");
        }
    }
}
