using EntityFrameworkPaginateCore;
using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.FeedbackModel;
using GreeenGarden.Data.Models.PaginationModel;
using GreeenGarden.Data.Repositories.GenericRepository;
using GreeenGarden.Data.Repositories.ImageRepo;
using Microsoft.EntityFrameworkCore;

namespace GreeenGarden.Data.Repositories.FeedbackRepo
{

    public class FeedbackRepo : Repository<TblFeedBack>, IFeedbackRepo
    {
        private readonly GreenGardenDbContext _context;
        private readonly IImageRepo _imageRepo;
        public FeedbackRepo(GreenGardenDbContext context, IImageRepo imageRepo) : base(context)
        {
            _context = context;
            _imageRepo = imageRepo;
        }

        public async Task<bool> ChangeStatus(FeedbackChangeStatusModel model)
        {
            TblFeedBack result = await _context.TblFeedBacks.Where(x => x.Id == model.FeedbackID).FirstOrDefaultAsync();
            result.Status = model.Status;
            _ = _context.TblFeedBacks.Update(result);
            _ = await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<TblFeedBack>> GetFeedBackByOrderID(Guid orderID)
        {
            var result = await _context.TblFeedBacks.Where(x => x.OrderId.Equals(orderID)).OrderBy(x => x.CreateDate).ToListAsync();
            foreach (var i in result)
            {
                var listImg = await _imageRepo.GetImgUrlFeedback(i.Id);
            }
            return result;
        }

        public async Task<List<TblFeedBack>> GetFeedBackByProductItemDetail(Guid productItemDetailID)
        {
            return await _context.TblFeedBacks.Where(x => x.ProductItemDetailId.Equals(productItemDetailID)).ToListAsync();
        }

        public async Task<List<FeedbackOrderResModel>> GetFeedBackOrderDetail(Guid orderID, Guid productItemDetailID)
        {
            List<FeedbackOrderResModel> resList = new();
            List<TblFeedBack> tblFeedBacks =  await _context.TblFeedBacks.Where(x => x.ProductItemDetailId.Equals(productItemDetailID) &&  x.OrderId.Equals(orderID)).ToListAsync();
           
            if(tblFeedBacks != null && tblFeedBacks.Count > 0)
            {
                foreach(var item in tblFeedBacks)
                {
                    List<string> fbImages = await _imageRepo.GetImgUrlFeedback(item.Id);
                    FeedbackOrderResModel model = new FeedbackOrderResModel
                    { 
                        ID = item.Id,
                        Rating = item.Rating,
                        Comment = item.Comment,
                        CreateDate = item.CreateDate,
                        UpdateDate = item.UpdateDate,
                        Status = item.Status,
                        ImageURL = fbImages
                    };
                    resList.Add(model);
                }
                return resList;
            }
            else { return null; }
        }

        public async Task<bool> UpdateFeedback(TblFeedBack entity)
        {
            _context.TblFeedBacks.Update(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
