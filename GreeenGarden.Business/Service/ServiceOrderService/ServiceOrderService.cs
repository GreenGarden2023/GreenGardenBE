using GreeenGarden.Business.Utilities.TokenService;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Enums;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Models.ServiceOrderModel;
using GreeenGarden.Data.Repositories.ServiceOrderRepo;
using GreeenGarden.Data.Repositories.ServiceRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace GreeenGarden.Business.Service.ServiceOrderService
{
    public class ServiceOrderService : IServiceOrderService
    {
        private readonly DecodeToken _decodeToken;
        private readonly IServiceOrderRepo _serOrRepo;
        private readonly IServiceRepo _serRepo;

        public ServiceOrderService(IServiceOrderRepo serOrRepo, IServiceRepo serRepo)
        {
            _serOrRepo = serOrRepo;
            _serRepo = serRepo;
            _decodeToken = new DecodeToken();
        }

        public async Task<ResultModel> changeStatus(string token, ServiceOrderChangeStatusModel model)
        {
            var result = new ResultModel();
            try
            {


                result.Code = 200;
                result.IsSuccess = true;
                result.Data = "";
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }//---

        public async Task<ResultModel> createServiceOrder(string token, ServiceOrderCreateModel model)
        {
            var result = new ResultModel();
            try
            {
                var service = await _serOrRepo.GetTblServiceByID(model.ServiceId);
                var tblUser = await _serOrRepo.getTblUserByID(service.UserId); 
                string userRole = _decodeToken.Decode(token, ClaimsIdentity.DefaultRoleClaimType);
                if (!userRole.Equals(Commons.MANAGER))
                {
                    return new ResultModel()
                    {
                        IsSuccess = false,
                        Message = "User not allowed"
                    };
                }
                DateTime ServiceStartDate = Utilities.Convert.ConvertUtil.convertStringToDateTime(model.ServiceStartDate);
                DateTime ServiceEndDate = Utilities.Convert.ConvertUtil.convertStringToDateTime(model.ServiceEndDate);
                double dateRange = (ServiceEndDate - ServiceStartDate).TotalDays;
                double Deposit = 0;
                double TotalPrice = 0;
                

                //Xử lý trước 
                for (int i = 0; i < model.UserTrees.Count; i++)// dồn cây
                {
                    for (int j = 1; j < model.UserTrees.Count; j++)
                    {
                        if (model.UserTrees[i].UserTreeID.Equals(model.UserTrees[j].UserTreeID) && i != j)
                        {
                            model.UserTrees[i].Quantity += model.UserTrees[j].Quantity;
                            model.UserTrees.Remove(model.UserTrees[j]);
                        }
                    }
                }

                //check điều kiện

                var checkService = await _serOrRepo.GetTblServiceByID(model.ServiceId);
                if (checkService == null)
                {
                    result.Code = 102;
                    result.IsSuccess = true;
                    result.Message = "Không tìm thấy service";
                    return result;
                }

                if (ServiceStartDate < DateTime.Now || ServiceEndDate < ServiceStartDate) // check ngày
                {
                    result.Code = 103;
                    result.IsSuccess = true;
                    result.Message = "Lỗi ngày";
                    return result;
                }

                foreach (var i in model.UserTrees)
                {
                    var utCheck = await _serOrRepo.getUserTreeByID(i.UserTreeID);
                    if (utCheck == null)
                    {
                        result.Code = 102;
                        result.IsSuccess = false;
                        return result;
                    }
                    if (i.Quantity > utCheck.Quantity)
                    {
                        result.Code = 101;
                        result.IsSuccess = false;
                        return result;
                    }
                    TotalPrice += i.Price * i.Quantity;
                }

                // tính các khoàn tiền
                TotalPrice = TotalPrice * dateRange;
                Deposit = TotalPrice / 2;
                TotalPrice += model.Incurred + model.TransportFee;

                // addTable
                #region Region: UpdateServiceUserTree


                var listSer = await _serOrRepo.GetListTblServiceUserTreeByServiceID(model.ServiceId);
                foreach (var i in listSer)
                {
                    await _serOrRepo.DeleteServiceUserTree(i);
                }
                foreach (var sut in model.UserTrees)
                {
                    var newSut = new TblServiceUserTree()
                    {
                        Id = Guid.NewGuid(),
                        Price= sut.Price,
                        Quantity= sut.Quantity,
                        ServiceId =model.ServiceId,
                        UserTreeId = sut.UserTreeID,
                    };
                    await _serRepo.insertServiceUserTree(newSut);
                }

                #endregion

                await _serRepo.ChangeStatusService(model.ServiceId, Status.ACCEPT);

                var newServiceOrder = new TblServiceOrder()
                {
                    Id = Guid.NewGuid(),
                    ServiceStartDate =ServiceStartDate,
                    ServiceEndDate = ServiceEndDate,
                    Deposit= Deposit,
                    TotalPrice= TotalPrice,
                    Status = Status.ACCEPT, //chỉnh sửa khi Hà check
                    RewardPointGain =0,
                    RewardPointUsed = 0,
                    TechnicianId = model.TechnicianId,
                    ServiceId = model.ServiceId,
                    Incurred= model.Incurred,
                    Description= model.Description,
                    TransportFee = model.TransportFee,
                    CreateDate = DateTime.Now,                   
                    UserId = tblUser.Id,
                };
                await _serOrRepo.Insert(newServiceOrder);

                result.Code = 200;
                result.IsSuccess = true;
                result.Data = "";
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> getDetailServiceOrder(string token, Guid SerOrderID)
        {
            var result = new ResultModel();
            try
            {
                var res = await _serOrRepo.GetDetailServiceOrder(SerOrderID);

                result.Code = 200;
                result.IsSuccess = true;
                result.Data = res;
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }//---

        public async Task<ResultModel> getListServiceOrderByCustomer(string token)
        {
            var result = new ResultModel();
            try
            {
                var tblUser = await _serOrRepo.getTblUserByUsername(_decodeToken.Decode(token, "username"));

                var res = await _serOrRepo.GetListServiceOrder(tblUser);
                

                result.Code = 200;
                result.IsSuccess = true;
                result.Data = res;
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> updateServiceOrder(string token, ServiceUserUpdateModel model)
        {
            var result = new ResultModel();
            try
            {
                string userRole = _decodeToken.Decode(token, ClaimsIdentity.DefaultRoleClaimType);
                if (!userRole.Equals(Commons.MANAGER))
                {
                    return new ResultModel()
                    {
                        IsSuccess = false,
                        Message = "User not allowed"
                    };
                }
                DateTime ServiceStartDate = Utilities.Convert.ConvertUtil.convertStringToDateTime(model.ServiceStartDate);
                DateTime ServiceEndDate = Utilities.Convert.ConvertUtil.convertStringToDateTime(model.ServiceEndDate);
                int dateRange = (ServiceEndDate - ServiceStartDate).Days;
                var serviceOrder = await _serOrRepo.GetTblServiceOrderByID(model.ServiceOrderId);
                double Deposit = 0;
                double TotalPrice = 0;


                //Xử lý trước 
                for (int i = 0; i < model.UserTrees.Count; i++)// dồn cây
                {
                    for (int j = 1; j < model.UserTrees.Count; j++)
                    {
                        if (model.UserTrees[i].UserTreeID.Equals(model.UserTrees[j].UserTreeID) && i != j)
                        {
                            model.UserTrees[i].Quantity += model.UserTrees[j].Quantity;
                            model.UserTrees.Remove(model.UserTrees[j]);
                        }
                    }
                }

                //check điều kiện
                if (serviceOrder.Status == Status.ACCEPT)
                {
                    result.Code = 105;
                    result.IsSuccess = true;
                    result.Message = "Trạng thái đã hoàn tất, không thể update";
                    return result;
                }

                var checkServiceOrder = await _serOrRepo.GetTblServiceByID((Guid)serviceOrder.ServiceId);
                if (checkServiceOrder == null)
                {
                    result.Code = 102;
                    result.IsSuccess = true;
                    result.Message = "Không tìm thấy service";
                    return result;
                }

                if (ServiceStartDate < DateTime.Now || ServiceEndDate < ServiceStartDate) // check ngày
                {
                    result.Code = 103;
                    result.IsSuccess = true;
                    result.Message = "Lỗi ngày";
                    return result;
                }

                foreach (var i in model.UserTrees)
                {
                    var utCheck = await _serOrRepo.getUserTreeByID(i.UserTreeID);
                    if (utCheck == null)
                    {
                        result.Code = 102;
                        result.IsSuccess = false;
                        return result;
                    }
                    if (i.Quantity > utCheck.Quantity)
                    {
                        result.Code = 101;
                        result.IsSuccess = false;
                        return result;
                    }
                    TotalPrice += i.Price * i.Quantity;
                }

                // tính các khoàn tiền
                TotalPrice = TotalPrice * dateRange;
                Deposit = TotalPrice / 2;
                TotalPrice += model.Incurred;

                // addTable
                #region Region: UpdateServiceUserTree
                var listSer = await _serOrRepo.GetListTblServiceUserTreeByServiceID((Guid)serviceOrder.ServiceId);
                foreach (var i in listSer)
                {
                    await _serOrRepo.DeleteServiceUserTree(i);
                }
                foreach (var sut in model.UserTrees)
                {
                    var newSut = new TblServiceUserTree()
                    {
                        Id = Guid.NewGuid(),
                        Price = sut.Price,
                        Quantity = sut.Quantity,
                        ServiceId = serviceOrder.ServiceId,
                        UserTreeId = sut.UserTreeID,
                    };
                    await _serRepo.insertServiceUserTree(newSut);
                }
                #endregion

                serviceOrder.TechnicianId = model.TechnicianId;
                serviceOrder.ServiceStartDate = ServiceStartDate;
                serviceOrder.ServiceEndDate = ServiceEndDate;
                serviceOrder.Incurred = model.Incurred;
                serviceOrder.Description = model.Description;
                await _serOrRepo.updateServiceOrder(serviceOrder);


                result.Code = 200;
                result.IsSuccess = true;
                result.Data = "";
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> getTechnician(string token)
        {
            var result = new ResultModel();
            try
            {
                string userRole = _decodeToken.Decode(token, ClaimsIdentity.DefaultRoleClaimType);
                if (!userRole.Equals(Commons.MANAGER))
                {
                    return new ResultModel()
                    {
                        IsSuccess = false,
                        Message = "User not allowed"
                    };
                }

                var res = await _serOrRepo.getListTecnician();

                result.Code = 200;
                result.IsSuccess = true;
                result.Data = res;
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> cleanServiceOrder(string token)
        {
            var result = new ResultModel();
            try
            {
                var tblUser = await _serOrRepo.getTblUserByUsername(_decodeToken.Decode(token, "username"));
                result.IsSuccess = await _serOrRepo.removeAll(tblUser.Id);
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> getListServiceOrderByManager(string token)
        {
            var result = new ResultModel();
            try
            {
                string userRole = _decodeToken.Decode(token, ClaimsIdentity.DefaultRoleClaimType);
                if (!userRole.Equals(Commons.MANAGER))
                {
                    return new ResultModel()
                    {
                        IsSuccess = false,
                        Message = "User not allowed"
                    };
                }
                var res = await _serOrRepo.GetListServiceOrderByManager();

                result.IsSuccess = true;
                result.Data = res;
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
