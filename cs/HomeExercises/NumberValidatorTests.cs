using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[TestCaseSource(nameof(ValidDataSource))]
		public void Should_CorrectParse_OnCorrectData(int precision, int scale, bool isOnlyPositive, string input, bool expected)
		{
			Assert.AreEqual(expected, 
				new NumberValidator(precision, scale, isOnlyPositive).IsValidNumber(input));
		}
		
		
		[TestCaseSource(nameof(InvalidDataSource))]
		public void Should_CorrectParse_OnInvalidData(int precision, int scale, bool isOnlyPositive, string input, bool expected)
		{
			Assert.AreEqual(expected, 
				new NumberValidator(precision, scale, isOnlyPositive).IsValidNumber(input));
		}

		[Test]
		public void Should_ThrowArgumentException_OnNegativePrecision()
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(-1, 2, true));
		}
		
		[Test]
		public void Should_ThrowArgumentException_OnNegativeScale()
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(2, -1, true));
		}
		
		[Test]
		public void Should_ThrowArgumentException_OnPrecisionEqualThanScale()
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(1, 1, true));
		}
		
		[Test]
		public void Should_ThrowArgumentException_OnPrecisionLessThanScale()
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(1, 2, true));
		}

		private static IEnumerable ValidDataSource()
		{
			var data = new List<TestCaseData> {
					new TestCaseData(17, 2, true, "-0", false).SetName("Zero with specified precision"), 
					new TestCaseData(17, 2, true, "0,0", true).SetName("Zero with specified by comma precision"),
					new TestCaseData(17, 2, true, "0", true).SetName("Zero without precision"),
					new TestCaseData(17, 2, true, "01", true).SetName("Leading zeros"),
					new TestCaseData(17, 2, true, "0.10", true).SetName("Trailing zeros"),
					new TestCaseData(1, 0, true, "01", true).SetName("Leading zeros doesn't affect on precision"),
					new TestCaseData(17, 2, true, "1.23", true).SetName("Number with max scale"),
					new TestCaseData(4, 0, true, "1234", true).SetName("Number with max precision"),
					new TestCaseData(4, 2, true, "12.34", true).SetName("Number with max precision and max scale"),
					new TestCaseData(1, 0, true, "11", false).SetName("Actual number precision more then specified precision"),
					new TestCaseData(17, 2, true, "1.234", false).SetName("Actual number scale more then specified scale"),
					new TestCaseData(17, 2, false, "+0", true).SetName("Positive signed number with false onlyPositive parameter"),
					new TestCaseData(17, 2, true, "+0", true).SetName("Positive signed number with true onlyPositive parameter")
			};
			foreach (var caseData in data)
				yield return caseData;
		}

		private static IEnumerable InvalidDataSource()
		{
			var data = new List<TestCaseData>
			{
				new TestCaseData(17, 2, false, null, false).SetName("Null string for parsing"),
				new TestCaseData(17, 2, false, "", false).SetName("Empty string for parsing"),
				new TestCaseData(17, 2, false, "abc.1", false).SetName("Not decimal string mixed with decimal for parsing"),
				new TestCaseData(17, 2, false, "abc", false).SetName("Not decimal string for parsing"),
				new TestCaseData(17, 2, false, "0.", false).SetName("Number with dot but without scale"),
				new TestCaseData(17, 2, false, "0.,1", false).SetName("Dot and comma separated numbers"),
				new TestCaseData(17, 2, false, "+-0", false).SetName("Double signed number with mixed signs"),
				new TestCaseData(17, 2, false, "--0", false).SetName("Double negative signed number"),
				new TestCaseData(17, 2, false, "++0", false).SetName("Double positive signed number"),
				new TestCaseData(17, 2, true, "-0", false).SetName("Negative signed number with true onlyPositive parameter")
			};
			foreach (var caseData in data)
				yield return caseData;
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
			numberRegex = new Regex(@"^([+-]?)0*(\d+)([.,](\d+))?$", RegexOptions.IgnoreCase);
		}

		public bool IsValidNumber(string value)
		{
			// Проверяем соответствие входного значения формату N(m,k), в соответствии с правилом, 
			// описанным в Формате описи документов, направляемых в налоговый орган в электронном виде по
			// телекоммуникационным каналам связи: Формат числового значения указывается в виде N(m.к), где m –
			// максимальное количество знаков в числе, включая знак (для отрицательного числа), целую и дробную часть
			// числа без разделяющей десятичной точки, k – максимальное число знаков дробной части числа. Если число
			// знаков дробной части числа равно 0 (т.е. число целое), то формат числового значения имеет вид N(m).

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