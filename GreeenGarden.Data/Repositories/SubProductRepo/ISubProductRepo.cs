using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Repositories.GenericRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreeenGarden.Data.Repositories.SubProductRepo
{
    public interface ISubProductRepo : IRepository<TblSubProduct>
    {
        public TblSubProduct queryDetailBySubId(Guid? SubId);
        public bool checkSizeUnique(Guid? SubId);
        public void updateSubProduct(TblSubProduct subProduct);
    }
}
