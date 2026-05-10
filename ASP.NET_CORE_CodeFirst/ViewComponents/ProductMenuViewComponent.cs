using ASP.NET_CORE_CodeFirst.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASP.NET_CORE_CodeFirst.ViewComponents
{
    public class ProductMenuViewComponent : ViewComponent
    {

        private readonly ApplicationDbContext _context;
        public ProductMenuViewComponent(ApplicationDbContext _context)
        {
            this._context = _context;

        }
        public async Task<IViewComponentResult> InvokeAsync(int max = 10)
        {
            var products = await _context.Products.Take(max).OrderByDescending(x => x.ProductName).ToListAsync();
            return View(products);
        }


    }


}
