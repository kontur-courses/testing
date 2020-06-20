using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Samples.Specifications
{
	[TestFixture]
	public class Stack_Specification
	{
		[Test]
		public void Constructor_CreatesEmptyStack()
		{
			// ReSharper disable once CollectionNeverUpdated.Local
			var stack = new Stack<int>();

			Assert.AreEqual(0, stack.Count);
		}

		[Test]
		public void Constructor_PushesItemsToEmptyStack()
		{
			var stack = new Stack<int>();
			
			stack.Push(1);
			stack.Push(2);
			
			Assert.AreEqual(2, stack.Count);
		}

		[Test]
		public void Constructor_PopItemsFromStack()
		{
			var stack = new Stack<int>(new[] { 1, 2, 3 });

			Assert.AreEqual(3, stack.Count);
			Assert.AreEqual(3, stack.Pop());
			Assert.AreEqual(2, stack.Pop());
			Assert.AreEqual(1, stack.Pop());
			Assert.AreEqual(0, stack.Count);
		}

		[Test]
		public void Constructor_PeekNotDeleteItemsFromStack()
		{
			var stack = new Stack<int>(new[] { 1, 2 });

			Assert.AreEqual(2, stack.Count);
			Assert.AreEqual(2, stack.Peek());
			Assert.AreEqual(2, stack.Peek());
			Assert.AreEqual(2, stack.Count);
		}

		[Test]
		public void ToArray_ReturnsItemsInPopOrder()
		{
			var stack = new Stack<int>(new[] { 1, 2, 3 });

			Assert.AreEqual(new[] { 3, 2, 1 }, stack.ToArray());
		}

		[Test]
		public void Push_AddsItemToStackTop()
		{
			var stack = new Stack<int>(new[] { 1, 2, 3 });

			stack.Push(42);

			CollectionAssert.AreEqual(new[] { 42, 3, 2, 1 }, stack.ToArray());
		}

		[Test]
		public void Pop_OnEmptyStack_Fails()
		{
			var stack = new Stack<int>();

			Assert.Throws<InvalidOperationException>(() => stack.Pop());
		}
		
		[Test]
		public void Pop_ReturnsLastPushedItem()
		{
			var stack = new Stack<int>(new[] { 1, 2, 3 });

			stack.Push(42);

			Assert.AreEqual(42, stack.Pop());
		}
	}
}
