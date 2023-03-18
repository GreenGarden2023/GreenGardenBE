using AutoMapper;
using GreeenGarden.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GreeenGarden.Data.Models.ServiceOrderModel;
using GreeenGarden.Data.Models.ServiceModel;

namespace GreeenGarden.Data.Utilities
{
    public class MappingProfile : Profile
    {
        public MappingProfile() {
            #region Nullable type
            CreateMap<int?, int>().ConvertUsing((src, des) => src ?? des);
            CreateMap<bool?, bool>().ConvertUsing((src, des) => src ?? des);
            CreateMap<Guid?, Guid>().ConvertUsing((src, des) => src ?? des);
            CreateMap<DateTime?, DateTime>().ConvertUsing((src, des) => src ?? des);
            #endregion

            CreateMap<TblServiceOrder, DetailServiceOrderResModel>();

            CreateMap<TblServiceOrder, ServiceOrderResModel>();
            CreateMap<TblServiceOrder, ServiceOrderResManagerModel>();

            CreateMap<TblUser, UserResModel>();

            CreateMap<TblServiceUserTree, ServiceUserTreeResModel>(); 
            CreateMap<TblServiceUserTree, ServiceUserTreeRespModel>(); 


            CreateMap<TblService, ServiceResModel>();
            CreateMap<TblService, ServiceByManagerResModel>();

            CreateMap<TblUserTree, UserTreeResModel>();
            
        }
    }
}
