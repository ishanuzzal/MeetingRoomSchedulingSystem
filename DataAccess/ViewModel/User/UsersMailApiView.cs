using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.ViewModel.User
{
    public class UsersMailApiView
    {
        [Required]
        public string Id { get; set; } = null!;
        [Required]
        public string Email { get; set; } = null!;

    }
}
