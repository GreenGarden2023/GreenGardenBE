using System;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.ResultModel;
using GreeenGarden.Data.Models.TakecareComboModel;
using GreeenGarden.Data.Repositories.GenericRepository;
using GreeenGarden.Data.Repositories.SizeRepo;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace GreeenGarden.Data.Repositories.TakecareComboRepo
{
	public class TakecareComboRepo : Repository<TblTakecareCombo>, ITakecareComboRepo
    {
        private readonly GreenGardenDbContext _context;
        public TakecareComboRepo(GreenGardenDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<TblTakecareCombo>> GetAllTakecareCombo(string status)
        {
            if (status.Trim().ToLower().Equals("active"))
            {
                List<TblTakecareCombo> tblTakecareCombos = await _context.TblTakecareCombos.OrderByDescending(x => x.Name).Where(y => y.Status == true ).ToListAsync();
                return tblTakecareCombos;
            }
            else if (status.Trim().ToLower().Equals("disabled"))
            {
                List<TblTakecareCombo> tblTakecareCombos = await _context.TblTakecareCombos.OrderByDescending(x => x.Name).Where(y => y.Status == false).ToListAsync();
                return tblTakecareCombos;
            }
            else
            {
                List<TblTakecareCombo> tblTakecareCombos = await _context.TblTakecareCombos.OrderByDescending(x => x.Name).ToListAsync();
                return tblTakecareCombos;
            }
        }

        public async Task<ResultModel> UpdateTakecareCombo(TakecareComboUpdateModel takecareComboUpdateModel)
        {
            ResultModel result = new();

            try
            {
                TblTakecareCombo tblTakecareCombo = await _context.TblTakecareCombos.Where(x => x.Id.Equals(takecareComboUpdateModel.Id)).FirstOrDefaultAsync();
                if (!String.IsNullOrEmpty(takecareComboUpdateModel.Name) && !takecareComboUpdateModel.Name.Equals(tblTakecareCombo.Name))
                {
                    tblTakecareCombo.Name = takecareComboUpdateModel.Name;
                }
                if (!String.IsNullOrEmpty(takecareComboUpdateModel.Description) && !takecareComboUpdateModel.Description.Equals(tblTakecareCombo.Description))
                {
                    tblTakecareCombo.Description = takecareComboUpdateModel.Description;
                }
                if (!String.IsNullOrEmpty(takecareComboUpdateModel.Guarantee) && !takecareComboUpdateModel.Guarantee.Equals(tblTakecareCombo.Guarantee))
                {
                    tblTakecareCombo.Guarantee = takecareComboUpdateModel.Guarantee;
                }
                if (takecareComboUpdateModel.Price != null && takecareComboUpdateModel.Price != tblTakecareCombo.Price)
                {
                    tblTakecareCombo.Price = (double)takecareComboUpdateModel.Price;
                }
                if (takecareComboUpdateModel.Status != null && !takecareComboUpdateModel.Status.Equals(tblTakecareCombo.Status))
                {
                    tblTakecareCombo.Status = takecareComboUpdateModel.Status;
                }
                if (takecareComboUpdateModel.CareGuide != null && !takecareComboUpdateModel.CareGuide.Equals(tblTakecareCombo.Careguide))
                {
                    tblTakecareCombo.Careguide = takecareComboUpdateModel.CareGuide;
                }
                _ = _context.TblTakecareCombos.Update(tblTakecareCombo);
                _ = await _context.SaveChangesAsync();
                result.IsSuccess = true;
                result.Code = 200;
                result.Message = "Update success";
                return result;

            }
            catch(Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.Data = e.ToString();
                result.Message = "Update failed.";
                return result;
            }
        } 
    }
}

