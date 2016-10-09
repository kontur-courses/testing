using System;
using System.Linq;
using NUnit.Framework;

namespace Challenge.Infrastructure
{
    [TestFixture]
	public class GenerateIncorrectTests
	{
		[Test]
		public void Generate()
		{
			var impls = ChallengeHelpers.GetIncorrectImplementationTypes();
			var code = string.Join(Environment.NewLine,
				impls.Select(imp => $"public class {imp.Name}_Tests : {nameof(IncorrectImplementation_TestsBase)} {{}}")
				);
			Console.WriteLine(code);
		}

		[Test]
		public void CheckAllTestsAreInPlace()
		{
			var implTypes = ChallengeHelpers.GetIncorrectImplementationTypes();
		    var testedImpls = ChallengeHelpers.GetIncorrectImplementationTests()
                .Select(t => t.CreateStatistics())
                .ToArray();

            foreach (var impl in implTypes)
			{
				Assert.NotNull(testedImpls.SingleOrDefault(t => t.GetType().FullName == impl.FullName),
                    "Single implementation of tests for {0} not found. Regenerate tests with test above!", impl.FullName);
			}
		}
	}

    #region Generated with test above

    public class WordsStatisticsC_Tests : IncorrectImplementation_TestsBase { }
    public class WordsStatisticsE_Tests : IncorrectImplementation_TestsBase { }
    public class WordsStatisticsL_Tests : IncorrectImplementation_TestsBase { }
    public class WordsStatisticsCR_Tests : IncorrectImplementation_TestsBase { }
    public class WordsStatisticsE2_Tests : IncorrectImplementation_TestsBase { }
    public class WordsStatisticsE3_Tests : IncorrectImplementation_TestsBase { }
    public class WordsStatisticsE4_Tests : IncorrectImplementation_TestsBase { }
    public class WordsStatisticsL2_Tests : IncorrectImplementation_TestsBase { }
    public class WordsStatisticsL3_Tests : IncorrectImplementation_TestsBase { }
    public class WordsStatisticsL4_Tests : IncorrectImplementation_TestsBase { }
    public class WordsStatisticsO1_Tests : IncorrectImplementation_TestsBase { }
    public class WordsStatisticsO2_Tests : IncorrectImplementation_TestsBase { }
    public class WordsStatisticsO3_Tests : IncorrectImplementation_TestsBase { }
    public class WordsStatisticsO4_Tests : IncorrectImplementation_TestsBase { }
    public class WordsStatisticsO5_Tests : IncorrectImplementation_TestsBase { }
    public class WordsStatistics_123_Tests : IncorrectImplementation_TestsBase { }
    public class WordsStatistics_998_Tests : IncorrectImplementation_TestsBase { }
    public class WordsStatistics_999_Tests : IncorrectImplementation_TestsBase { }
    public class WordsStatistics_QWE_Tests : IncorrectImplementation_TestsBase { }
    public class WordsStatistics_STA_Tests : IncorrectImplementation_TestsBase { }

    #endregion
}