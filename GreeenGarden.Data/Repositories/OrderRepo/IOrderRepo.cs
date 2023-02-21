namespace GreeenGarden.Data.Repositories.OrderRepo
{
    public interface IOrderRepo
    {
        public Boolean checkWholesaleProduct(Guid subProductId, int quantity);
        // public Boolean checkRetailProduct(Guid productItemId);
    }
}
