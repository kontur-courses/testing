using System.Collections.Generic;

namespace Challenge
{
	public struct WordCount
	{
		public WordCount(string word, int count)
		{
			Word = word;
			Count = count;
		}

		public string Word { get; set; }
		public int Count { get; set; }

		public static WordCount Create(KeyValuePair<string, int> pair)
		{
			return new WordCount(pair.Key, pair.Value);
		}
	}
}