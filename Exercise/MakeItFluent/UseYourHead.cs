using System.Collections.Generic;
using NUnit.Framework;

namespace MakeItFluent
{
	public class UseYourHead
	{
		public List<string> List = new List<string>();
		// Задача - отрефакторить и стабилизировать тест с более разумным использованием 
		// аттрибута timeout.

		[Test]
		[Timeout(6)]
		public void Test()
		{
			AddToListAndSort("1");
			AddToListAndSort("1");
			AddToListAndSort("цуфа5");
			AddToListAndSort("5ыавы");
			AddToListAndSort("ываыва5");
		}

		public void AddToListAndSort(string item)
		{
			List.Sort();
			List.Add(item);
			List.Sort();
		}
	}
}