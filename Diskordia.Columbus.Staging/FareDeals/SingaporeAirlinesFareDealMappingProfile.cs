using System;
using AutoMapper;
using Diskordia.Columbus.Contract.FareDeals;

namespace Diskordia.Columbus.Staging.FareDeals
{
	public class SingaporeAirlinesFareDealMappingProfile : Profile
	{
		public SingaporeAirlinesFareDealMappingProfile()
		{
			this.CreateMap<SingaporeAirlinesFareDeal, SingaporeAirlinesFareDealEntity>()
				.ForMember(dest => dest.CreatedOn, opt => opt.UseValue(DateTime.UtcNow));
		}
	}
}
