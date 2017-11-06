using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Samples.Antipatterns
{
    [TestFixture, Explicit]
    public class Stack2_Tests
	{
		[Test]
		public void TestPushPop()
		{
			var stack = new Stack<int>();
			stack.Push(10);
			stack.Push(20);
			stack.Push(30);
			while (stack.Any())
				Console.WriteLine(stack.Pop());
		}

		#region Почему это плохо?
		/*
		## Антипаттерн Loudmouth

		Тест не является автоматическим. Если он сломается, никто этого не заметит.

		## Мораль

		Вместо вывода на консоль, используйте Assert-ы.
		*/
		#endregion
	}
}
