using System;
using System.Collections.Generic;
using System.Linq;
using Challenge.Infrastructure;

namespace Challenge.IncorrectImplementations
{
    #region Не подглядывать!

    [IncorrectImplementation]
    public class WordsStatisticsL2 : WordsStatistics
    {
        public override void AddWord(string word)
        {
            if (word == null) throw new ArgumentNullException(nameof(word));
            if (string.IsNullOrWhiteSpace(word)) return;
            int count;
			statistics[word.ToLower()] = 1 + (statistics.TryGetValue(word.ToLower(), out count) ? count : 0);
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
            else if (word.Length > 5) word = word.Substring(0, word.Length - 2);
            int count;
			statistics[word.ToLower()] = 1 + (statistics.TryGetValue(word.ToLower(), out count) ? count : 0);
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
			statistics[word.ToLower()] = 1 + (statistics.TryGetValue(word.ToLower(), out count) ? count : 0);
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
            if (!statistics.ContainsKey(word.ToLower()))
                statistics[word] = 0;
            statistics[word]++;
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
			statistics[word.ToLower()] = 1 + (statistics.TryGetValue(word.ToLower(), out count) ? count : 0);
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
			statistics[word.ToLower()] = 1 + (statistics.TryGetValue(word.ToLower(), out count) ? count : 0);
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
			statistics[word.ToLower()] = 1 + (statistics.TryGetValue(word.ToLower(), out count) ? count : 0);
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
			statistics[word.ToLower()] = 1 + (statistics.TryGetValue(word.ToLower(), out count) ? count : 0);
		}
    }

    [IncorrectImplementation]
    public class WordsStatisticsO1 : WordsStatistics
    {
        public override IEnumerable<WordCount> GetStatistics()
        {
            return statistics
                .Select(WordCount.Create)
                .OrderBy(wordCount => wordCount.Word);
        }
    }

    [IncorrectImplementation]
    public class WordsStatisticsO2 : WordsStatistics
    {
        public override IEnumerable<WordCount> GetStatistics()
        {
            return statistics
                .Select(WordCount.Create)
                .OrderBy(wordCount => wordCount.Count);
        }
    }

    [IncorrectImplementation]
    public class WordsStatisticsO3 : WordsStatistics
    {
        public override IEnumerable<WordCount> GetStatistics()
        {
            return base.GetStatistics().OrderBy(wordCount => wordCount.Word);
        }
    }

    [IncorrectImplementation]
    public class WordsStatisticsO4 : WordsStatistics
    {
        public override IEnumerable<WordCount> GetStatistics()
        {
            return base.GetStatistics()
                .OrderByDescending(wordCount => wordCount.Count)
                .ThenByDescending(wordCount => wordCount.Word);
        }
    }

    [IncorrectImplementation]
    public class WordsStatisticsCR : IWordsStatistics
    {
        private readonly IDictionary<string, int> statistics =
            new Dictionary<string, int>();

        public void AddWord(string word)
        {
            if (word == null) throw new ArgumentNullException(nameof(word));
            if (string.IsNullOrEmpty(word)) return;
            if (word.Length > 10) word = word.Substring(0, 10);
            word = word.ToLower();
            int count;
            statistics[word] = 1 + (statistics.TryGetValue(word, out count) ? count : 0);
        }

        public IEnumerable<WordCount> GetStatistics()
        {
            return statistics
                .Select(WordCount.Create)
                .OrderByDescending(wordCount => wordCount.Count)
                .ThenBy(wordCount => wordCount.Word);
        }
    }

    [IncorrectImplementation]
    public class WordsStatisticsSTA : IWordsStatistics
    {
        private static readonly IDictionary<string, int> statistics = new Dictionary<string, int>();

        public WordsStatisticsSTA()
        {
	        statistics.Clear();
        }

        public void AddWord(string word)
        {
            if (word == null) throw new ArgumentNullException(nameof(word));
            if (string.IsNullOrWhiteSpace(word)) return;
            if (word.Length > 10) word = word.Substring(0, 10);
            int count;
			statistics[word.ToLower()] = 1 + (statistics.TryGetValue(word.ToLower(), out count) ? count : 0);
		}

        public IEnumerable<WordCount> GetStatistics()
        {
            return statistics
                .Select(WordCount.Create)
                .OrderByDescending(wordCount => wordCount.Count)
                .ThenBy(wordCount => wordCount.Word);
        }
    }

    [IncorrectImplementation]
    public class WordsStatistics123 : IWordsStatistics
    {
        private const int MAX_SIZE = 1237;

        private readonly int[] statistics = new int[MAX_SIZE];
        private readonly string[] words = new string[MAX_SIZE];

