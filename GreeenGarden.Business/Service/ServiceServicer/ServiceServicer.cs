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

        public async Task<ResultModel> createService(string token/*, ServiceCreateModel model*/)
        {
            var result = new ResultModel();
            try
            {
                /*var tblUser = await _serRepo.getTblUserByUsername(_decodeToken.Decode(token, "username"));
                DateTime StartDate = Utilities.Convert.ConvertUtil.convertStringToDateTime(model.StartDate);
                DateTime EndDate = Utilities.Convert.ConvertUtil.convertStringToDateTime(model.EndDate);


                var newService = new TblService()
                {
                    Id = Guid.NewGuid(),
                    StartDate = StartDate,
                    EndDate= EndDate,
                    Mail = model.Mail,
                    Phone = model.Phone,
                    Address= model.Address,
                    Status = Status.READY,
                    Name= model.Name,
                    UserId = tblUser.Id,
                };
                await _serRepo.Insert(newService);

                foreach (var ut in model.userTrees)
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
                }*/

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

        public async Task<ResultModel> updateService(string token, ServiceUpdateModel model)
        {
            var result = new ResultModel();
            try
            {
                var tblUser = await _serRepo.getTblUserByUsername(_decodeToken.Decode(token, "username"));
                DateTime StartDate = Utilities.Convert.ConvertUtil.convertStringToDateTime(model.service.StartDate);
                DateTime EndDate = Utilities.Convert.ConvertUtil.convertStringToDateTime(model.service.EndDate);

                // updateService

                var ser = await _serRepo.GetTblService(model.serviceID);
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
                    /*foreach (var sutModel in model.service.userTrees)
                    {
                        if (sut.UserTreeId == sutModel.UserTreeID)
                        {
                            var tblSerUt = await _serRepo.GetTblServiceUserTree(sut.Id);
                            tblSerUt.Quantity= sutModel.Quantity;
                            await _serRepo.UpdateServiceUserTree(tblSerUt);
                            serUtRemove.Remove(sut);
                        }
                    }*/
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
