using GreeenGarden.Business.Utilities.TokenService;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Enums;
using GreeenGarden.Data.Models.CartModel;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Repositories.CartRepo;
using GreeenGarden.Data.Repositories.OrderRepo;

namespace GreeenGarden.Business.Service.CartService
{
    public class CartService : ICartService
    {
        private readonly DecodeToken _decodeToken;
        private readonly ICartRepo _cartRepo;
        private readonly IOrderRepo _orderRepo;

        public CartService(ICartRepo cartRepo, IOrderRepo orderRepo)
        {
            _cartRepo = cartRepo;
            _decodeToken = new DecodeToken();
            _orderRepo = orderRepo;
        }

        public async Task<ResultModel> AddToCart(string token, AddToCartModel model)
        {
            var result = new ResultModel();
            try
            {
                if (model.rentItems.Count > 1)
                {
                    for (int i = 0; i < model.rentItems.Count; i++)
                    {
                        for (int j = 1; j < model.rentItems.Count; j++)
                        {
                            if (model.rentItems[i].sizeProductItemID.Equals(model.rentItems[j].sizeProductItemID))
                            {
                                model.rentItems[i].quantity += model.rentItems[j].quantity;
                                model.rentItems[j].sizeProductItemID = null;
                            }
                        }

                    }
                }
                if (model.saleItems.Count > 1)
                {
                    for (int i = 0; i < model.saleItems.Count; i++)
                    {
                        for (int j = 1; j < model.saleItems.Count; j++)
                        {
                            if (model.saleItems[i].sizeProductItemID.Equals(model.saleItems[j].sizeProductItemID) && i != j)
                            {
                                model.saleItems[i].quantity += model.saleItems[j].quantity;
                                model.saleItems[j].sizeProductItemID = null;
                            }
                        }
                    }
                }
                // check sale + rent
                foreach (var i in model.saleItems)
                {
                    foreach (var j in model.rentItems)
                    {
                        if (i.sizeProductItemID == j.sizeProductItemID)
                        {
                            var sizeProItem = await _orderRepo.GetSizeProductItem((Guid)i.sizeProductItemID);
                            if (sizeProItem == null)
                            {
                                result.IsSuccess = false;
                                result.Message = "Don't product: " + i.sizeProductItemID;
                                return result;

                            }
                            if ((i.quantity + j.quantity) > sizeProItem.Quantity)
                            {
                                result.IsSuccess = false;
                                result.Message = "Số lượng của sản phẩm: " + i.sizeProductItemID + " trong kho chỉ còn: " + sizeProItem.Quantity + ", đơn hàng của bạn tổng: " + (i.quantity + j.quantity);
                                return result;
                            }
                        }
                    }
                }


                double? totalRentPriceCart = 0;
                double? totalSalePriceCart = 0;
                var modelResponse = new CartShowModel();
                var user = await _cartRepo.GetByUserName(_decodeToken.Decode(token, "username"));
                if (await _cartRepo.GetCart(user.Id) == null)
                {
                    var cartTemp = new TblCart()
                    {
                        Id = Guid.NewGuid(),
                        UserId = user.Id,
                        Status = Status.ACTIVE,
                    };
                    await _cartRepo.Insert(cartTemp);
                }



                var cart = await _cartRepo.GetCart(user.Id);
                var cartDetail = await _cartRepo.GetListCartDetail(cart.Id);
                foreach (var item in cartDetail)
                {
                    await _cartRepo.RemoveCartDetail(item);
                }

                modelResponse.rentItems = new List<ItemRequest>();
                modelResponse.saleItems = new List<ItemRequest>();

                if (model.rentItems != null)
                {
                    foreach (var item in model.rentItems)
                    {
                        if (item.sizeProductItemID != null)
                        {
                            var sizeProductItem = await _cartRepo.GetSizeProductItem(item.sizeProductItemID);
                            if (item.quantity > sizeProductItem.Quantity)
                            {
                                result.IsSuccess = false;
                                result.Message = "Product " + sizeProductItem.Id + " don't enough quantity!";
                                return result;
                            }
                            if (sizeProductItem.Status.ToLower() != Status.ACTIVE || sizeProductItem.RentPrice == 0)
                            {
                                result.Code = 400;
                                result.IsSuccess = false;
                                result.Message = "Sản phẩm " + item.sizeProductItemID + " đang bị vô hiệu!";
                                return result;
                            }
                            var newCartDetail = new TblCartDetail()
                            {
                                Id = Guid.NewGuid(),
                                ProductItemDetailId = item.sizeProductItemID,
                                Quantity = item.quantity,
                                CartId = cart.Id,
                                IsForRent = true
                            };
                            await _cartRepo.AddProductItemToCart(newCartDetail);
                            //show
                            var productItem = await _cartRepo.GetProductItem(sizeProductItem.ProductItemId);

                            var ItemRequest = new ItemRequest()
                            {
                                sizeProductItem = sizeProductItem,
                                quantity = item.quantity,
                                unitPrice = sizeProductItem.RentPrice,
                                productItem = productItem
                            };
                            modelResponse.rentItems.Add(ItemRequest);
                            totalRentPriceCart += sizeProductItem.RentPrice * item.quantity;
                        }
                    }
                }

                if (model.saleItems != null)
                {
                    foreach (var item in model.saleItems)
                    {
                        if (item.sizeProductItemID != null)
                        {
                            var sizeProductItem = await _cartRepo.GetSizeProductItem(item.sizeProductItemID);

                            if (item.quantity > sizeProductItem.Quantity)
                            {
                                result.IsSuccess = false;
                                result.Message = "Product " + sizeProductItem.Id + " don't enough quantity!";
                                return result;
                            }
                            if (sizeProductItem.Status.ToLower() != Status.ACTIVE || sizeProductItem.SalePrice == 0)
                            {
                                result.Code = 400;
                                result.IsSuccess = false;
                                result.Message = "Sản phẩm " + item.sizeProductItemID + " đang bị vô hiệu!";
                                return result;
                            }
                            var newCartDetail = new TblCartDetail()
                            {
                                Id = Guid.NewGuid(),
                                ProductItemDetailId = item.sizeProductItemID,
                                Quantity = item.quantity,
                                CartId = cart.Id,
                                IsForRent = false
                            };
                            await _cartRepo.AddProductItemToCart(newCartDetail);
                            //show
                            var productItem = await _cartRepo.GetProductItem(sizeProductItem.ProductItemId);
                            var ItemRequest = new ItemRequest()
                            {
                                sizeProductItem = sizeProductItem,
                                quantity = item.quantity,
                                unitPrice = sizeProductItem.SalePrice,
                                productItem = productItem
                            };
                            modelResponse.saleItems.Add(ItemRequest);
                            totalSalePriceCart += sizeProductItem.SalePrice * item.quantity;
                        }

                    }
                }

                modelResponse.totalRentPrice = totalRentPriceCart;
                modelResponse.totalSalePrice = totalSalePriceCart;
                modelResponse.totalPrice = totalSalePriceCart + totalRentPriceCart;

                result.Code = 200;
                result.IsSuccess = true;
                result.Data = modelResponse;
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> GetCart(string token)
        {
            var result = new ResultModel();
            try
            {
                var modelResponse = new CartShowModel();
                modelResponse.rentItems = new List<ItemRequest>();
                modelResponse.saleItems = new List<ItemRequest>();
                double? totalRentPriceCart = 0;
                double? totalSalePriceCart = 0;
                var user = await _cartRepo.GetByUserName(_decodeToken.Decode(token, "username"));
                var cart = await _cartRepo.GetCart(user.Id);
                if (cart == null)
                {
                    result.Code = 200;
                    result.IsSuccess = true;
                    result.Data = null;
                    return result;
                }
                var listCartDetail = await _cartRepo.GetListCartDetail(cart.Id);
                if (listCartDetail.Count == 0)
                {
                    result.Code = 200;
                    result.IsSuccess = true;
                    result.Data = null;
                    return result;
                }
                foreach (var item in listCartDetail)
                {
                    if (item.IsForRent == true)
                    {
                        var sizeProductItem = await _cartRepo.GetSizeProductItem(item.ProductItemDetailId);
                        var productItem = await _cartRepo.GetProductItem(sizeProductItem.ProductItemId);
                        var ItemRequest = new ItemRequest()
                        {
                            sizeProductItem = sizeProductItem,
                            quantity = item.Quantity,
                            unitPrice = sizeProductItem.RentPrice,
                            productItem = productItem
                        };
                        modelResponse.rentItems.Add(ItemRequest);
                        totalRentPriceCart += sizeProductItem.RentPrice * item.Quantity;
                    }
                    if (item.IsForRent == false)
                    {
                        var sizeProductItem = await _cartRepo.GetSizeProductItem(item.ProductItemDetailId);
                        var productItem = await _cartRepo.GetProductItem(sizeProductItem.ProductItemId);
                        var ItemRequest = new ItemRequest()
                        {
                            sizeProductItem = sizeProductItem,
                            quantity = item.Quantity,
                            unitPrice = sizeProductItem.SalePrice,
                            productItem = productItem
                        };
                        modelResponse.saleItems.Add(ItemRequest);
                        totalSalePriceCart += sizeProductItem.SalePrice * item.Quantity;
                    }
                }
                modelResponse.totalRentPrice = totalRentPriceCart;
                modelResponse.totalSalePrice = totalSalePriceCart;
                modelResponse.totalPrice = totalSalePriceCart + totalRentPriceCart;

                result.Code = 200;
                result.IsSuccess = true;
                result.Data = modelResponse;
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;

        }
    }
}
