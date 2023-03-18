using GreeenGarden.Business.Utilities.TokenService;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Enums;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Models.UserTreeModel;
using GreeenGarden.Data.Repositories.CartRepo;
using GreeenGarden.Data.Repositories.UserTreeRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace GreeenGarden.Business.Service.UserTreeService
{
    public class UserTreeService : IUserTreeService
    {
        private readonly DecodeToken _decodeToken;
        private readonly IUserTreeRepo _utRepo;

        public UserTreeService(IUserTreeRepo utRepo)
        {
            _utRepo = utRepo;
            _decodeToken = new DecodeToken();
        }

        public async Task<ResultModel> changeStatus(string token, UserTreeChangeStatusModel model)
        {
            var result = new ResultModel();
            try
            {
                var tblUser = await _utRepo.GetTblUserByUsername(_decodeToken.Decode(token, "username"));
                var detailUt = await _utRepo.GetDetailUserTreeByCustomer(model.UserTreeID);
                if (detailUt.User.Id != tblUser.Id)
                {
                    result.IsSuccess = false;
                    result.Data = "Cây này k thuộc sở hữu của user";
                    return result;
                }
                result.IsSuccess = await _utRepo.ChangeUserTreeByCustomer(model.UserTreeID, model.Status);


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

        public async Task<ResultModel> createUserTree(string token, UserTreeCreateModel model)
        {
            var result = new ResultModel();
            try
            {
                var tblUser = await _utRepo.GetTblUserByUsername(_decodeToken.Decode(token, "username"));
                var ut = new TblUserTree()
                {
                    Id = Guid.NewGuid(),
                    TreeName = model.TreeName,
                    UserId = tblUser.Id,
                    Description = model.Description,
                    Quantity = model.Quantity,
                    Status = Status.ACTIVE
                };
                await _utRepo.Insert(ut);

                foreach (var i in model.imgUrl)
                {
                    var img = new TblImage()
                    {
                        Id = Guid.NewGuid(),
                        UserTreeId = ut.Id,
                        ImageUrl = i
                    };
                    await _utRepo.InsertImg(img);
                }




                result.Code = 200;
                result.IsSuccess = true;
                result.Data = await _utRepo.GetDetailUserTreeByCustomer(ut.Id);
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> getDetailUserTree(string token, Guid userTreeID)
        {
            var result = new ResultModel();
            try
            {
                var tblUser = await _utRepo.GetTblUserByUsername(_decodeToken.Decode(token, "username"));
                var role = await _utRepo.GetTblRoleByUserID(tblUser.RoleId);

                var res = await _utRepo.GetDetailUserTreeByCustomer(userTreeID);
                if (role.RoleName != "Manager")
                {
                    if (tblUser.Id != res.User.Id)
                    {
                        result.IsSuccess = false;
                        result.Message = "Cây này k thuộc user";
                        return result;
                    }
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

        public async Task<ResultModel> getListUserTree(string token)
        {
            var result = new ResultModel();
            try
            {
                var tblUser = await _utRepo.GetTblUserByUsername(_decodeToken.Decode(token, "username"));

                var res = await _utRepo.GetListUserTreeByCustomer(tblUser);

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

        public async Task<ResultModel> updateUserTree(string token, UserTreeUpdateModel model)
        {
            var result = new ResultModel();
            try
            {
                var tblUser = await _utRepo.GetTblUserByUsername(_decodeToken.Decode(token, "username"));
                var detailUt = await _utRepo.GetDetailUserTreeByCustomer(model.UserTreeID);
                await _utRepo.UpdateUserTreeByCustomer(model);
                if (model.imgUrl.Count != 0)
                {
                    await _utRepo.updateImgUrlByUTID(model.UserTreeID, model.imgUrl);
                }

                if (detailUt.User.Id != tblUser.Id)
                {
                    result.IsSuccess = false;
                    result.Data = "Cây này k thuộc sở hữu của user";
                    return result;
                }
                result.IsSuccess = true;
                result.Data = await _utRepo.GetDetailUserTreeByCustomer(model.UserTreeID);
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
