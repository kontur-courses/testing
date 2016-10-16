using System;
using System.Collections.Generic;
using System.Linq;
using Challenge.Infrastructure;

namespace Challenge.IncorrectImplementations
{
	#region Не подглядывать!

    [IncorrectImplementation]
	public class WordsStatisticsL : WordsStatistics
	{
		public override void AddWord(string word)
		{
			if (word == null) throw new ArgumentNullException(nameof(word));
			if (string.IsNullOrWhiteSpace(word)) return;
			word = word.Substring(0, 10);
			int count;
			stats[word.ToLower()] = stats.TryGetValue(word.ToLower(), out count) ? count + 1 : 1;
		}
	}

    [IncorrectImplementation]
    public class WordsStatisticsL2 : WordsStatistics
	{
		public override void AddWord(string word)
		{
			if (word == null) throw new ArgumentNullException(nameof(word));
			if (string.IsNullOrWhiteSpace(word)) return;
			int count;
			stats[word.ToLower()] = stats.TryGetValue(word.ToLower(), out count) ? count + 1 : 1;
		}
	}

    [IncorrectImplementation]
    public class WordsStatisticsL3 : WordsStatistics
	{
		public override void AddWord(string word)
		{
			if (word == null) throw new ArgumentNullException(nameof(word));
			if (string.IsNullOrWhiteSpace(word)) return;
			if (word.Length > 10) word = word.Substring(0, 10);
			else if (word.Length > 5) word = word.Substring(0, word.Length - 1);
			int count;
			stats[word.ToLower()] = stats.TryGetValue(word.ToLower(), out count) ? count + 1 : 1;

		}
	}

    [IncorrectImplementation]
    public class WordsStatisticsL4 : WordsStatistics
	{
		public override void AddWord(string word)
		{
			if (word == null) throw new ArgumentNullException(nameof(word));
			if (string.IsNullOrWhiteSpace(word)) return;
			if (word.Length - 1 > 10) word = word.Substring(0, 10);
			int count;
			stats[word.ToLower()] = stats.TryGetValue(word.ToLower(), out count) ? count + 1 : 1;
		}
	}

    [IncorrectImplementation]
    public class WordsStatisticsC : WordsStatistics
	{
		public override void AddWord(string word)
		{
			if (word == null) throw new ArgumentNullException(nameof(word));
			if (string.IsNullOrWhiteSpace(word)) return;
			if (word.Length > 10) word = word.Substring(0, 10);
			if (!stats.ContainsKey(word.ToLower()))
				stats[word] = 0;
			stats[word]++;
		}
	}

    [IncorrectImplementation]
    public class WordsStatisticsE : WordsStatistics
	{
		public override void AddWord(string word)
		{
			if (string.IsNullOrWhiteSpace(word)) throw new ArgumentNullException(nameof(word));
			if (word.Length > 10) word = word.Substring(0, 10);
			int count;
			stats[word.ToLower()] = stats.TryGetValue(word.ToLower(), out count) ? count + 1 : 1;
		}
	}

    [IncorrectImplementation]
    public class WordsStatisticsE2 : WordsStatistics
	{
		public override void AddWord(string word)
		{
			if (string.IsNullOrWhiteSpace(word)) return;
			if (word.Length > 10) word = word.Substring(0, 10);
			int count;
			stats[word.ToLower()] = stats.TryGetValue(word.ToLower(), out count) ? count + 1 : 1;
		}
	}

    [IncorrectImplementation]
    public class WordsStatisticsE3 : WordsStatistics
	{
		public override void AddWord(string word)
		{
			if (word == null) throw new ArgumentNullException(nameof(word));
			if (word.Length > 10) word = word.Substring(0, 10);
			if (string.IsNullOrWhiteSpace(word)) return;
			int count;
			stats[word.ToLower()] = stats.TryGetValue(word.ToLower(), out count) ? count + 1 : 1;
		}
	}

    [IncorrectImplementation]
    public class WordsStatisticsE4 : WordsStatistics
	{
		public override void AddWord(string word)
		{
			if (word.Length == 0 || word.All(char.IsWhiteSpace)) return;
			if (word.Length > 10) word = word.Substring(0, 10);
			int count;
			stats[word.ToLower()] = stats.TryGetValue(word.ToLower(), out count) ? count + 1 : 1;
		}
	}

