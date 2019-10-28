using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Challenge.Infrastructure
{
	[TestFixture]
	public class FirebaseActions
	{
		[Test]
		[Explicit]
		public void ClearLeaderboard()
		{
			using (var client = Firebase.CreateClient())
			{
				var response = client.Get("");
				var jObject = (JObject) JsonConvert.DeserializeObject(response.Body);
				foreach (var pair in jObject) client.Delete(pair.Key);
			}
		}
	}
}