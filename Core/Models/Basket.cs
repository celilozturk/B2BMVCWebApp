﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class Basket:BaseEntity
    {
        public string  AppUserId { get; set; }
        public AppUser AppUser { get; set; }
        public List<BasketItem> BasketItems { get; set; }
    }
}
