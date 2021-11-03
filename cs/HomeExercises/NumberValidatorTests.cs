using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[TestCase(2, 1, true,"", TestName = "Scale less than precision")]
		[TestCase(1, 0, true,"", TestName = "Scale is zero")]
		[TestCaseSource(nameof(IsValidShouldBeFalseTestCaseData))]
		[TestCaseSource(nameof(IsValidShouldBeTrueTestCaseData))]
		public void Constructor_ShouldCreateNumberValidator(int precision, int scale, bool onlyPositive, string _)
		{
			FluentActions.Invoking(
					() => new NumberValidator(precision, scale, onlyPositive))
				.Should().NotThrow();
		}

		[TestCase(-1, 2, true, TestName = "Negative precision")]
		[TestCase(1, -1, true, TestName = "Negative scale")]
		[TestCase(1, 2, true, TestName = "Scale greater than precision")]
		[TestCase(0, 1, true, TestName = "Precision is zero")]
		public void Constructor_ShouldThrowArgumentException(int precision, int scale, bool onlyPositive)
		{
			FluentActions.Invoking(
					() => new NumberValidator(precision, scale, onlyPositive))
				.Should().Throw<ArgumentException>();;
		}

		[TestCaseSource(nameof(IsValidShouldBeFalseTestCaseData))]
		public void IsValid_ShouldBeFalse(int precision ,int scale, bool  onlyPositive, string value)
		{
			var validator = new NumberValidator(precision, scale, onlyPositive);

			var validatorResult = validator.IsValidNumber(value);

			validatorResult.Should().BeFalse();
		}

		[TestCaseSource(nameof(IsValidShouldBeTrueTestCaseData))]
		public void IsValid_ShouldBeTrue(int precision, int scale,  bool onlyPositive, string value)
		{
			var validator = new NumberValidator(precision, scale, onlyPositive);

			var validatorResult = validator.IsValidNumber(value);

			validatorResult.Should().BeTrue();
		}

		private static IEnumerable<TestCaseData> IsValidShouldBeFalseTestCaseData
		{
			get
			{
				yield return new TestCaseData(1, 0, true, "").SetName("Value is empty string");
				yield return new TestCaseData(2, 1, true, " ").SetName("Value is white space");
				yield return new TestCaseData(3, 2, true, null).SetName("Value is null");
				yield return new TestCaseData(4, 3, true, "a.sd").SetName("Value is not a number");
				yield return new TestCaseData(4, 1, true, "-0.0").SetName("NumberValidator is onlyPositive but value is negative");
				yield return new TestCaseData(3, 2, true, "0.000").SetName("Fractional part grater then scale");
				yield return new TestCaseData(3, 2, true, ".0").SetName("Value without int part");
				yield return new TestCaseData(3, 2, true, "0.").SetName("Value without fractional part but with separator");
				yield return new TestCaseData(5, 2, true, "+000.00").SetName("Int part plus fractional part greater than precision");
			}
		}
		
		private static IEnumerable<TestCaseData> IsValidShouldBeTrueTestCaseData
		{
			get
			{
				yield return new TestCaseData(3, 2, true, "0,0").SetName("Separator is comma");
				yield return new TestCaseData(3, 2, true, "+0.0").SetName("Value is positive");
				yield return new TestCaseData(3, 2, false, "-0.0").SetName("Value is negative");
			}
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