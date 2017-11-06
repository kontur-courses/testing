using System;
using System.Collections.Generic;

namespace Challenge
{
	public interface IWordsStatistics
	{
		void AddWord(string word);

		/**
		<summary>
		Частотный словарь добавленных слов. 
		Слова сравниваются без учета регистра символов. 
		Порядок — по убыванию частоты слова.
		При одинаковой частоте — в лексикографическом порядке.
		</summary>
		*/
		IEnumerable<Tuple<int, string>> GetStatistics();
	}
}