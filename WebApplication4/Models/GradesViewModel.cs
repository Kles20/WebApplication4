using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using WebApplication1.Models;


namespace WebApplication1.Models
{
    public class GradesViewModel
    {
        public List<Subject> Subjects { get; set; }
        public List<IdentityUser> Users { get; set; }
    }
}