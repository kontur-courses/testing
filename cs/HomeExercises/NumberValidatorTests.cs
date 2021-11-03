using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[TestCase(-1, 0, TestName = "Non positive precision")]
		[TestCase(1, -1, TestName = "Negative scale")]
		[TestCase(1, 2, TestName = "Scale bigger than precision")]
		[TestCase(1, 1, TestName = "Scale same as precision")]
		public void Constructor_ThrowsArgumentException(int precision, int scale)
		{
			Action create = () => { new NumberValidator(precision, scale, true); };
			create.Should().Throw<ArgumentException>();
		}
		
		[TestCaseSource(nameof(ShouldBeFalseTestData))]
		[TestCaseSource(nameof(ShouldBeTrueTestData))]
		public void Constructor_DoesNotThrowsException(int precision, int scale, string value)
		{
			Action create = () => { new NumberValidator(precision, scale, true); };
			create.Should().NotThrow();
		}
		
		[TestCase(null)]
		[TestCase("")]
		[TestCase("HelloWorld")]
		[TestCase("Hello.World")]
		[TestCase("-")]
		[TestCase("+")]
		[TestCase("+-1")]
		[TestCase("IV")]
		[TestCase("2D", TestName = "Letter in number")]
		[TestCase("0.00.1", TestName = "Many dots")]
		[TestCase("10 0", TestName = "Space in number")]
		[TestCase(".01", TestName = "No integer part")]
		[TestCase( "-.01", TestName = "Sign and no integer part")]
		[TestCase( "1.", TestName = "Dot without fraction")]
		public void IsValidNumber_False_WithIncorrectValue(string value)
		{
			var numberValidator = new NumberValidator(100, 50);
			numberValidator.IsValidNumber(value).Should().BeFalse();
		}

		[TestCase("-1")]
		public void IsValidNumber_OnlyPositiveFlag_FalseWithNegativeValue(string value)
		{
			var numberValidator = new NumberValidator(10, 2, true);
			numberValidator.IsValidNumber(value).Should().BeFalse();
		}
		
		[TestCase("1")]
		[TestCase("0")]
		[TestCase("+1")]
		public void IsValidNumber_OnlyPositiveFlag_TrueWithPositiveValue(string value)
		{
			var numberValidator = new NumberValidator(10, 2, true);
			numberValidator.IsValidNumber(value).Should().BeTrue();
		}
		
		[TestCaseSource(nameof(ShouldBeTrueTestData))]
		public void IsValidNumber_ShouldBeTrue(int precision, int scale, string value)
		{
			var numberValidator = new NumberValidator(precision, scale);
			numberValidator.IsValidNumber(value).Should().BeTrue();
		}
		
		[TestCaseSource(nameof(ShouldBeFalseTestData))]
		public void IsValidNumber_ShouldBeFalse(int precision, int scale, string value)
		{
			var numberValidator = new NumberValidator(precision, scale);
			numberValidator.IsValidNumber(value).Should().BeFalse();
		}

		private static IEnumerable<TestCaseData> ShouldBeTrueTestData()
		{
			yield return new TestCaseData(17, 2, "0");
			yield return new TestCaseData(17, 2, "0.00");
			yield return new TestCaseData(17, 2, "0,0") { TestName = "WorksWithComma" };
			yield return new TestCaseData(4, 2, "+1.23");
			yield return new TestCaseData(4, 2, "-1.23");
		}

		private static IEnumerable<TestCaseData> ShouldBeFalseTestData()
		{
			yield return new TestCaseData(17, 2, "0.000") { TestName = "Fractional part longer than scale" };
			yield return new TestCaseData(3, 2, "+0.00") { TestName = "Sign counts in precision" };
			yield return new TestCaseData(3, 2, "00.00") { TestName = "Leading zero counts" };
			yield return new TestCaseData(3, 2, "11100") { TestName = "Too long number" };
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