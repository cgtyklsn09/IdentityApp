using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace IdentityApp.Models
{
    public class AppUser : IdentityUser
    {
        public string? FullName { get; set; }
    }
}
