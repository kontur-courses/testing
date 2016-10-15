using FireSharp;
using FireSharp.Config;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Challenge.Infrastructure
{
    [TestFixture]
    public class FirebaseActions
    {
        [Test, Explicit]
        public void ClearLeaderboard()
        {
            var config = new FirebaseConfig
            {
                BasePath = "https://testing-challenge.firebaseio.com/word-statistics/"
            };
            using (var client = new FirebaseClient(config))
            {
                var response = client.Get("");
                var jObject = JsonConvert.DeserializeObject(response.Body) as JObject;
                foreach (var pair in jObject)
                {
                    client.Delete(pair.Key);
                }
            }
        }
    }
}