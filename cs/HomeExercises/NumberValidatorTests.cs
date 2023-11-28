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
		public static TestCaseData[] unsignedPositiveNumberTestCases =
		{
			new TestCaseData(1, 0, "0").SetName("unsigned zero"),
			new TestCaseData(2, 1, "0.0").SetName("unsigned zero with 1 digit after"),
			new TestCaseData(1, 0, "1").SetName("unsigned 1 digit number"),
			new TestCaseData(2, 0, "12").SetName("unsigned 2 digit number"),
			new TestCaseData(2, 1, "1.2").SetName("unsigned 2 digit number with 1 digit after point"),
			new TestCaseData(3, 2, "1.23").SetName("unsigned 3 digit number with 2 digit after point"),
			new TestCaseData(3, 1, "12.3").SetName("unsigned 3 digit number with 1 digit after point"),
			new TestCaseData(4, 2, "12.34").SetName("unsigned 4 digit number with 2 digit after point"),
		};

		public static TestCaseData[] signedPositiveNumberTestCases =
		{
			new TestCaseData(2, 0, "+0").SetName("positive zero"),
			new TestCaseData(3, 1, "+0.0").SetName("positive zero with 1 digit after"),
			new TestCaseData(2, 0, "+1").SetName("positive 1 digit number"),
			new TestCaseData(3, 0, "+12").SetName("positive 2 digit number"),
			new TestCaseData(3, 1, "+1.2").SetName("positive 2 digit number with 1 digit after point"),
			new TestCaseData(4, 2, "+1.23").SetName("positive 3 digit number with 2 digit after point"),
			new TestCaseData(4, 1, "+12.3").SetName("positive 3 digit number with 1 digit after point"),
			new TestCaseData(5, 2, "+12.34").SetName("positive 4 digit number with 2 digit after point"),
		};

		public static TestCaseData[] negativeNumberTestCases =
		{
			new TestCaseData(2, 0, "-0").SetName("negative zero"),
			new TestCaseData(3, 1, "-0.0").SetName("negative zero with 1 digit after"),
			new TestCaseData(2, 0, "-1").SetName("negative 1 digit number"),
			new TestCaseData(3, 0, "-12").SetName("negative 2 digit number"),
			new TestCaseData(3, 1, "-1.2").SetName("negative 2 digit number with 1 digit after point"),
			new TestCaseData(4, 2, "-1.23").SetName("negative 3 digit number with 2 digit after point"),
			new TestCaseData(4, 1, "-12.3").SetName("negative 3 digit number with 1 digit after point"),
			new TestCaseData(5, 2, "-12.34").SetName("negative 4 digit number with 2 digit after point"),
		};

		public static TestCaseData[] wrongCases =
		{
			new TestCaseData(1, 0, "-1")
				.SetName("integer number with \"+\" sign must have precision more than digits count").Returns(false),
			new TestCaseData(1, 0, "+1")
				.SetName("integer number with \"-\" sign must have precision more than digits count").Returns(false),
			new TestCaseData(2, 1, "+1.2")
				.SetName("number with fractional part with \"+\" sign must have precision more than digits count")
				.Returns(false),
			new TestCaseData(2, 1, "-1.2")
				.SetName("number with fractional part with \"-\" sign must have precision more than digits count")
				.Returns(false),
		};

		[TestCaseSource(nameof(unsignedPositiveNumberTestCases))]
		[TestCaseSource(nameof(signedPositiveNumberTestCases))]
		[TestCaseSource(nameof(negativeNumberTestCases))]
		public void IsValidNumber(int precision, int scale, string validatingString)
		{
			new NumberValidator(precision, scale)
				.IsValidNumber(validatingString)
				.Should()
				.BeTrue();
		}

		[TestCaseSource(nameof(wrongCases))]
		public bool IsNotValidNumber(int precision, int scale, string validatingString)
		{
			return new NumberValidator(precision, scale).IsValidNumber(validatingString);
		}

		[TestCaseSource(nameof(negativeNumberTestCases))]
		public void ShouldFailWhenNumberNegativeWithOnlyPositive(int precision, int scale, string validatingString)
		{
			new NumberValidator(precision, scale, true)
				.IsValidNumber(validatingString)
				.Should()
				.BeFalse();
		}

		[TestCase(3, 2, "a.sd", TestName = "non digit symbols")]
		[TestCase(2, 1, ".0", TestName = "must have digits before point")]
		[TestCase(1, 0, "0.", TestName = "must have digits after point (if exist)")]
		public void WrongFormat(int precision, int scale, string validatingString, bool onlyPositive = true)
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
			Assert.Throws<ArgumentException>(() => new NumberValidator(precision, scale, onlyPositive));
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