    [IncorrectImplementation]
    public class WordsStatisticsO1 : WordsStatistics
	{
		public override IEnumerable<Tuple<int, string>> GetStatistics()
		{
			return stats.OrderBy(kv => kv.Key)
				.Select(kv => Tuple.Create(kv.Value, kv.Key));
		}
	}

    [IncorrectImplementation]
    public class WordsStatisticsO2 : WordsStatistics
	{
		public override IEnumerable<Tuple<int, string>> GetStatistics()
		{
			return stats.OrderByDescending(kv => kv.Value)
				.Select(kv => Tuple.Create(kv.Value, kv.Key));
		}
	}

    [IncorrectImplementation]
    public class WordsStatisticsO3 : WordsStatistics
	{
		public override IEnumerable<Tuple<int, string>> GetStatistics()
		{
			return stats.OrderBy(kv => kv.Value)
				.Select(kv => Tuple.Create(kv.Value, kv.Key));
		}
	}

    [IncorrectImplementation]
    public class WordsStatisticsO4 : WordsStatistics
	{
		public override IEnumerable<Tuple<int, string>> GetStatistics()
		{
			return base.GetStatistics().OrderBy(t => t.Item2);
		}
	}

    [IncorrectImplementation]
    public class WordsStatisticsO5 : WordsStatistics
	{
		public override IEnumerable<Tuple<int, string>> GetStatistics()
		{
			return base.GetStatistics().OrderByDescending(t => t);
		}
	}

    [IncorrectImplementation]
    public class WordsStatisticsCR : IWordsStatistics
	{
		private readonly IDictionary<string, int> stats =
			new Dictionary<string, int>();

		public void AddWord(string word)
		{
			if (word == null) throw new ArgumentNullException(nameof(word));
			if (string.IsNullOrEmpty(word)) return;
			if (word.Length > 10) word = word.Substring(0, 10);
			word = word.ToLower();
			int count;
			stats[word] = stats.TryGetValue(word, out count)
				? count + 1 : 1;
		}

		public IEnumerable<Tuple<int, string>> GetStatistics()
		{
			return stats.OrderByDescending(kv => kv.Value)
				.ThenBy(kv => kv.Key)
				.Select(kv => Tuple.Create(kv.Value, kv.Key));
		}
	}

    [IncorrectImplementation]
    public class WordsStatistics_STA : IWordsStatistics
	{
		private static readonly IDictionary<string, int> stats = new Dictionary<string, int>();

		public WordsStatistics_STA()
		{
			stats.Clear();
		}

		public void AddWord(string word)
		{
			if (word == null) throw new ArgumentNullException(nameof(word));
			if (string.IsNullOrWhiteSpace(word)) return;
			if (word.Length > 10) word = word.Substring(0, 10);
			int count;
			stats[word.ToLower()] = stats.TryGetValue(word.ToLower(), out count) ? count + 1 : 1;
		}

		public IEnumerable<Tuple<int, string>> GetStatistics()
		{
			return stats.OrderByDescending(kv => kv.Value).ThenBy(kv => kv.Key).Select(kv => Tuple.Create(kv.Value, kv.Key));
		}
	}

    [IncorrectImplementation]
    public class WordsStatistics_123 : IWordsStatistics
	{
		private const int MAX_SIZE = 12347;

		private readonly int[] stats = new int[MAX_SIZE];
		private readonly string[] words = new string[MAX_SIZE];

		public void AddWord(string word)
		{
			if (word == null) throw new ArgumentNullException(nameof(word));
			if (string.IsNullOrWhiteSpace(word)) return;
			if (word.Length > 10) word = word.Substring(0, 10);
			var index = Math.Abs(word.ToLower().GetHashCode()) % MAX_SIZE;
			stats[index]++;
			words[index] = word.ToLower();
		}

		public IEnumerable<Tuple<int, string>> GetStatistics()
		{
			return stats.Zip(words, Tuple.Create)
				.Where(t => t.Item1 > 0)
				.OrderByDescending(t => t.Item1)
				.ThenBy(t => t.Item2);
		}
	}

    [IncorrectImplementation]
    public class WordsStatistics_QWE : IWordsStatistics
	{
		private readonly IDictionary<string, int> stats = new Dictionary<string, int>();

