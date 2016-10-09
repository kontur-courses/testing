using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Samples.Antipatterns
{
    [TestFixture, Explicit]
    public class Stack_tests3
	{
		[Test]
		public void Test()
		{
			var stack = new Stack<int>();
			CollectionAssert.IsEmpty(stack);
			stack.Push(1);
			stack.Pop();
			CollectionAssert.IsEmpty(stack);
			stack.Push(1);
			stack.Push(2);
			stack.Push(3);
			Assert.AreEqual(3, stack.Count);
			stack.Pop();
			stack.Pop();
			stack.Pop();
			CollectionAssert.IsEmpty(stack);
			foreach (var i in Enumerable.Range(0, 1000))
				stack.Push(i);
			foreach (var i in Enumerable.Range(0, 1000).Reverse())
				Assert.AreEqual(i, stack.Pop());

		}
		#region Почему этот тест плохой?
		/*
		## Антипаттерн Freeride

		1. Непонятна область его ответственности. Складывается впечатление, что он тестирует все, однако он это делает плохо.
		Он дает ложное чувство, что все протестировано. Хотя, например, этот тест не проверяет много важных случаев.

		2. Таким тестам как-правило невозможно придумать внятное название.
		
		3. Если что-то упадет в середине теста, будет сложно разобраться что именно пошло не так и сложно отлаживать — нужно жонглировать точками останова.
		
		4. Такой тест не работает как документация. По этому сценарию непросто восстановить требования к тестируемому объекту.

		## Мораль

		Каждый тест должен тестировать одно конкретное требование. Это требование должно отражаться в названии теста.
		Если вы не можете придумать название теста, у вас Loudmouth!
		*/
		#endregion
	}
}
