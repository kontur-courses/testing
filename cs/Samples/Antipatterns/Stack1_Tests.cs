using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace Samples.Antipatterns
{
    [TestFixture, Explicit]
	public class Stack1_Tests
	{
		[Test]
		public void Test()
		{
			var lines = File.ReadAllLines(@"C:\work\edu\testing-course\Patterns\bin\Debug\data.txt")
				.Select(line => line.Split(' '))
				.Select(line => new { command = line[0], value = line[1] });

			var stack = new Stack<string>();
			foreach (var line in lines)
			{
				if (line.command == "push")
					stack.Push(line.value);
				else
					Assert.AreEqual(line.value, stack.Pop());
			}
		}

		#region Почему это плохо?
		/*
		## Антипаттерн Local Hero

		Тест не будет работать на машине другого человека или на Build-сервере. 
		Да и у того же самого человека после Clean Solution / переустановки ОС / повторного Clone репозитория / ...

		## Решение

		Тест не должен зависеть от особенностей локальной среды.
		Если нужна работа с файлами, то либо включите файл в проект и настройте в свойствах его копирование в OutputDir,
		либо поместите его в ресурсы.

		var lines = File.ReadAllLines(@"data.txt")
		var lines = Resources.data.Split(new []{"\r\n"}, StringSplitOptions.RemoveEmptyEntries)
		*/
		#endregion
	}
}
