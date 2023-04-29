using GreeenGarden.Data.Entities;
using GreeenGarden.Data.Models.ProductItemDetailModel;
using GreeenGarden.Data.Models.UserModels;

namespace GreeenGarden.Data.Models.FeedbackModel
{
    public class FeedbackByItemResModel
    {
        public Guid ProductItemDetailID { get; set; }
        public List<FeedbackResModel> ListFeedback { get; set; }

    }

        public class FeedbackResModel
    {
        public Guid ID { get; set; }
        public double Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string? Status { get; set; }
        public UserCurrResModel? User { get; set; }
        public List<string>? ImageURL { get; set; }
    }

    public class FeedbackOrderResModel
    {
        public Guid ID { get; set; }
        public double? Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string? Status { get; set; }
        public List<string>? ImageURL { get; set; }
    }
}
