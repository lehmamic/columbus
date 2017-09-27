using System;
namespace Diskordia.Columbus.Common
{
	public interface ISerializer
	{
		string Serialize(object value);

		T Dezerializer<T>(string input);
	}
}
