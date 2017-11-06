using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Samples.Antipatterns
{
    [TestFixture, Explicit]
    public class Stack4_Tests
	{
		[Test]
		public void TestPop()
		{
			var stack = new Stack<int>(new[] { 1, 2, 3, 4, 5 });
			var result = stack.Pop();
			Assert.AreEqual(5, result);
			Assert.IsTrue(stack.Any());
			Assert.AreEqual(4, stack.Count);
			Assert.AreEqual(4, stack.Peek());
			Assert.AreEqual(new[] { 4, 3, 2, 1 }, stack.ToArray());
		}

		#region Почему это плохо?
		/*	
		## Антипаттерн Overspecification

		1. Непонятна область ответственности. Сложно придумать название. Не так плохо, как FreeRide, но плохо.

		2. Изменение API роняет сразу много подобных тестов, создавая много рутинной работы по их починке.

		3. Если все тесты будут такими, то при появлении бага, падают они большой компанией.


		## Мораль

		Сфокусируйтесь на проверке одного конкретного требования в каждом тесте.
		Не старайтесь проверить "за одно" какое-то требование сразу во всех тестах — это может выйти боком.

		Признак возможной проблемы — более одного Assert на метод.
		*/
		#endregion
	}
}