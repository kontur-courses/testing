using System;
using Challenge.IncorrectImplementations;
using NUnit.Framework;

namespace Challenge.Infrastructure
{
	public abstract class IncorrectImplementation_TestsBase : WordsStatistics_Tests
	{
	    public override IWordsStatistics CreateStatistics()
		{
			string ns = typeof(WordsStatisticsL).Namespace;
			var implTypeName = ns + "." + GetType().Name.Replace("_Tests", "");
			var implType = GetType().Assembly.GetType(implTypeName);
			if (implType == null)
				Assert.Fail("no type {0}", implTypeName);
			return (IWordsStatistics) Activator.CreateInstance(implType);
		}
	}
}

