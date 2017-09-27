using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Diskordia.Columbus.Common
{
	public class ObjectSerializer : ISerializer
	{
		private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
		{
			ContractResolver = new CamelCasePropertyNamesContractResolver()
		};

		public T Dezerializer<T>(string input)
		{
			return (T)JsonConvert.DeserializeObject(input, Settings);
		}

		public string Serialize(object value)
		{
			return JsonConvert.SerializeObject(value, Settings);
		}
	}
}
