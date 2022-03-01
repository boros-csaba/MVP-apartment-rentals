using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MVPApartmentRentals.Models
{
    public class Apartment
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double FloorAreaSize { get; set; }
        [Column(TypeName = "decimal(18,3)")]
        public decimal PricePerMonth { get; set; }
        public int NumberOfRooms { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime AddedDate { get; set; }
        public ApartmentStatusEnum Status { get; set; }
        public User Realtor { get; set; }
    }
}