		public void AddWord(string word)
		{
			if (word == null) throw new ArgumentNullException(nameof(word));
			if (string.IsNullOrWhiteSpace(word)) return;
			if (word.Length > 10) word = word.Substring(0, 10);
			word = ToLower(word);
			int count;
			stats[word] = stats.TryGetValue(word, out count) ? count + 1 : 1;
		}

		public IEnumerable<Tuple<int, string>> GetStatistics()
		{
			return stats.OrderByDescending(kv => kv.Value).ThenBy(kv => kv.Key).Select(kv => Tuple.Create(kv.Value, kv.Key));
		}

		private char ToLower(char c)
		{
			if ("QWERTYUIOPLJKHGFDSAZXCVBNM".Contains(c))
				return (char)(c - 'D' + 'd');
			else if ("ЙЦУКЕНГШЩЗФЫВАПРОЛДЯЧСМИТЬ".Contains(c))
				return (char)(c - 'Я' + 'я');
			return c;
		}

		private string ToLower(string s)
		{
			return new string(s.Select(ToLower).ToArray());
		}
	}

    [IncorrectImplementation]
    public class WordsStatistics_998 : IWordsStatistics
	{
		private readonly List<Tuple<int, string>> stats = new List<Tuple<int, string>>();

		public void AddWord(string word)
		{
			if (word == null) throw new ArgumentNullException(nameof(word));
			if (string.IsNullOrWhiteSpace(word)) return;
			if (word.Length > 10) word = word.Substring(0, 10);
			var tuple = stats.FirstOrDefault(s => s.Item2 == word.ToLower());
			if (tuple != null)
				stats.Remove(tuple);
			else
				tuple = Tuple.Create(0, word.ToLower());
			stats.Add(Tuple.Create(tuple.Item1 - 1, tuple.Item2));
			stats.Sort();
		}

		public IEnumerable<Tuple<int, string>> GetStatistics()
		{
			return stats.Select(t => Tuple.Create(-t.Item1, t.Item2));
		}
	}

    [IncorrectImplementation]
    public class WordsStatistics_999 : IWordsStatistics
	{
		private readonly HashSet<string> usedWords = new HashSet<string>();
		private readonly List<Tuple<int, string>> stats = new List<Tuple<int, string>>();

		public IEnumerable<Tuple<int, string>> GetStatistics()
		{
			return stats.OrderByDescending(t => t.Item1)
				.ThenBy(t => t.Item2);
		}

		public void AddWord(string word)
		{
			if (word == null) throw new ArgumentNullException(nameof(word));
			if (string.IsNullOrWhiteSpace(word)) return;
			if (word.Length > 10) word = word.Substring(0, 10);
			word = word.ToLower();
			if (usedWords.Contains(word))
			{
				var tuple = stats.First(s => s.Item2 == word);
				stats.Remove(tuple);
				stats.Add(Tuple.Create(tuple.Item1 + 1, tuple.Item2));
			}
			else
			{
				stats.Add(Tuple.Create(1, word));
				usedWords.Add(word);
			}
		}
	}

    [IncorrectImplementation]
    public class WordsStatistics_EN : IWordsStatistics
	{
        private IDictionary<string, int> stats
            = new Dictionary<string, int>();

        public void AddWord(string word)
        {
            if (word == null) throw new ArgumentNullException(nameof(word));
            if (string.IsNullOrWhiteSpace(word)) return;
            if (word.Length > 10)
                word = word.Substring(0, 10);
            int count;
            stats[word.ToLower()] = stats.TryGetValue(word.ToLower(), out count) ? count + 1 : 1;
        }

        public IEnumerable<Tuple<int, string>> GetStatistics()
        {
            var temp = stats;
            stats = new Dictionary<string, int>();
            return temp.OrderByDescending(kv => kv.Value)
                .ThenBy(kv => kv.Key)
                .Select(kv => Tuple.Create(kv.Value, kv.Key));
        }
    }

    [IncorrectImplementation]
    public class WordsStatistics_EN2 : WordsStatistics
	{
        private List<Tuple<int, string>> result;

        public override IEnumerable<Tuple<int, string>> GetStatistics()
        {
            return result ?? (result = base.GetStatistics().ToList());
        }
    }

	#endregion
}