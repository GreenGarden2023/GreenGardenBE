namespace GreeenGarden.Data.Models.ServiceModel
{
    public class ServiceCRUDModel
    {
    }
    public class ServiceCreateModel
    {
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Mail { get; set; }
        public List<UserTreeAddModel> UserTrees { get; set; }
    }
    public class UserTreeAddModel
    {
        public Guid UserTreeID { get; set; }
        public int Quantity { get; set; }
    }
    public class ServiceUpdateModel
    {
        public Guid serviceID { get; set; }
        public ServiceCreateModel service { get; set; }
    }
}
