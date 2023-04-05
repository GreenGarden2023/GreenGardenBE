using GreeenGarden.Data.Models.ProductItemDetailModel;
using GreeenGarden.Data.Models.UserModels;

namespace GreeenGarden.Data.Models.FeedbackModel
{
    public class FeedbackResModel
    {
        public Guid ID { get; set; }
        public double Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string? Status { get; set; }
        public UserCurrResModel? User { get; set; }
        public ProductItemDetailResModel? ProductItemDetail { get; set; }
        public List<string>? ImageURL { get; set; }
    }

}
