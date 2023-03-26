using GreeenGarden.Business.Utilities.TokenService;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Enums;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Models.UserTreeModel;
using GreeenGarden.Data.Repositories.ImageRepo;
using GreeenGarden.Data.Repositories.UserTreeRepo;
using System.Security.Claims;

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
                        Code = 403,
                        Message = "User not allowed"
                    };
                }
            }
            else
            {
                return new ResultModel()
                {
                    IsSuccess = false,
                    Code = 403,
                    Message = "User not allowed"
                };
            }
            ResultModel result = new();
            try
            {
                string userID = _decodeToken.Decode(token, "userid");
                TblUserTree tblUserTree = new()
                {
                    Id = Guid.NewGuid(),
                    UserId = Guid.Parse(userID),
                    Description = userTreeInsertModel.Description,
                    Quantity = userTreeInsertModel.Quantity,
                    TreeName = userTreeInsertModel.TreeName,
                    Status = TreeStatus.ACTIVE
                };
                Guid insert = await _userTreeRepo.Insert(tblUserTree);

                if (insert != Guid.Empty)
                {
                    foreach (string url in userTreeInsertModel.ImgUrls)
                    {
                        TblImage tblImage = new()
                        {
                            UserTreeId = insert,
                            ImageUrl = url
                        };
                        _ = await _imageRepo.Insert(tblImage);
                    }

                    TblUserTree res = await _userTreeRepo.Get(insert);
                    List<string> imgs = await _imageRepo.GetImgUrlUserTree(insert);
                    UserTreeResModel userTreeResModel = new()
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

        public async Task<ResultModel> GetUserTrees(string token)
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
                        Code = 403,
                        Message = "User not allowed"
                    };
                }
            }
            else
            {
                return new ResultModel()
                {
                    IsSuccess = false,
                    Code = 403,
                    Message = "User not allowed"
                };
            }
            ResultModel result = new();
            try
            {
                string userID = _decodeToken.Decode(token, "userid");
                List<TblUserTree> tblUserTree = await _userTreeRepo.GetUserTrees(Guid.Parse(userID));
                if (tblUserTree != null)
                {
                    List<UserTreeResModel> resList = new();
                    foreach (TblUserTree tree in tblUserTree)
                    {
                        List<string> imgs = await _imageRepo.GetImgUrlUserTree(tree.Id);
                        UserTreeResModel userTreeResModel = new()
                        {
                            Id = tree.Id,
                            TreeName = tree.TreeName,
                            UserId = (Guid)tree.UserId,
                            Description = tree.Description,
                            Quantity = (int)tree.Quantity,
                            Status = tree.Status,
                            ImgUrls = imgs
                        };
                        resList.Add(userTreeResModel);
                    }
                    result.Code = 200;
                    result.IsSuccess = true;
                    result.Data = resList;
                    result.Message = "Get user tree success.";
                    return result;
                }
                else
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.Message = "Get user tree failed.";
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

        public async Task<ResultModel> UpdateUserTree(string token, UserTreeUpdateModel userTreeUpdateModel)
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
                        Code = 403,
                        Message = "User not allowed"
                    };
                }
            }
            else
            {
                return new ResultModel()
                {
                    IsSuccess = false,
                    Code = 403,
                    Message = "User not allowed"
                };
            }
            ResultModel result = new();
            try
            {
                TblUserTree tblUserTree = await _userTreeRepo.Get(userTreeUpdateModel.Id);
                if (tblUserTree != null)
                {
                    bool update = await _userTreeRepo.UpdateUserTree(userTreeUpdateModel);
                    if (userTreeUpdateModel.ImgUrls.Any())
                    {
                        _ = await _imageRepo.UpdateImgForUserTree(userTreeUpdateModel.Id, userTreeUpdateModel.ImgUrls);
                    }
                    if (update == true)
                    {
                        TblUserTree tblUserTreeRes = await _userTreeRepo.Get(userTreeUpdateModel.Id);
                        List<string> imgs = await _imageRepo.GetImgUrlUserTree(tblUserTreeRes.Id);
                        UserTreeResModel userTreeResModel = new()
                        {
                            Id = tblUserTreeRes.Id,
                            TreeName = tblUserTreeRes.TreeName,
                            UserId = (Guid)tblUserTreeRes.UserId,
                            Description = tblUserTreeRes.Description,
                            Quantity = (int)tblUserTreeRes.Quantity,
                            Status = tblUserTreeRes.Status,
                            ImgUrls = imgs
                        };
                        result.Code = 200;
                        result.IsSuccess = true;
                        result.Data = userTreeResModel;
                        result.Message = "Update user tree success.";
                        return result;
                    }
                    else
                    {
                        result.Code = 400;
                        result.IsSuccess = false;
                        result.Message = "Update user tree failed.";
                        return result;
                    }
                }
                else
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.Message = "User tree ID invalid.";
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

        public async Task<ResultModel> UpdateUserTreeStatus(string token, UserTreeStatusModel userTreeStatusModel)
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
                        Code = 403,
                        Message = "User not allowed"
                    };
                }
            }
            else
            {
                return new ResultModel()
                {
                    IsSuccess = false,
                    Code = 403,
                    Message = "User not allowed"
                };
            }
            ResultModel result = new();
            try
            {
                TblUserTree tblUserTree = await _userTreeRepo.Get(userTreeStatusModel.Id);
                if (tblUserTree != null)
                {
                    bool update = await _userTreeRepo.ChangeUserTreeStatus(userTreeStatusModel);
                    if (update == true)
                    {
                        TblUserTree tblUserTreeRes = await _userTreeRepo.Get(userTreeStatusModel.Id);
                        List<string> imgs = await _imageRepo.GetImgUrlUserTree(tblUserTreeRes.Id);
                        UserTreeResModel userTreeResModel = new()
                        {
                            Id = tblUserTreeRes.Id,
                            TreeName = tblUserTreeRes.TreeName,
                            UserId = (Guid)tblUserTreeRes.UserId,
                            Description = tblUserTreeRes.Description,
                            Quantity = (int)tblUserTreeRes.Quantity,
                            Status = tblUserTreeRes.Status,
                            ImgUrls = imgs
                        };
                        result.Code = 200;
                        result.IsSuccess = true;
                        result.Data = userTreeResModel;
                        result.Message = "Update user tree status success.";
                        return result;
                    }
                    else
                    {
                        result.Code = 400;
                        result.IsSuccess = false;
                        result.Message = "Update user tree status failed.";
                        return result;
                    }
                }
                else
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.Message = "User tree ID invalid.";
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

