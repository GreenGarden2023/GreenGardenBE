using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.FeedbackModel;
using GreeenGarden.Data.Models.ProductItemModel;
using GreeenGarden.Data.Repositories.GenericRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreeenGarden.Data.Repositories.FeedbackRepo
{
    public interface IFeedbackRepo : IRepository<TblFeedBack>
    {
        Task<bool> ChangeStatus(FeedbackChangeStatusModel model);
    }
}
