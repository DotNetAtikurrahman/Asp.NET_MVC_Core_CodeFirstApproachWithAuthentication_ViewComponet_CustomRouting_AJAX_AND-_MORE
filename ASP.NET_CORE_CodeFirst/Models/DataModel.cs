using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASP.NET_CORE_CodeFirst.Models
{
    public partial class Product
    {
        public int ProductId { get; set; }
        [Display(Name = "Product Name")]
        public string? ProductName { get; set; }


        public virtual ICollection<TransactionDetail> TransactionDetails { get; set; } = new List<TransactionDetail>();
    }

    public partial class Customer
    {
        public int CustomerId { get; set; }
        [Display(Name = "Customer Name"), Required]
        public string? CustomerName { get; set; }
        public string? Picture { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        [Display(Name = "Purchased Date"), Required, Column(TypeName = "date"), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public System.DateTime PurchaseDate { get; set; }
        [Display(Name = "Total Bill")]
        public double TotalBill { get; set; }
        public bool IsPaid { get; set; }

        public virtual ICollection<TransactionDetail> TransactionDetails { get; set; } = new List<TransactionDetail>();
    }

    public partial class TransactionDetail
    {
        public int TransactionDetailId { get; set; }
        [Display(Name = "Customer"), Required]
        public int CustomerId { get; set; }
        [Display(Name = "Product"), Required]

        public int ProductId { get; set; }

        public virtual Customer? Customer { get; set; }
        public virtual Product? Product { get; set; }
    }
}
