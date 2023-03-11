using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Repositories.GenericRepository;

namespace GreeenGarden.Data.Repositories.RequestRepo
{
    public interface IRequestRepo: IRepository<TblRequest>
    {
        Task<TblRequest> Create(TblRequest request);
    }
}
