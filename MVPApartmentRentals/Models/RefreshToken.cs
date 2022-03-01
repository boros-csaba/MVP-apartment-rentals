using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MVPApartmentRentals.Models
{
    public class RefreshToken
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Token { get; set; }

        public string JwtTokenId { get; set; }

        public DateTime CreationDate { get; set; }

        public DateTime ExpiryDate { get; set; }

        public bool Invalidated { get; set; }
        public User User { get; set; }
    }
}
