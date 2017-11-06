using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Samples.AAA
{
	[TestFixture]
	public class Zip_Should
	{
		[Test]
		public void GiveResultOfSameSize_OnEqualSizeArrays()
		{
			var arr1 = new[] { 1 };
			var arr2 = new[] { 2 };

			var result = arr1.Zip(arr2, Tuple.Create);

			CollectionAssert.AreEqual(new[] { Tuple.Create(1, 2) }, result);
		}

		[Test]
		public void BeEmpty_WhenBothInputAreEmpty()
		{
			var arr1 = new int[0];
			var arr2 = new int[0];

			var result = arr1.Zip(arr2, Tuple.Create);

			CollectionAssert.IsEmpty(result);
		}

		[Test]
		public void BeEmpty_WhenFirstIsEmpty()
		{
			var arr1 = new int[0];
			var arr2 = new[] { 1, 2 };

			var result = arr1.Zip(arr2, Tuple.Create);

			CollectionAssert.IsEmpty(result);
		}

		[Test]
		public void BeEmpty_WhenSecondIsEmpty()
		{
			var arr1 = new[] { 1, 2 };
			var arr2 = new int[0];

			var result = arr1.Zip(arr2, Tuple.Create);

			CollectionAssert.IsEmpty(result);
		}

		[Test]
		public void HaveLengthOfSecond_WhenFirstIsInfinite()
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
		public void HaveLengthOfFirst_WhenSecondIsInfinite()
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
			// ReSharper disable once IteratorNeverReturns
		}
	}
}