using GreeenGarden.Business.Utilities.TokenService;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Enums;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Models.ServiceModel;
using GreeenGarden.Data.Repositories.CartRepo;
using GreeenGarden.Data.Repositories.ServiceRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace GreeenGarden.Business.Service.ServiceServicer
{
    public class ServiceServicer : IServiceServicer
    {
        private readonly DecodeToken _decodeToken;
        private readonly IServiceRepo _serRepo;

        public ServiceServicer(IServiceRepo serRepo)
        {
            _serRepo = serRepo;
            _decodeToken = new DecodeToken();
        }

        public async Task<ResultModel> changeStatus(string token, Guid serviceID, string status)
        {
            var result = new ResultModel();
            try
            {
                await _serRepo.ChangeStatusService(serviceID, status);

                result.Code = 201;
                result.IsSuccess = true;
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> createService(string token, ServiceCreateModel model)
        {
            var result = new ResultModel();
            try
            {
                var tblUser = await _serRepo.getTblUserByUsername(_decodeToken.Decode(token, "username"));
                DateTime StartDate = Utilities.Convert.ConvertUtil.convertStringToDateTime(model.StartDate);
                DateTime EndDate = Utilities.Convert.ConvertUtil.convertStringToDateTime(model.EndDate);

                //-Check condidtion
                if (StartDate < DateTime.Now || EndDate < StartDate) // check ngày
                {
                    result.Code = 103;
                    result.IsSuccess = true;
                    result.Message = "Lỗi ngày";
                    return result;
                }
                //--Dồn số cây
                for (int i = 0; i < model.UserTrees.Count; i++)
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

                //--CheckQuantity
                foreach (var i in model.UserTrees)
                {
                    var utCheck = await _serRepo.getUserTreeByID(i.UserTreeID); 
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
                }
                //--Add table

                var newService = new TblService()
                {
                    Id = Guid.NewGuid(),
                    StartDate = StartDate,
                    EndDate = EndDate,
                    Mail = model.Mail,
                    Phone = model.Phone,
                    Address = model.Address,
                    Status = Status.PROCESSING,
                    Name = model.Name,
                    UserId = tblUser.Id,
                };
                await _serRepo.Insert(newService);

                foreach (var ut in model.UserTrees)
                {
                    var newServiceUt = new TblServiceUserTree()
                    {
                        Id = Guid.NewGuid(),
                        UserTreeId = ut.UserTreeID,
                        ServiceId = newService.Id,
                        Quantity = ut.Quantity,
                        Price = 0
                    };
                    await _serRepo.insertServiceUserTree(newServiceUt);
                }

                result.Code = 200;
                result.IsSuccess = true;
                result.Data = await _serRepo.GetDetailServiceByCustomer(newService.Id); ;
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> getDetailServiceByCustomer(string token, Guid serviceID)
        {
            var result = new ResultModel();
            try
            {
                var tblUser = await _serRepo.getTblUserByUsername(_decodeToken.Decode(token, "username"));
                var res = await _serRepo.GetDetailServiceByCustomer(serviceID);
                if (res.User.Id != tblUser.Id)
                {
                    result.IsSuccess = false;
                    result.Message = "Service này k phải của user";
                    return result;
                }

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

        public async Task<ResultModel> getListServiceByCustomer(string token)
        {
            var result = new ResultModel();
            try
            {
                var tblUser = await _serRepo.getTblUserByUsername(_decodeToken.Decode(token, "username"));
                var res = await _serRepo.GetListServiceByCustomer(tblUser);

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

        public async Task<ResultModel> getListServiceByManager(string token)
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
                var listSer = await _serRepo.GetListServiceByManager();

                result.Code = 200;
                result.IsSuccess = true;
                result.Data = listSer;
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> updateService(string token, ServiceUpdateModel model)
        {
            var result = new ResultModel();
            try
            {
                var tblUser = await _serRepo.getTblUserByUsername(_decodeToken.Decode(token, "username"));
                DateTime StartDate = Utilities.Convert.ConvertUtil.convertStringToDateTime(model.service.StartDate);
                DateTime EndDate = Utilities.Convert.ConvertUtil.convertStringToDateTime(model.service.EndDate);

                //check điều kiện
                for (int i = 0; i < model.service.UserTrees.Count; i++)
                {
                    for (int j = 1; j < model.service.UserTrees.Count; j++)
                    {
                        if (model.service.UserTrees[i].UserTreeID.Equals(model.service.UserTrees[j].UserTreeID) && i != j)
                        {
                            model.service.UserTrees[i].Quantity += model.service.UserTrees[j].Quantity;
                            model.service.UserTrees.Remove(model.service.UserTrees[j]);
                        }
                    }

                }

                //--CheckQuantity
                foreach (var i in model.service.UserTrees)
                {
                    var utCheck = await _serRepo.getUserTreeByID(i.UserTreeID);
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
                }

                // updateService

                var ser = await _serRepo.GetTblService(model.serviceID);
                if (ser.Status == Status.ACCEPT)
                {
                    result.Code = 105;
                    result.IsSuccess = false;
                    result.Message = "Trạng thái đã hoàn tất, không thể update";
                    return result;
                }
                ser.StartDate = StartDate; 
                ser.EndDate = EndDate;
                ser.Name = model.service.Name;
                ser.Phone = model.service.Phone;
                ser.Address = model.service.Address;
                ser.Mail = model.service.Mail;
                await _serRepo.UpdateService(ser);

                var listSerUt = await _serRepo.GetListTblServiceUserTree(model.serviceID);
                var serUtRemove = await _serRepo.GetListTblServiceUserTree(model.serviceID);
                foreach (var sut in listSerUt)
                {
                    foreach (var sutModel in model.service.UserTrees)
                    {
                        if (sut.UserTreeId == sutModel.UserTreeID)
                        {
                            var tblSerUt = await _serRepo.GetTblServiceUserTree(sut.Id);
                            tblSerUt.Quantity = sutModel.Quantity;
                            await _serRepo.UpdateServiceUserTree(tblSerUt);
                            serUtRemove.Remove(sut);
                        }
                    }
                }
                foreach (var sr in serUtRemove)
                {
                    await _serRepo.DeleteServiceUserTree(sr);
                }


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
    }
}
