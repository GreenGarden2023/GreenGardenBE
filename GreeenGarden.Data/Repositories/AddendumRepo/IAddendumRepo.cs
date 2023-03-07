using System;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Repositories.GenericRepository;

namespace GreeenGarden.Data.Repositories.AddendumRepo
{
	public interface IAddendumRepo : IRepository<TblAddendum>
    {
		public Task<ResultModel> UpdateRentAddendumPayment(Guid addendumId, double payAmount);
        public Task<ResultModel> UpdateSaleAddendumPayment(Guid addendumId, double payAmount);
        public Task<ResultModel> UpdateDepositAddendumPayment(Guid addendumId, double payAmount);
    }
}

