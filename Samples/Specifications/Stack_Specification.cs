using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Samples.Specifications
{
	[TestFixture]
	public class Stack_Specification
	{
		[Test]
		public void defaultConstructor_createsEmptyStack()
		{
			// ReSharper disable once CollectionNeverUpdated.Local
			var stack = new Stack<int>();

			Assert.AreEqual(0, stack.Count);
		}

		[Test]
		public void pop_onEmptyStack_fails()
		{
			var stack = new Stack<int>();

			Assert.Throws<InvalidOperationException>(() => stack.Pop());
		}

		[Test]
		public void constructor_pushesItemsToEmptyStack()
		{
			var stack = new Stack<int>(new[] { 1, 2, 3 });

			Assert.AreEqual(3, stack.Count);
			Assert.AreEqual(3, stack.Pop());
			Assert.AreEqual(2, stack.Pop());
			Assert.AreEqual(1, stack.Pop());
			Assert.AreEqual(0, stack.Count);
		}

		[Test]
		public void enumeration_returnsItemsInPopOrder()
		{
			var stack = new Stack<int>(new[] { 1, 2, 3 });

			Assert.AreEqual(new[] { 3, 2, 1 }, stack.ToArray());
		}

		[Test]
		public void push_addItemToStackTop()
		{
			var stack = new Stack<int>(new[] { 1, 2, 3 });

			stack.Push(42);

			CollectionAssert.AreEqual(new[] { 42, 3, 2, 1 }, stack);
		}

		[Test]
		public void pop_returnsLastPushedItem()
		{
			var stack = new Stack<int>(new[] { 1, 2, 3 });

			stack.Push(42);

			Assert.AreEqual(42, stack.Pop());
		}
	}
}