﻿using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Repositories.GenericRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreeenGarden.Data.Repositories.ServiceOrderRepo
{
    public interface IServiceOrderRepo : IRepository<TblServiceOrder>
    {
        //Task<TblRequest> getRequestByID(Guid id);
    }
}
