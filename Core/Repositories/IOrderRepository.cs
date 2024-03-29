﻿using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Repositories
{
    public interface IOrderRepository:IGenericRepository<Order>
    {
        Task<Order> GetLastOrder();
        Task<List<Order>> GetOrdersWithUser();
        Task<Order> GetOrderWithOrderItemsByOrderId(int orderId);
    }
}
