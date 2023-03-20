using GreeenGarden.Data.Models.ServiceModel;

namespace GreeenGarden.Data.Models.ServiceOrderModel
{
    public class DetailServiceOrderResModel
    {
        public ServiceOrderResModel ServiceOrder { get; set; }
        public UserResModel User { get; set; }
    }
    public class ServiceUserTreeResModel
    {
        public Guid Id { get; set; }
        public Guid UserTreeID { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
    }

    public class ServiceResModel
    {
        public Guid Id { get; set; }
        public string Mail { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Status { get; set; }
        public string Name { get; set; }
        public List<ServiceUserTreeRespModel> ServiceUserTrees { get; set; }
    }

    public class ListServiceOrderResModel
    {
        public List<ServiceOrderResModel> ServiceOrder { get; set; }
        public UserResModel User { get; set; }

    }
    public class ServiceOrderResModel
    {
        public Guid Id { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? ServiceStartDate { get; set; }
        public DateTime? ServiceEndDate { get; set; }
        public double? Deposit { get; set; }
        public double? TotalPrice { get; set; }
        public string? Status { get; set; }
        public int? RewardPointGain { get; set; }
        public int? RewardPointUsed { get; set; }
        public UserResModel Technician { get; set; }
        public ServiceResModel Service { get; set; }
    }

    public class ServiceOrderResManagerModel
    {
        public Guid Id { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? ServiceStartDate { get; set; }
        public DateTime? ServiceEndDate { get; set; }
        public double? Deposit { get; set; }
        public double? TotalPrice { get; set; }
        public string? Status { get; set; }
        public double? Incurred { get; set; }
        public string? Description { get; set; }
        public int? RewardPointGain { get; set; }
        public int? RewardPointUsed { get; set; }
        public UserResModel Technician { get; set; }
        public ServiceResModel Service { get; set; }
        public UserResModel User { get; set; }
    }




}
