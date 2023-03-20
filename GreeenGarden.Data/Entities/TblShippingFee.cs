namespace GreeenGarden.Data.Entities;

public partial class TblShippingFee
{
    public Guid Id { get; set; }

    public int DistrictId { get; set; }

    public double FeeAmount { get; set; }
}
