namespace GreeenGarden.Data.Models.FeedbackModel
{
    public class FeedbackUpdateModel
    {
        public Guid FeedbackID { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public List<string>? ImagesUrls { get; set; }

    }
    public class FeedbackChangeStatusModel
    {
        public Guid FeedbackID { get; set; }
        public string? Status { get; set; }
    }
}
