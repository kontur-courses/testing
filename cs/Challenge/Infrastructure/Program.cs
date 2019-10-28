using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Xml;
using NUnit.Engine;

namespace Challenge.Infrastructure
{
	internal class Program
	{
		private static void Main()
		{
			if (string.IsNullOrWhiteSpace(YourName.Authors))
			{
				Console.WriteLine("Enter your surnames at YourName.cs in AUTHORS constant");
				Thread.Sleep(3000);
				return;
			}

			var testPackage = new TestPackage(Assembly.GetExecutingAssembly().Location);
			using (var engine = new TestEngine())
			using (var testRunner = engine.GetRunner(testPackage))
			{
				if (TestsAreValid(testRunner))
				{
					var incorrectImplementations = ChallengeHelpers.GetIncorrectImplementationTypes();
					var statuses = GetIncorrectImplementationsResults(testRunner, incorrectImplementations);
					var res = new List<ImplementationStatus>();
					foreach (var status in statuses)
					{
						res.Add(status);
						WriteImplementationStatusToConsole(status);
					}

					if (!string.IsNullOrWhiteSpace(YourName.Authors))
						PostResults(res);
				}
			}
		}

		private static void PostResults(IList<ImplementationStatus> statuses)
		{
			try
			{
				using (var client = Firebase.CreateClient())
				{
					var authorsKey = MakeFirebaseSafe(YourName.Authors);
					var values = statuses.ToDictionary(s => s.Name, s => s.Fails.Length);
					//client.Set(authorsKey + "/implementations", values);
					client.Set(authorsKey, new
					{
						implementations = values,
						time = DateTime.Now.ToUniversalTime(),
						lang = "cs"
					});
				}

				Console.WriteLine("");
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
			}
		}

		private static string MakeFirebaseSafe(string s)
		{
			var badChars = Enumerable.Range(0, 32).Select(code => (char) code)
				.Concat(".$#[]/" + (char) 127);
			foreach (var badChar in badChars)
				s = s.Replace(badChar, '_');
			return s;
		}

		private static IEnumerable<ImplementationStatus> GetIncorrectImplementationsResults(
			ITestRunner testRunner, IEnumerable<Type> implementations)
		{
			var implTypeToTestsType = ChallengeHelpers.GetIncorrectImplementationTests()
				.ToDictionary(t => t.CreateStatistics().GetType(), t => t.GetType());
			foreach (var implementation in implementations)
			{
				var failed = GetFailedTests(testRunner,
						implementation,
						implTypeToTestsType[implementation])
					.ToArray();
				yield return new ImplementationStatus(implementation.Name, failed);
			}
		}

		private static void WriteImplementationStatusToConsole(ImplementationStatus status)
		{
			var paddedName = status.Name.PadRight(20, ' ');
			if (status.Fails.Any())
			{
				Console.ForegroundColor = ConsoleColor.Green;
				Console.WriteLine(TrimToConsole(paddedName + "fails on: " + string.Join(", ", status.Fails)));
				Console.ForegroundColor = ConsoleColor.Gray;
			}
			else
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(paddedName + "write tests to kill it");
				Console.ForegroundColor = ConsoleColor.Gray;
			}
		}

		private static string TrimToConsole(string text)
		{
			try
			{
				var w = Console.WindowWidth - 1;
				return text.Length > w ? text.Substring(0, w) : text;
			}
			catch (IOException)
			{
				return text;
			}
		}

		private static bool TestsAreValid(ITestRunner testRunner)
		{
			Console.WriteLine("Check all tests pass with correct implementation...");
			var failed = GetFailedTests(testRunner,
					typeof(WordsStatistics),
					typeof(WordsStatistics_Tests))
				.ToList();
			if (failed.Any())
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("Incorrect tests detected: " + string.Join(", ", failed));
				Console.ForegroundColor = ConsoleColor.Gray;
				return false;
			}
			else
			{
				Console.ForegroundColor = ConsoleColor.Green;
				Console.WriteLine("Tests are OK!");
				Console.ForegroundColor = ConsoleColor.Gray;
				return true;
			}
		}

		private static IEnumerable<string> GetFailedTests(ITestRunner testRunner,
			Type implementationType, Type testsType)
		{
			var builder = new TestFilterBuilder();
			builder.AddTest(testsType.FullName);
			var report = testRunner.Run(null, builder.GetFilter());
			Debug.Assert(report != null);
			File.WriteAllText($"{implementationType.Name}.nunitReport.xml", report.OuterXml);
			var failedTestCases = report.SelectNodes("//test-case[@result='Failed']");
			Debug.Assert(failedTestCases != null);
			foreach (var xmlNode in failedTestCases.Cast<XmlNode>())
			{
				Debug.Assert(xmlNode.Attributes != null);
				yield return xmlNode.Attributes["name"].Value;
			}
		}
	}
}