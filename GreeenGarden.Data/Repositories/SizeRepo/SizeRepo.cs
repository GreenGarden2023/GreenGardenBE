using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.SizeModel;
using GreeenGarden.Data.Repositories.Repository;
using Microsoft.EntityFrameworkCore;

namespace GreeenGarden.Data.Repositories.SizeRepo
{
    public class SizeRepo : Repository<TblSize>, ISizeRepo
    {
        //private readonly IMapper _mapper;
        private readonly GreenGardenDbContext _context;
        public SizeRepo(/*IMapper mapper,*/ GreenGardenDbContext context) : base(context)
        {
            //_mapper = mapper;
            _context = context;
        }

        public async Task<bool> DeleteSizes(Guid sizeID)
        {
            try
            {
                TblSize? tblSize = await _context.TblSizes.Where(x => x.Id.Equals(sizeID)).FirstOrDefaultAsync();
                _ = _context.TblSizes.Remove(tblSize);
                _ = await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public async Task<List<TblSize>> GetProductItemSizes()
        {
            return await _context.TblSizes.ToListAsync();
        }

        public async Task<bool> UpdateSizes(SizeUpdateModel model)
        {
            try
            {
                TblSize? size = await _context.TblSizes.Where(x => x.Id == model.SizeID).FirstOrDefaultAsync();
                if (model.SizeName != null)
                {
                    size.Name = model.SizeName;
                }

                if (model.SizeType != null)
                {
                    size.Type = model.SizeType;
                }

                _ = _context.TblSizes.Update(size);
                _ = await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                return false;
            }
            return true;

        }
    }
}
