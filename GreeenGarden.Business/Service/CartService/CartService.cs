using GreeenGarden.Business.Utilities.TokenService;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Enums;
using GreeenGarden.Data.Models.CartModel;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Repositories.CartRepo;

namespace GreeenGarden.Business.Service.CartService
{
    public class CartService : ICartService
    {
        private readonly DecodeToken _decodeToken;
        private readonly ICartRepo _cartRepo;

        public CartService(ICartRepo cartRepo)
        {
            _cartRepo = cartRepo;
            _decodeToken = new DecodeToken();
        }

        public async Task<ResultModel> AddToCart(string token, AddToCartModel model)
        {
            ResultModel result = new();
            try
            {
                if (model.rentItems.Count > 1)
                {
                    for (int i = 0; i < model.rentItems.Count; i++)
                    {
                        for (int j = 1; j < model.rentItems.Count; j++)
                        {
                            if (model.rentItems[i].productItemDetailID.Equals(model.rentItems[j].productItemDetailID) && i != j)
                            {
                                model.rentItems[i].quantity += model.rentItems[j].quantity;
                                model.rentItems[j].productItemDetailID = null;
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
                            if (model.saleItems[i].productItemDetailID.Equals(model.saleItems[j].productItemDetailID) && i != j)
                            {
                                model.saleItems[i].quantity += model.saleItems[j].quantity;
                                model.saleItems[j].productItemDetailID = null;
                            }
                        }
                    }
                }
                // check sale + rent
                foreach (ItemResponse i in model.saleItems)
                {
                    foreach (ItemResponse j in model.rentItems)
                    {
                        if (i.productItemDetailID == j.productItemDetailID)
                        {
                            TblProductItemDetail proItemDetial = await _cartRepo.GetProductItemDetails((Guid)i.productItemDetailID);
                            if (proItemDetial == null)
                            {
                                result.Code = 102;
                                result.IsSuccess = false;
                                result.Message = "Don't product: " + i.productItemDetailID;
                                return result;

                            }
                            if ((i.quantity + j.quantity) > proItemDetial.Quantity)
                            {
                                result.Code = 101;
                                result.IsSuccess = false;
                                result.Message = "Số lượng của sản phẩm: " + i.productItemDetailID + " trong kho chỉ còn: " + proItemDetial.Quantity + ", đơn hàng của bạn tổng: " + (i.quantity + j.quantity);
                                return result;
                            }
                        }
                    }
                }


                double? totalRentPriceCart = 0;
                double? totalSalePriceCart = 0;
                CartShowModel modelResponse = new();
                TblUser user = await _cartRepo.GetByUserName(_decodeToken.Decode(token, "username"));
                if (await _cartRepo.GetCart(user.Id) == null)
                {
                    TblCart cartTemp = new()
                    {
                        Id = Guid.NewGuid(),
                        UserId = user.Id,
                        Status = Status.ACTIVE,
                    };
                    _ = await _cartRepo.Insert(cartTemp);
                }



                TblCart cart = await _cartRepo.GetCart(user.Id);
                if (cart == null)
                {
                    result.IsSuccess = false;
                    result.Message = "sai cho nay ne";
                    return result;

                }
                List<TblCartDetail> cartDetail = await _cartRepo.GetListCartDetail(cart.Id);
                foreach (TblCartDetail item in cartDetail)
                {
                    _ = await _cartRepo.RemoveCartDetail(item);
                }

                modelResponse.rentItems = new List<ItemRequest>();
                modelResponse.saleItems = new List<ItemRequest>();

                if (model.rentItems != null)
                {
                    if (model.rentItems.Count != 0)
                    {
                        foreach (ItemResponse item in model.rentItems)
                        {
                            if (item.productItemDetailID != null)
                            {
                                productItemDetail productItemDetail = await _cartRepo.GetProductItemDetail(item.productItemDetailID);
                                if (item.quantity > productItemDetail.Quantity)
                                {
                                    result.Code = 101;
                                    result.IsSuccess = false;
                                    result.Message = "Product " + productItemDetail.Id + " don't enough quantity!";
                                    return result;
                                }
                                if (productItemDetail.Status.ToLower() != Status.ACTIVE || productItemDetail.RentPrice == 0)
                                {
                                    result.Code = 102;
                                    result.IsSuccess = false;
                                    result.Message = "Sản phẩm " + item.productItemDetailID + " đang bị vô hiệu!";
                                    return result;
                                }
                                TblCartDetail newCartDetail = new()
                                {
                                    Id = Guid.NewGuid(),
                                    SizeProductItemId = item.productItemDetailID,
                                    Quantity = item.quantity,
                                    CartId = cart.Id,
                                    IsForRent = true
                                };
                                _ = await _cartRepo.AddProductItemToCart(newCartDetail);
                                //show
                                TblProductItem productItem = await _cartRepo.GetProductItem(productItemDetail.ProductItemId);
                                productItem productItemRecord = new()
                                {
                                    Content = productItem.Content,
                                    Description = productItem.Description,
                                    Id = productItem.Id,
                                    Name = productItem.Name,
                                    Type = productItem.Type
                                };


                                ItemRequest ItemRequest = new()
                                {
                                    productItemDetail = productItemDetail,
                                    quantity = item.quantity,
                                    unitPrice = productItemDetail.RentPrice,
                                    productItem = productItemRecord
                                };
                                modelResponse.rentItems.Add(ItemRequest);
                                totalRentPriceCart += productItemDetail.RentPrice * item.quantity;
                            }
                        }

                    }
                }

                if (model.saleItems != null)
                {
                    if (model.saleItems.Count != 0)
                    {
                        foreach (ItemResponse item in model.saleItems)
                        {
                            if (item.productItemDetailID != null)
                            {
                                productItemDetail productItemDetail = await _cartRepo.GetProductItemDetail(item.productItemDetailID);

                                if (item.quantity > productItemDetail.Quantity)
                                {
                                    result.Code = 101;
                                    result.IsSuccess = false;
                                    result.Message = "Product " + productItemDetail.Id + " don't enough quantity!";
                                    return result;
                                }
                                if (productItemDetail.Status.ToLower() != Status.ACTIVE || productItemDetail.SalePrice == 0)
                                {
                                    result.Code = 102;
                                    result.IsSuccess = false;
                                    result.Message = "Sản phẩm " + item.productItemDetailID + " đang bị vô hiệu!";
                                    return result;
                                }
                                TblCartDetail newCartDetail = new()
                                {
                                    Id = Guid.NewGuid(),
                                    SizeProductItemId = item.productItemDetailID,
                                    Quantity = item.quantity,
                                    CartId = cart.Id,
                                    IsForRent = false
                                };
                                _ = await _cartRepo.AddProductItemToCart(newCartDetail);
                                //show
                                TblProductItem productItem = await _cartRepo.GetProductItem(productItemDetail.ProductItemId);
                                productItem productItemRecord = new()
                                {
                                    Content = productItem.Content,
                                    Description = productItem.Description,
                                    Id = productItem.Id,
                                    Name = productItem.Name,
                                    Type = productItem.Type
                                };
                                ItemRequest ItemRequest = new()
                                {
                                    productItemDetail = productItemDetail,
                                    quantity = item.quantity,
                                    unitPrice = productItemDetail.SalePrice,
                                    productItem = productItemRecord
                                };
                                modelResponse.saleItems.Add(ItemRequest);
                                totalSalePriceCart += productItemDetail.SalePrice * item.quantity;
                            }
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

        public async Task<ResultModel> CleanCart(string token)
        {
            ResultModel result = new();
            try
            {
                TblUser tblUser = await _cartRepo.GetByUserName(_decodeToken.Decode(token, "username"));
                TblCart cart = await _cartRepo.GetCart(tblUser.Id);
                if (cart == null)
                {
                    result.Code = 201;
                    result.IsSuccess = true;
                    result.Data = "Xóa thành công";
                    return result;
                }
                List<TblCartDetail> cartDetail = await _cartRepo.GetListCartDetail(cart.Id);
                if (cartDetail == null || cartDetail.Count == 0)
                {
                    result.Code = 201;
                    result.IsSuccess = true;
                    result.Data = "Xóa thành công";
                    return result;
                }

                foreach (TblCartDetail i in cartDetail)
                {
                    _ = await _cartRepo.RemoveCartDetail(i);
                }



                result.Code = 201;
                result.IsSuccess = true;
                result.Message = "Xóa thành công";
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> CleanRentCart(string token)
        {
            ResultModel result = new();
            try
            {
                TblUser tblUser = await _cartRepo.GetByUserName(_decodeToken.Decode(token, "username"));
                TblCart cart = await _cartRepo.GetCart(tblUser.Id);
                if (cart == null)
                {
                    result.Code = 201;
                    result.IsSuccess = true;
                    result.Data = "Xóa thành công";
                    return result;
                }
                List<TblCartDetail> cartDetail = await _cartRepo.GetListCartDetailByType(cart.Id, true);
                if (cartDetail == null || cartDetail.Count == 0)
                {
                    result.Code = 201;
                    result.IsSuccess = true;
                    result.Data = "Xóa thành công";
                    return result;
                }

                foreach (TblCartDetail i in cartDetail)
                {
                    _ = await _cartRepo.RemoveCartDetail(i);
                }



                result.Code = 201;
                result.IsSuccess = true;
                result.Message = "Xóa thành công";
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> CleanSaleCart(string token)
        {
            ResultModel result = new();
            try
            {
                TblUser tblUser = await _cartRepo.GetByUserName(_decodeToken.Decode(token, "username"));
                TblCart cart = await _cartRepo.GetCart(tblUser.Id);
                if (cart == null)
                {
                    result.Code = 201;
                    result.IsSuccess = true;
                    result.Data = "Xóa thành công";
                    return result;
                }
                List<TblCartDetail> cartDetail = await _cartRepo.GetListCartDetailByType(cart.Id, false);
                if (cartDetail == null || cartDetail.Count == 0)
                {
                    result.Code = 201;
                    result.IsSuccess = true;
                    result.Data = "Xóa thành công";
                    return result;
                }

                foreach (TblCartDetail i in cartDetail)
                {
                    _ = await _cartRepo.RemoveCartDetail(i);
                }



                result.Code = 201;
                result.IsSuccess = true;
                result.Message = "Xóa thành công";
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
            ResultModel result = new();
            try
            {
                CartShowModel modelResponse = new()
                {
                    rentItems = new List<ItemRequest>(),
                    saleItems = new List<ItemRequest>()
                };
                double? totalRentPriceCart = 0;
                double? totalSalePriceCart = 0;
                TblUser user = await _cartRepo.GetByUserName(_decodeToken.Decode(token, "username"));
                TblCart cart = await _cartRepo.GetCart(user.Id);
                if (cart == null)
                {
                    result.Code = 200;
                    result.IsSuccess = true;
                    result.Data = null;
                    return result;
                }
                List<TblCartDetail> listCartDetail = await _cartRepo.GetListCartDetail(cart.Id);
                if (listCartDetail.Count == 0)
                {
                    result.Code = 200;
                    result.IsSuccess = true;
                    result.Data = null;
                    return result;
                }
                foreach (TblCartDetail item in listCartDetail)
                {
                    if (item.IsForRent == true)
                    {
                        productItemDetail productItemDetail = await _cartRepo.GetProductItemDetail(item.SizeProductItemId);
                        TblProductItem productItem = await _cartRepo.GetProductItem(productItemDetail.ProductItemId);
                        productItem productItemRecord = new()
                        {
                            Content = productItem.Content,
                            Description = productItem.Description,
                            Id = productItem.Id,
                            Name = productItem.Name,
                            Type = productItem.Type
                        };
                        ItemRequest ItemRequest = new()
                        {
                            productItemDetail = productItemDetail,
                            quantity = item.Quantity,
                            unitPrice = productItemDetail.RentPrice,
                            productItem = productItemRecord
                        };
                        modelResponse.rentItems.Add(ItemRequest);
                        totalRentPriceCart += productItemDetail.RentPrice * item.Quantity;
                    }
                    if (item.IsForRent == false)
                    {
                        productItemDetail productItemDetail = await _cartRepo.GetProductItemDetail(item.SizeProductItemId);
                        TblProductItem productItem = await _cartRepo.GetProductItem(productItemDetail.ProductItemId);
                        productItem productItemRecord = new()
                        {
                            Content = productItem.Content,
                            Description = productItem.Description,
                            Id = productItem.Id,
                            Name = productItem.Name,
                            Type = productItem.Type
                        };
                        ItemRequest ItemRequest = new()
                        {
                            productItemDetail = productItemDetail,
                            quantity = item.Quantity,
                            unitPrice = productItemDetail.SalePrice,
                            productItem = productItemRecord
                        };
                        modelResponse.saleItems.Add(ItemRequest);
                        totalSalePriceCart += productItemDetail.SalePrice * item.Quantity;
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
