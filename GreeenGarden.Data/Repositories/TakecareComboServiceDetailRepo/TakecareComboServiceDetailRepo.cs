using System;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.TakecareComboServiceModel;
using GreeenGarden.Data.Repositories.GenericRepository;
using GreeenGarden.Data.Repositories.TakecareComboRepo;
using Microsoft.EntityFrameworkCore;

namespace GreeenGarden.Data.Repositories.TakecareComboServiceDetailRepo
{
	public class TakecareComboServiceDetailRepo : Repository<TblTakecareComboServiceDetail>, ITakecareComboServiceDetailRepo
    {
        private readonly GreenGardenDbContext _context;
        public TakecareComboServiceDetailRepo(GreenGardenDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<TakecareComboServiceDetail> GetTakecareComboServiceDetail(Guid takecareComboServiceID)
        {
            try
            {
                TblTakecareComboServiceDetail tblTakecareComboServiceDetail = await _context.TblTakecareComboServiceDetails.Where(x => x.TakecareComboServiceId.Equals(takecareComboServiceID)).FirstOrDefaultAsync();
                if (tblTakecareComboServiceDetail != null)
                {
                    TakecareComboServiceDetail takecareComboServiceDetail = new()
                    {
                        TakecareComboDescription = tblTakecareComboServiceDetail.TakecareComboDescription,
                        TakecareComboGuarantee = tblTakecareComboServiceDetail.TakecareComboGuarantee,
                        TakecareComboName = tblTakecareComboServiceDetail.TakecareComboName,
                        TakecareComboPrice = tblTakecareComboServiceDetail.TakecareComboPrice,
                    };
                    return takecareComboServiceDetail;
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> UpdateTakecareComboServiceDetail(Guid takecareComboServiceID, Guid newTakecareComboID)
        {
            try
            {
                TblTakecareComboServiceDetail tblTakecareComboServiceDetail = await _context.TblTakecareComboServiceDetails.Where(x => x.TakecareComboServiceId.Equals(takecareComboServiceID)).FirstOrDefaultAsync();
                if (tblTakecareComboServiceDetail != null)
                {
                    TblTakecareCombo tblTakecareCombo = await _context.TblTakecareCombos.Where(x => x.Id.Equals(newTakecareComboID)).FirstOrDefaultAsync();
                    tblTakecareComboServiceDetail.TakecareComboName = tblTakecareCombo.Name;
                    tblTakecareComboServiceDetail.TakecareComboPrice = tblTakecareCombo.Price;
                    tblTakecareComboServiceDetail.TakecareComboGuarantee = tblTakecareCombo.Guarantee;
                    tblTakecareComboServiceDetail.TakecareComboDescription = tblTakecareCombo.Description;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}

