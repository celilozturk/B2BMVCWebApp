﻿using AutoMapper;
using Core.Models;
using Core.Repositories;
using Core.Services;
using Core.UnitOfWorks;
using Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Business.Services
{
    public class BasketItemService : GenericService<BasketItem>, IBasketItemService
    {
        private readonly IBasketItemRepository _basketItemRepository;
        private readonly IBasketRepository _basketRepository;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        public BasketItemService(IGenericRepository<BasketItem> repository, IUnitOfWork unitOfWork, IMapper mapper, IBasketRepository basketRepository, IProductRepository productRepository, IBasketItemRepository basketItemRepository) : base(repository, unitOfWork)
        {
            _mapper = mapper;
            _basketRepository = basketRepository;
            _productRepository = productRepository;
            _basketItemRepository = basketItemRepository;
        }

        public async Task<bool> CheckBasketItemPropertiesOnDB(BasketItemVM basketItemVM)
        {
            if (!await _productRepository.AnyAsync(p => p.Id == basketItemVM.ProductId))
            {
                return false;
            }          
            if (!await _basketRepository.AnyAsync(b => b.Id == basketItemVM.BasketId))
            {
                return false;
            }
            return true;
        }
        
        public async Task<List<BasketItemVM>> GetBasketItemsWithProductByBasketId(int basketId)
        {
            var basketItems=await _basketItemRepository.GetBasketItemsWithProductByBasketId(basketId);
            return _mapper.Map<List<BasketItemVM>>(basketItems);
        }

        public async Task<int> GetBasketItemsCountByUserId(string userId)
        {
            //var basket=await _basketRepository.GetBasketByUserId(userId);
            //int itemCount = await _basketItemRepository.GetBasketItemsCountByBasketId(basket.Id);
            try
            {
                var basket = await _basketRepository.GetBasketByUserId(userId);
                if (basket!=null)
                {
                    int itemCount = await _basketItemRepository.GetBasketItemsCountByBasketId(basket.Id);
                    return itemCount;
                }
               
            }
            catch (Exception)
            {

            }
            return 0;
        }

        public async Task RemoveBasketItemsFromBasket(string appUserId)
        {
            var basket = await _basketRepository.GetBasketByUserId(appUserId);
            
            var basketItems = await _basketItemRepository.Where(b => b.BasketId == basket.Id).ToListAsync();
             _basketItemRepository.RemoveRange(basketItems);
             await _unitOfWork.CommitAsync();
        }

    }
}
