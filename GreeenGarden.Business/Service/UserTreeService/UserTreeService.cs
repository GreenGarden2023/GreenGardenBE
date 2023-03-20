using System;
using GreeenGarden.Business.Utilities.TokenService;
using System.Security.Claims;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Enums;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Models.UserTreeModel;
using GreeenGarden.Data.Repositories.Repository;
using GreeenGarden.Data.Repositories.UserTreeRepo;
using GreeenGarden.Data.Repositories.ImageRepo;

namespace GreeenGarden.Business.Service.UserTreeService
{
	public class UserTreeService : IUserTreeService
    {
        private readonly DecodeToken _decodeToken;
        private readonly IUserTreeRepo _userTreeRepo;
        private readonly IImageRepo _imageRepo;
        public UserTreeService(IUserTreeRepo userTreeRepo, IImageRepo imageRepo)
        {
            _decodeToken = new DecodeToken();
            _userTreeRepo = userTreeRepo;
            _imageRepo = imageRepo;
        }

        public async Task<ResultModel> CreateUserTree(string token, UserTreeInsertModel userTreeInsertModel)
        {
            if (!string.IsNullOrEmpty(token))
            {
                string userRole = _decodeToken.Decode(token, ClaimsIdentity.DefaultRoleClaimType);
                if (!userRole.Equals(Commons.MANAGER)
                    && !userRole.Equals(Commons.STAFF)
                    && !userRole.Equals(Commons.ADMIN)
                    && !userRole.Equals(Commons.CUSTOMER))
                {
                    return new ResultModel()
                    {
                        IsSuccess = false,
                        Message = "User not allowed"
                    };
                }
            }
            else
            {
                return new ResultModel()
                {
                    IsSuccess = false,
                    Message = "User not allowed"
                };
            }
            ResultModel result = new ResultModel();
            try
            {
                string userID = _decodeToken.Decode(token, "userid");
                TblUserTree tblUserTree = new TblUserTree
                {
                    Id = Guid.NewGuid(),
                    UserId = Guid.Parse(userID),
                    Description = userTreeInsertModel.Description,
                    Quantity = userTreeInsertModel.Quantity,
                    TreeName = userTreeInsertModel.TreeName,
                    Status = TreeStatus.ACTIVE
                };
                Guid insert = await _userTreeRepo.Insert(tblUserTree);
                
                if(insert != Guid.Empty)
                {
                    foreach (string url in userTreeInsertModel.ImgUrls)
                    {
                        TblImage tblImage = new TblImage
                        {
                            UserTreeId = insert,
                            ImageUrl = url
                        };
                        _ = await _imageRepo.Insert(tblImage);
                    }

                    TblUserTree res = await _userTreeRepo.Get(insert);
                    List<string> imgs = await _imageRepo.GetImgUrlUserTree(insert);
                    UserTreeResModel userTreeResModel = new UserTreeResModel
                    {
                        Id = res.Id,
                        TreeName = res.TreeName,
                        UserId = (Guid)res.UserId,
                        Description = res.Description,
                        Quantity = (int)res.Quantity,
                        Status = res.Status,
                        ImgUrls = imgs
                    };
                    result.Code = 200;
                    result.IsSuccess = true;
                    result.Data = userTreeResModel;
                    result.Message = "Create user tree success.";
                    return result;
                }
                else
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.Message = "Create user tree failed.";
                    return result;
                }

            }
            catch (Exception e)
            {
                result.Code = 400;
                result.IsSuccess = false;
                result.Message = e.ToString();
                return result;
            }
        }
    }
}

