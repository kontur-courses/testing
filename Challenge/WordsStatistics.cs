using System;
using System.Collections.Generic;
using System.Linq;

namespace Challenge
{
	public class WordsStatistics : IWordsStatistics
	{
		protected readonly IDictionary<string, int> stats 
			= new Dictionary<string, int>();

		public virtual void AddWord(string word)
		{
			if (word == null) throw new ArgumentNullException(nameof(word));
			if (string.IsNullOrWhiteSpace(word)) return;
			if (word.Length > 10)
				word = word.Substring(0, 10);
			int count;
			stats[word.ToLower()] = stats.TryGetValue(word.ToLower(), out count) ? count + 1 : 1;
		}

		/**
		<summary>
		Частотный словарь добавленных слов. 
		Слова сравниваются без учета регистра символов. 
		Порядок — по убыванию частоты слова.
		При одинаковой частоте — в лексикографическом порядке.
		</summary>
		*/

		public virtual IEnumerable<Tuple<int, string>> GetStatistics()
		{
			return stats.OrderByDescending(kv => kv.Value)
				.ThenBy(kv => kv.Key)
				.Select(kv => Tuple.Create(kv.Value, kv.Key));
		}
	}
}