using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{

		private NumberValidator numberValidator;

		[SetUp]
		public void SetUp()
		{
			numberValidator = new NumberValidator(4, 2, true);
		}

		[Category("InvalidInput")]
		[TestCase("12.345", TestName = "False_WhenNumberLongerThenPrecision")]
		[TestCase("1.234",  TestName = "False_WhenFractionalPartNumberLongerThanScale")]
		[TestCase("a.b",    TestName = "False_WhenNumberHaveNotDigitOrSignChar")]
		[TestCase(".1",     TestName = "False_WhenStartWithDot")]
		[TestCase("1.", TestName = "False_WhenEndWithDot")]
		[TestCase("-1.2", TestName = "False_WhenNumberValidatorOnlyForPositiveNumberButTakeNegative")]
		[TestCase("--1.2", TestName = "False_WhenHaveTwoSign")]
		[TestCase("-", TestName = "False_WhenHaveOnlySign")]
		[TestCase(null, TestName = "False_WhenStringNull")]
		[TestCase("", TestName = "False_WhenStringEmpty")]
		[TestCase("1..2", TestName = "False_WhenHaveOneMoreDot")]
		public void False_WhenValue(string value)
		{
			numberValidator.IsValidNumber(value).Should().Be(false);
		}

		[Category("ValidNumber")]
		[TestCase("1", TestName = "True_WhenNumberHaveCorrectFormatWithoutFractalPart")]
		[TestCase("1.23", TestName = "True_WhenNumberHaveCorrectFormatWithFractalPart")]
		[TestCase("+1.23", TestName = "True_WhenNumberWithSign")]
		[TestCase("+1,23", TestName = "True_WhenComma")]
		public void True_WhenValue(string value)
		{
			numberValidator.IsValidNumber(value).Should().Be(true);
		}

		[Test]
		[Category("ValidNumber")]
		public void True_WhenNumberValidatorNotOnlyForPositiveNumberButTakeNegative()
		{
			var numberValidatorWithNegativeNumber = new NumberValidator(4, 2, false);
			numberValidatorWithNegativeNumber.IsValidNumber("-1.23").Should().Be(true);
		}

		[Category("InvalidInitialization")]
		[TestCase(2, 3, TestName = "ThrowException_WhenScaleMorePrecision")]
		[TestCase(-1, TestName = "ThrowException_WhenScalePrecisionLessZero")]
		[TestCase(0, TestName = "ThrowException_PrecisionEqualZero")]
		[TestCase(3, -1, TestName = "ThrowException_ScaleLessZero")]
		[TestCase(3, 3, TestName = "ThrowException_ScaleEqualPrecision")]
		public void ThrowException_WhenSomething(int precision, int scale = 0, bool onlyPositive = false)
		{
			Action init = (() => new NumberValidator(precision, scale, onlyPositive));
			init.Should().Throw<ArgumentException>();
		}

		[Test]
		public void DoesNotThrowException_WhenScaleZero()
		{
			Action init = (() => new NumberValidator(1, 0, false));
			init.Should().NotThrow();
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