﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVPApartmentRentals.Models
{
    public class ApartmentFilter: PaginationFilter
    {
        public string UserId { get; set; }
        public double? MinArea { get; set; }
        public double? MaxArea { get; set; }
        public int? MinRooms { get; set; }
        public int? MaxRooms { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public bool OnlyAvailable { get; set; }
    }
}
