using System;
namespace GreeenGarden.Data.Models.OrderModel
{
	public class ServiceOrderCreateModel
	{
		public Guid ServiceId { get; set; }

		public int RewardPointUsed { get; set; }

		public double TransportFee { get; set; }

        public bool IsTransport { get; set; }
        
    }
}

