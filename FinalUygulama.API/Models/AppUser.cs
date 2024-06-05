using Microsoft.AspNetCore.Identity;

namespace FinalUygulama.API.Models
{
    public class AppUser : IdentityUser
    {
        public string FullName { get; set; }
        public string PicUrl { get; set; }
    }
}
