using Microsoft.AspNetCore.Identity;

namespace ASP.NET_CORE_CodeFirst.Data
{
    public class ApplicationUser :IdentityUser
    {
        public string? Name { get; set; }
        public string? CellPhoneNo { get; set; }
        public string? Location { get; set; }

    }
}
