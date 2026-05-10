using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASP.NET_CORE_CodeFirst.Models.ViewModel
{
    public class ClientVM
    {
        public int CustomerId { get; set; }
        [Display(Name = "Customer Name"), Required]
        public string? CustomerName { get; set; }
        public string? Picture { get; set; }
        [Display(Name = "Profile Photo")]
        public IFormFile? PictureFile { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        [Display(Name = "Purchased Date"), Required, Column(TypeName = "date"), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public System.DateTime PurchaseDate { get; set; }
        [Display(Name = "Total Bill")]
        public double TotalBill { get; set; }
        public bool IsPaid { get; set; }
        public List<int> ProductList { get; set; } = new List<int>();
    }
}
