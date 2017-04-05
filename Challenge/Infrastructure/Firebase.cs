using System;
using FireSharp;
using FireSharp.Config;

namespace Challenge.Infrastructure
{
    public static class Firebase
    {
        private static FirebaseConfig BuildConfig()
        {
            const string Url = "https://testing-challenge.firebaseio.com";
            const string  Realm = "word-statistics";
            var dateKey = DateTime.Now.Date.ToString("yyyyMMdd");

            var config = new FirebaseConfig
            {
                BasePath = $"{Url}/{Realm}/{dateKey}"
            };
            return config;
        }

        public static FirebaseClient CreateClient()
        {
            return new FirebaseClient(BuildConfig());
        }
    }
}