        public void AddWord(string word)
        {
            if (word == null) throw new ArgumentNullException(nameof(word));
            if (string.IsNullOrWhiteSpace(word)) return;
            if (word.Length > 10) word = word.Substring(0, 10);
            var index = Math.Abs(word.ToLower().GetHashCode()) % MAX_SIZE;
	        statistics[index]++;
            words[index] = word.ToLower();
        }

        public IEnumerable<WordCount> GetStatistics()
        {
            return statistics.Zip(words, (s, w) => new WordCount(w, s))
                .Where(t => t.Count > 0)
                .OrderByDescending(t => t.Count)
                .ThenBy(t => t.Word);
        }
    }

    [IncorrectImplementation]
    public class WordsStatisticsQWE : IWordsStatistics
    {
        private readonly IDictionary<string, int> statistics = new Dictionary<string, int>();

        public void AddWord(string word)
        {
            if (word == null) throw new ArgumentNullException(nameof(word));
            if (string.IsNullOrWhiteSpace(word)) return;
            if (word.Length > 10) word = word.Substring(0, 10);
            word = ToLower(word);
            int count;
	        statistics[word] = 1 + (statistics.TryGetValue(word, out count) ? count : 0);
        }

        public IEnumerable<WordCount> GetStatistics()
        {
            return statistics
                .Select(WordCount.Create)
                .OrderByDescending(wordCount => wordCount.Count)
                .ThenBy(wordCount => wordCount.Word);
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
    public class WordsStatistics998 : IWordsStatistics
    {
        private readonly List<WordCount> statistics = new List<WordCount>();

        public void AddWord(string word)
        {
            if (word == null) throw new ArgumentNullException(nameof(word));
            if (string.IsNullOrWhiteSpace(word)) return;
            if (word.Length > 10) word = word.Substring(0, 10);
	        var lowerCaseWord = word.ToLower();
	        var wordCount = statistics.FirstOrDefault(s => s.Word == lowerCaseWord);
            if (wordCount.Word != null)
                statistics.Remove(wordCount);
            else
                wordCount = new WordCount(lowerCaseWord, 0);
            statistics.Add(new WordCount(wordCount.Word, wordCount.Count - 1));

            statistics.Sort((a, b) => a.Count == b.Count
                ? string.Compare(a.Word, b.Word, StringComparison.Ordinal)
                : a.Count - b.Count);
        }

        public IEnumerable<WordCount> GetStatistics()
        {
            return statistics.Select(w => new WordCount(w.Word, -w.Count));
        }
    }

    [IncorrectImplementation]
    public class WordsStatistics999 : IWordsStatistics
    {
        private readonly HashSet<string> usedWords = new HashSet<string>();
        private readonly List<WordCount> statistics = new List<WordCount>();

        public void AddWord(string word)
        {
            if (word == null) throw new ArgumentNullException(nameof(word));
            if (string.IsNullOrWhiteSpace(word)) return;
            if (word.Length > 10) word = word.Substring(0, 10);
            word = word.ToLower();
            if (usedWords.Contains(word))
            {
                var stat = statistics.First(s => s.Word == word);
                statistics.Remove(stat);
                statistics.Add(new WordCount(stat.Word, stat.Count + 1));
            }
            else
            {
                statistics.Add(new WordCount(word, 1));
                usedWords.Add(word);
            }
        }

	    public IEnumerable<WordCount> GetStatistics()
	    {
		    return statistics
                .OrderByDescending(t => t.Count)
			    .ThenBy(t => t.Word);
	    }
	}

    [IncorrectImplementation]
    public class WordsStatisticsEN1 : IWordsStatistics
    {
        private IDictionary<string, int> statistics
            = new Dictionary<string, int>();

        public void AddWord(string word)
        {
            if (word == null) throw new ArgumentNullException(nameof(word));
            if (string.IsNullOrWhiteSpace(word)) return;
            if (word.Length > 10)
                word = word.Substring(0, 10);
            int count;
            statistics[word.ToLower()] = 1 + (statistics.TryGetValue(word.ToLower(), out count) ? count : 0);
        }

        public IEnumerable<WordCount> GetStatistics()
        {
            var temp = statistics;
            statistics = new Dictionary<string, int>();
            return temp
                .Select(WordCount.Create)
                .OrderByDescending(wordCount => wordCount.Count)
                .ThenBy(wordCount => wordCount.Word);
        }
    }

    [IncorrectImplementation]
    public class WordsStatisticsEN2 : WordsStatistics
    {
        private List<WordCount> result;

        public override IEnumerable<WordCount> GetStatistics()
        {
            return result ?? (result = base.GetStatistics().ToList());
        }
    }

    #endregion
}