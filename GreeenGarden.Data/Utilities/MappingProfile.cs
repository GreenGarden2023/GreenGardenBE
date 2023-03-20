using AutoMapper;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.ServiceModel;
using GreeenGarden.Data.Models.ServiceOrderModel;

namespace GreeenGarden.Data.Utilities
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            #region Nullable type
            CreateMap<int?, int>().ConvertUsing((src, des) => src ?? des);
            CreateMap<bool?, bool>().ConvertUsing((src, des) => src ?? des);
            CreateMap<Guid?, Guid>().ConvertUsing((src, des) => src ?? des);
            CreateMap<DateTime?, DateTime>().ConvertUsing((src, des) => src ?? des);
            #endregion

            _ = CreateMap<TblServiceOrder, DetailServiceOrderResModel>();

            _ = CreateMap<TblServiceOrder, ServiceOrderResModel>();
            _ = CreateMap<TblServiceOrder, ServiceOrderResManagerModel>();

            _ = CreateMap<TblUser, UserResModel>();

            _ = CreateMap<TblServiceUserTree, ServiceUserTreeResModel>();
            _ = CreateMap<TblServiceUserTree, ServiceUserTreeRespModel>();


            _ = CreateMap<TblService, ServiceResModel>();
            _ = CreateMap<TblService, ServiceByManagerResModel>();

            _ = CreateMap<TblUserTree, UserTreeResModel>();

        }
    }
}
