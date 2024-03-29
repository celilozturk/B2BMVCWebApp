﻿using AutoMapper;
using Core.Helpers;
using Core.Models;
using Core.Models.Enums;
using Core.Repositories;
using Core.Services;
using Core.UnitOfWorks;
using Core.ViewModels;
using DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services
{
    public class OrderService : GenericService<Order>, IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IBasketRepository _basketRepository;
        private readonly IBasketItemRepository _basketItemRepository;
        private readonly IMapper _mapper;
        public OrderService(IGenericRepository<Order> repository, IUnitOfWork unitOfWork, IOrderRepository orderRepository, IMapper mapper, IBasketRepository basketRepository, IBasketItemRepository basketItemRepository, IOrderItemRepository orderItemRepository) : base(repository, unitOfWork)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
            _basketRepository = basketRepository;
            _basketItemRepository = basketItemRepository;
            _orderItemRepository = orderItemRepository;
        }

        public Task<Order> GetLastOrder()
        {
            return _orderRepository.GetLastOrder();
        }
        public async Task SaveOrderWithOrderItemsAsync(Order order)
        {
            var basket = await _basketRepository.GetBasketByUserId(order.AppUserId);
            var basketItems = await _basketItemRepository.GetBasketItemsWithProductByBasketId(basket.Id);
            await _orderRepository.AddAsync(order);
            await _unitOfWork.CommitAsync();
            List<OrderItem> orderItemList = new List<OrderItem>();

            foreach (var basketItem in basketItems)
            {
                OrderItem orderItem = new OrderItem();
                orderItem.OrderId = order.Id;
                orderItem.ProductId=basketItem.ProductId;
                orderItem.Quantity = basketItem.Quantity;
                orderItem.Price = basketItem.Product.Price;
                orderItemList.Add(orderItem);
            }
           await _orderItemRepository.AddRangeAsync(orderItemList);
            
            _basketItemRepository.RemoveRange(basketItems);
            await _unitOfWork.CommitAsync();
        }

        public async Task<Order> CheckAndCreateOrder(OrderVM orderVM)
        {
            if (orderVM.Address == null || orderVM.Address == "" || orderVM.City == null || orderVM.PaymentMethod == null)
                return null;
            var order = _mapper.Map<Order>(orderVM);
            order.Status = Status.InProgress;
            order.Date = DateTime.Now;
            order.OrderNumber = await Generate();
            await SaveOrderWithOrderItemsAsync(order);
            return order;
        }
        public async Task<string> Generate()
        {
            var order = await GetLastOrder();
            if (order != null)
            {
                var orderNumber = order.OrderNumber;
                string value = order.OrderNumber.Substring(2, orderNumber.Length - 2);
                int newNumber = Int32.Parse(value) + 1;
                var newOrderNumber = "ON";
                for (int i = 0; i < 18 - newNumber.ToString().Length; i++)
                {
                    newOrderNumber += "0";
                }
                newOrderNumber += newNumber.ToString();
                return newOrderNumber;
            }
            return "ON000000000000000001";
        }

        public async Task<List<Order>> GetOrdersWithUser()
        {
            return await _orderRepository.GetOrdersWithUser();
        }

        public async Task<Order> GetOrderWithOrderItemsByOrderId(int orderId)
        {
            return await _orderRepository.GetOrderWithOrderItemsByOrderId(orderId);
        }

        public async Task<Order> ChangeOrderStatus(Order order)
        {
            switch (order.Status)
            {
                case Status.InProgress: order.Status = Status.Confirmed;
                    break;
                case Status.Confirmed: order.Status = Status.Shipped;
                    break;  
                default:
                    break;
            }
             _orderRepository.Update(order);
            _unitOfWork.Commit();
            return order;
        }

        public async Task<Order> CancelOrder(Order order)
        {
            if (order.Status==Status.InProgress)
            {
                order.Status = Status.Cancelled;
            }
            _orderRepository.Update(order);
            _unitOfWork.Commit();
            return order;
        }  
    }
}
