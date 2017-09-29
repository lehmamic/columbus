using System;

namespace Diskordia.Columbus.Contract.FareDeals
{
	public class FareDeal
	{
		public Uri Link { get; set; }

		public string DepartureAirport { get; set; }

		public string DestinationAirport { get; set; }

		public string Class { get; set; }

		public string Currency { get; set; }

		public decimal Price { get; set; }

		public DateTime BookBy { get; set; }

		public DateTime OutboundStartDate { get; set; }

		public DateTime OutboundEndDate { get; set; }

		public DateTime TravelCompleteDate { get; set; }
	}
}
