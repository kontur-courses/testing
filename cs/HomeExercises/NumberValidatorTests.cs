using System;
using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		#region TestCases

		public static object[][] numberTestCases =
		{
			new object[] { 1, 0, "0" },
			new object[] { 2, 1, "0.0" },
			new object[] { 1, 0, "1" },
			new object[] { 2, 0, "12" },
			new object[] { 2, 1, "1.2" },
			new object[] { 3, 2, "1.23" },
			new object[] { 3, 1, "12.3" },
			new object[] { 4, 2, "12.34" },
		};

		public static IEnumerable CorrectSignedNumbersCases()
		{
			int precision, scale;
			string validatingString;
			foreach (var testCase in numberTestCases)
			{
				precision = (int)testCase[0];
				scale = (int)testCase[1];
				validatingString = (string)testCase[2];
				yield return new TestCaseData(precision + 1, scale, $"+{validatingString}").Returns(true);
				yield return new TestCaseData(precision + 1, scale, $"-{validatingString}").Returns(true);
			}
		}

		public static IEnumerable CorrectUnsignedNumbersCases()
		{
			foreach (var testCase in numberTestCases)
			{
				yield return new TestCaseData(testCase).Returns(true);
			}
		}

		public static IEnumerable IncorrectUnsignedNumbersCases()
		{
			int precision, scale;
			string validatingString;
			foreach (var testCase in numberTestCases)
			{
				precision = (int)testCase[0];
				scale = (int)testCase[1];
				validatingString = (string)testCase[2];
				if (precision > 1 && precision - 1 > scale)
					yield return new TestCaseData(precision - 1, scale, validatingString).Returns(false);
				if (scale > 0)
					yield return new TestCaseData(testCase[0], scale - 1, validatingString).Returns(false);
			}
		}

		public static IEnumerable IncorrectSignedNumbersCases()
		{
			int precision, scale;
			string validatingString;
			foreach (var testCase in numberTestCases)
			{
				precision = (int)testCase[0];
				scale = (int)testCase[1];
				validatingString = (string)testCase[2];
				yield return new TestCaseData(precision, scale, $"+{validatingString}").Returns(false);
				yield return new TestCaseData(precision, scale, $"-{validatingString}").Returns(false);
				if (scale > 0 && precision > scale)
				{
					yield return new TestCaseData(precision + 1, scale - 1, $"+{validatingString}")
						.Returns(false);
					yield return new TestCaseData(precision + 1, scale - 1, $"-{validatingString}")
						.Returns(false);
				}
			}
		}

		public static IEnumerable NegativeNumbers()
		{
			int precision;
			string validatingString;
			foreach (var testCase in numberTestCases)
			{
				precision = (int)testCase[0];
				validatingString = (string)testCase[2];
				yield return new TestCaseData(precision + 1, testCase[1], $"-{validatingString}").Returns(false);
			}
		}

		#endregion

		[TestCaseSource(nameof(CorrectSignedNumbersCases))]
		[TestCaseSource(nameof(CorrectUnsignedNumbersCases))]
		[TestCaseSource(nameof(IncorrectUnsignedNumbersCases))]
		[TestCaseSource(nameof(IncorrectSignedNumbersCases))]
		public bool ValidationTests(int precision, int scale, string validatingString)
		{
			return new NumberValidator(precision, scale).IsValidNumber(validatingString);
		}

		[TestCaseSource(nameof(NegativeNumbers))]
		public bool ShouldFailWhenNumberNegativeWithOnlyPositive(int precision, int scale, string validatingString)
		{
			return new NumberValidator(precision, scale, true).IsValidNumber(validatingString);
		}

		[TestCase(3, 2, "a.sd", TestName = "non digit symbols")]
		[TestCase(2, 1, ".0", TestName = "must have digits before point")]
		[TestCase(1, 0, "0.", TestName = "must have digits after point (if exist)")]
		public void IsNotValid(int precision, int scale, string validatingString, bool onlyPositive = true)
		{
			new NumberValidator(precision, scale, onlyPositive)
				.IsValidNumber(validatingString)
				.Should()
				.BeFalse();
		}

		[TestCase(-1, 1, TestName = "negative precision")]
		[TestCase(1, -1, TestName = "negative scale")]
		[TestCase(-1, -1, TestName = "negative precision and scale")]
		[TestCase(1, 1, TestName = "precision equals scale")]
		[TestCase(1, 2, TestName = "precision less than scale")]
		public void ShouldThrow(int precision, int scale, bool onlyPositive = true)
		{
			Action act = () => new NumberValidator(precision, scale, onlyPositive);
			act.Should().Throw<ArgumentException>();
		}
	}

	public class NumberValidator
	{
		private readonly Regex numberRegex;
		private readonly bool onlyPositive;
		private readonly int precision;
		private readonly int scale;

		public NumberValidator(int precision, int scale = 0, bool onlyPositive = false)
		{
			this.precision = precision;
			this.scale = scale;
			this.onlyPositive = onlyPositive;
			if (precision <= 0)
				throw new ArgumentException("precision must be a positive number");
			if (scale < 0 || scale >= precision)
				throw new ArgumentException("precision must be a non-negative number less or equal than precision");
			numberRegex = new Regex(@"^([+-]?)(\d+)([.,](\d+))?$", RegexOptions.IgnoreCase);
		}

		public bool IsValidNumber(string value)
		{
			// Проверяем соответствие входного значения формату N(m,k), в соответствии с правилом, 
			// описанным в Формате описи документов, направляемых в налоговый орган в электронном виде по телекоммуникационным каналам связи:
			// Формат числового значения указывается в виде N(m.к), где m – максимальное количество знаков в числе, включая знак (для отрицательного числа), 
			// целую и дробную часть числа без разделяющей десятичной точки, k – максимальное число знаков дробной части числа. 
			// Если число знаков дробной части числа равно 0 (т.е. число целое), то формат числового значения имеет вид N(m).

			if (string.IsNullOrEmpty(value))
				return false;

			var match = numberRegex.Match(value);
			if (!match.Success)
				return false;

			// Знак и целая часть
			var intPart = match.Groups[1].Value.Length + match.Groups[2].Value.Length;
			// Дробная часть
			var fracPart = match.Groups[4].Value.Length;

			if (intPart + fracPart > precision || fracPart > scale)
				return false;

			if (onlyPositive && match.Groups[1].Value == "-")
				return false;
			return true;
		}
	}
}