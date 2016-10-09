using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Samples.AAA
{
	[TestFixture, Explicit]
	public class Zip_Tests
	{
	    private int Field = 5;

		[Test]
		public void EqualSizeArrays()
		{
			var arr1 = new[] { 1 };
			var arr2 = new[] { 2 };

			var result = arr1.Zip(arr2, Tuple.Create);

            CollectionAssert.AreEqual(new [] { Tuple.Create(1, 2) }, result);
		}

		[Test]
		public void BothEmpty()
		{
			var arr1 = new int[0];
			var arr2 = new int[0];

			var result = arr1.Zip(arr2, Tuple.Create);

            CollectionAssert.IsEmpty(result);
		}

		[Test]
		public void FirstIsEmpty()
		{
			var arr1 = new int[0];
			var arr2 = new[] { 1, 2 };

			var result = arr1.Zip(arr2, Tuple.Create);

            CollectionAssert.IsEmpty(result);
        }

		[Test]
		public void SecondIsEmpty()
		{
			var arr1 = new[] { 1, 2 };
			var arr2 = new int[0];

			var result = arr1.Zip(arr2, Tuple.Create);

            CollectionAssert.IsEmpty(result);
        }

		[Test]
		public void FirstIsInfinite()
		{
			var arr2 = new[] { 1, 2 };
			var infiniteArr2 = Infinite();

			var result = infiniteArr2.Zip(arr2, Tuple.Create);

		    CollectionAssert.AreEqual(new[]
		    {
		        Tuple.Create(42, 1),
		        Tuple.Create(42, 2)
		    }, result);
		}

		[Test]
		public void SecondIsInfinite()
		{
			var arr1 = new[] { 1, 2 };

			var result = arr1.Zip(Infinite(), Tuple.Create);

            CollectionAssert.AreEqual(new[]
            {
                Tuple.Create(1, 42),
                Tuple.Create(2, 42)
            }, result);
		}

		private static IEnumerable<int> Infinite()
		{
			while (true)
				yield return 42;
			// ReSharper disable once FunctionNeverReturns
		}   
	}
}