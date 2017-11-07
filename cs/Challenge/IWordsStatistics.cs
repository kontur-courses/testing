using System;
using System.Collections.Generic;

namespace Challenge
{
	public interface IWordsStatistics
	{
		void AddWord(string word);
		IEnumerable<Tuple<int, string>> GetStatistics();
	}
}