namespace GreeenGarden.Data.Models.FeedbackModel
{
    public class FeedbackUpdateModel
    {
    }
    public class FeedbackChangeStatusModel
    {
        public Guid FeedbackID { get; set; }
        public string? Status { get; set; }
    }
}
