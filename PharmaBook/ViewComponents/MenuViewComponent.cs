using Microsoft.AspNetCore.Mvc;

namespace PharmaBook.ViewComponents
{
    public class MenuViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            
            return View("Default");
        }
    }
}
