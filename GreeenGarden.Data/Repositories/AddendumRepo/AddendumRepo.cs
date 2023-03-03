﻿using System;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Enums;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Repositories.GenericRepository;
using Microsoft.EntityFrameworkCore;

namespace GreeenGarden.Data.Repositories.AddendumRepo
{
	public class AddendumRepo : Repository<TblAddendum>, IAddendumRepo
    {
        private readonly GreenGardenDbContext _context;
        public AddendumRepo(GreenGardenDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<ResultModel> UpdateAddendumPayment(Guid addendumId, double payAmount)
        {
            ResultModel result = new ResultModel();
            try
            {
                var addendum = await _context.TblAddendums.Where(x => x.Id.Equals(addendumId)).FirstOrDefaultAsync();
                if(addendum != null)
                {
                    addendum.RemainMoney = addendum.RemainMoney - payAmount;
                    if(addendum.RemainMoney < 0)
                    {
                        result.Code = 400;
                        result.IsSuccess = false;
                        result.Message = "The pay amount exceed the remain amount.";
                        return result;
                    }else if (addendum.RemainMoney == 0)
                    {
                        addendum.Status = Status.COMPLETED;
                        _context.TblAddendums.Update(addendum);
                        await _context.SaveChangesAsync();
                        result.Code = 200;
                        result.IsSuccess = true;
                        result.Message = "Addendum complete.";
                        return result;
                    }
                    else
                    {
                        _context.TblAddendums.Update(addendum);
                        await _context.SaveChangesAsync();
                        result.Code = 200;
                        result.IsSuccess = true;
                        result.Message = "Update successful.";
                        return result;
                    }
                }
                else
                {
                    result.Code = 400;
                    result.IsSuccess = false;
                    result.Message = "Update addendum payment failed.";
                    return result;
                }

            }catch(Exception e)
            {
                result.Code = 400;
                result.IsSuccess = false;
                result.Message = "Update addendum payment failed.";
                return result;
            }
        }
    }
}
