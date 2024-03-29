﻿using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Repositories
{
    public interface IBasketItemRepository:IGenericRepository<BasketItem>
    {
        Task<List<BasketItem>> GetBasketItemsWithProductByBasketId(int basketId);
        Task<int> GetBasketItemsCountByBasketId(int basketId);
        
    }
}
