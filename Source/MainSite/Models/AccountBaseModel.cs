using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MainSite.Models
{
    public class AccountBaseModel
    {
        [Required]
        public string Email { get; set; }
        [Required, DataType(DataType.Password)]
        public string Password { get; set; }
        [Required, DataType(DataType.Password), Compare(nameof(Password))]
        public string Confirmation { get; set; }

        public AccountBaseModel(string email, string password, string confirmation)
        {
            Email = email;
            Password = password;
            Confirmation = confirmation;
        }

        public AccountBaseModel()
        {
        }
    }
}
