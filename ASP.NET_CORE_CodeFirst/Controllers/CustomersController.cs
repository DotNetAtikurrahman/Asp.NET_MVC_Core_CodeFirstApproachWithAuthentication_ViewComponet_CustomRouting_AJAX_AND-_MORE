using ASP.NET_CORE_CodeFirst.Data;
using ASP.NET_CORE_CodeFirst.Models;
using ASP.NET_CORE_CodeFirst.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ASP.NET_CORE_CodeFirst.Controllers
{
    
    public class CustomersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        public CustomersController(ApplicationDbContext _context, IWebHostEnvironment _env)
        {
            this._context = _context;
            this._env = _env;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _context.Customers.Include(x => x.TransactionDetails).ThenInclude(y => y.Product).ToListAsync());
        }
        public IActionResult AddNewProduct(int? id)
        {
            ViewBag.product = new SelectList(_context.Products, "ProductId", "ProductName", id.ToString() ?? "");
            return PartialView("_addNewProduct");
        }
        [Authorize(Roles = "SuperAdmin,Admin")]
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Create(ClientVM clientVM, int[] productId)
        {
            if (ModelState.IsValid)
            {
                Customer customer = new Customer()
                {
                    CustomerId = clientVM.CustomerId,
                    CustomerName = clientVM.CustomerName,
                    Phone = clientVM.Phone,
                    Address = clientVM.Address,
                    PurchaseDate = clientVM.PurchaseDate,
                    TotalBill = clientVM.TotalBill,
                    IsPaid = clientVM.IsPaid,

                };

                //For Image
                var file = clientVM.PictureFile;
                string webroot = _env.WebRootPath;
                string folder = "Images";
                string ext = Path.GetExtension(clientVM!.PictureFile!.FileName);
                string imgFileName = Path.GetRandomFileName() + ext;
                string fileSave = Path.Combine(webroot, folder, imgFileName);

                if (file != null)
                {
                    using (var stream = new FileStream(fileSave, FileMode.Create))
                    {
                        await clientVM.PictureFile.CopyToAsync(stream);
                        customer.Picture = "/" + folder + "/" + imgFileName;
                    }
                }

                //For Products entry 
                foreach (var item in productId)
                {
                    TransactionDetail transactionDetail = new TransactionDetail()
                    {
                        Customer = customer,
                        CustomerId = customer.CustomerId,
                        ProductId = item
                    };
                    _context.TransactionDetails.Add(transactionDetail);
                }
                await _context.SaveChangesAsync();
                return PartialView("_success");
            }
            return PartialView("_error");
        }
        [Authorize(Roles = "SuperAdmin")]
        [Route("ohbrotheronlysuperadmin/canedit")]
        public async Task<IActionResult> Edit(int? id)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(x => x.CustomerId == id);
            ClientVM clientVM = new ClientVM()
            {
                CustomerId = customer!.CustomerId,
                CustomerName = customer.CustomerName,
                Picture = customer.Picture,
                Phone = customer.Phone,
                Address = customer.Address,
                PurchaseDate = customer.PurchaseDate,
                TotalBill = customer.TotalBill,
                IsPaid = customer.IsPaid,
            };
            //Product remove
            var oldProduct = _context.TransactionDetails.Where(x => x.CustomerId == id).ToList();
            foreach (var item in oldProduct)
            {
                clientVM.ProductList.Add(item.ProductId);
            }
            return View(clientVM);
        }

        [HttpPost]
        [Route("ohbrotheronlysuperadmin/canedit")]  // custom attribute routing
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ClientVM clientVM, int[] productId)
        {
            if (ModelState.IsValid)
            {
                Customer customer = new Customer()
                {
                    CustomerId = clientVM.CustomerId,
                    CustomerName = clientVM.CustomerName,
                    Phone = clientVM.Phone,
                    Address = clientVM.Address,
                    PurchaseDate = clientVM.PurchaseDate,
                    TotalBill = clientVM.TotalBill,
                    IsPaid = clientVM.IsPaid,

                };

                //Image
                var file = clientVM.PictureFile;
                var oldPic = clientVM.Picture;
                if (file != null)
                {
                    string webroot = _env.WebRootPath;
                    string folder = "Images";
                    string ext = Path.GetExtension(clientVM!.PictureFile!.FileName);
                    string imgFileName = Path.GetRandomFileName() + ext;
                    string fileSave = Path.Combine(webroot, folder, imgFileName);

                    using (var stream = new FileStream(fileSave, FileMode.Create))
                    {
                        await clientVM.PictureFile.CopyToAsync(stream);
                        customer.Picture = "/" + folder + "/" + imgFileName;
                    }
                }
                
                else
                {
                    customer.Picture = oldPic;
                }

                //Product delete
                var exitProduct = _context.TransactionDetails.Where(x => x.CustomerId == customer.CustomerId).ToList();
                foreach (var item in exitProduct)
                {
                    _context.TransactionDetails.Remove(item);
                }

                //add new product 
                foreach (var item in productId)
                {
                    TransactionDetail transactionDetail = new TransactionDetail()
                    {
                        CustomerId = customer.CustomerId,
                        ProductId = item
                    };
                    _context.TransactionDetails.Add(transactionDetail);
                }
                _context.Update(customer);
                await _context.SaveChangesAsync();
                return PartialView("_success");
            }
            return PartialView("_error");
        }
        
       [Authorize(Roles = "SuperAdmin")]
        
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(x => x.CustomerId == id);

            if (customer == null)
            {
                return NotFound();
            }

            // Initialize the ViewModel
            ClientVM clientVM = new ClientVM()
            {
                CustomerId = customer.CustomerId,
                CustomerName = customer.CustomerName,
                Picture = customer.Picture,
                Phone = customer.Phone,
                Address = customer.Address,
                PurchaseDate = customer.PurchaseDate,
                TotalBill = customer.TotalBill,
                IsPaid = customer.IsPaid,
                ProductList = new List<int>() // Ensure list is initialized
            };

            // Load associated product IDs
            var associatedProducts = await _context.TransactionDetails
                .Where(x => x.CustomerId == id)
                .ToListAsync();

            foreach (var item in associatedProducts)
            {
                clientVM.ProductList.Add(item.ProductId);
            }

            return View(clientVM);
        }

        // POST: Customers/Delete/5
        [HttpPost]
        
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
            {
                return PartialView("_error"); // Using existing partial error view
            }

            try
            {
                // 1. Remove related TransactionDetails first
                var exitProducts = _context.TransactionDetails.Where(x => x.CustomerId == id);
                _context.TransactionDetails.RemoveRange(exitProducts);

                // 2. Optional: Delete the physical image file from the server
                if (!string.IsNullOrEmpty(customer.Picture))
                {
                    var imagePath = Path.Combine(_env.WebRootPath, customer.Picture.TrimStart('/'));
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }

                // 3. Remove the customer 
                _context.Customers.Remove(customer);

                // 4. Save changes
                await _context.SaveChangesAsync();

                // Since your View uses data-ajax="true", return the success partial
                return PartialView("_success");
            }
            catch (Exception)
            {
                return PartialView("_error");
            }
        }
    }
}
