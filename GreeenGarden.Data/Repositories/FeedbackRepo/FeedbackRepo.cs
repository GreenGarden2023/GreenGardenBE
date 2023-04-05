using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.FeedbackModel;
using GreeenGarden.Data.Repositories.GenericRepository;
using Microsoft.EntityFrameworkCore;

namespace GreeenGarden.Data.Repositories.FeedbackRepo
{
    public class FeedbackRepo : Repository<TblFeedBack>, IFeedbackRepo
    {
        private readonly GreenGardenDbContext _context;
        public FeedbackRepo(GreenGardenDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<bool> ChangeStatus(FeedbackChangeStatusModel model)
        {
            TblFeedBack result = await _context.TblFeedBacks.Where(x => x.Id == model.FeedbackID).FirstOrDefaultAsync();
            result.Status = model.Status;
            _ = _context.TblFeedBacks.Update(result);
            _ = await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<TblFeedBack>> GetFeedBackByProductItemDetail(Guid productItemDetailID)
        {
            return await _context.TblFeedBacks.Where(x => x.ProductItemDetailId.Equals(productItemDetailID)).ToListAsync();
        }
    }
}
