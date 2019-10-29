using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace HomeExercises
{
	[TestFixture]
	public class NumberValidator_Initialization_Should
	{

		[TestCase(2, 3, TestName = "ThrowException_WhenScaleMorePrecision")]
		[TestCase(-1, TestName = "ThrowException_WhenPrecisionLessZero")]
		[TestCase(0, TestName = "ThrowException_PrecisionEqualZero")]
		[TestCase(3, -1, TestName = "ThrowException_ScaleLessZero")]
		[TestCase(3, 3, TestName = "ThrowException_ScaleEqualPrecision")]
		public void ThrowException_WhenSomething(int precision, int scale = 0, bool onlyPositive = false)
		{
			Action init = (() => new NumberValidator(precision, scale, onlyPositive));
			init.Should().Throw<ArgumentException>();
		}

		[Category("ValidInitialization")]
		[TestCase(1, 0, TestName = "DoesNotThrowException_WhenScaleZero")]
		public void DoesNotThrowException_WhenSomething(int precision, int scale = 0, bool onlyPositive = false)
		{
			Action init = (() => new NumberValidator(1, 0, false));
			init.Should().NotThrow();
		}
	}

	[TestFixture]
	public class OnlyPositive_NumberValidator_Should
	{
		private NumberValidator numberValidator;

		[SetUp]
		public void SetUp()
		{
			numberValidator = new NumberValidator(4, 2, true);
		}

		[TestCase("+1.23", TestName = "ValidNumber_WhenNumberWithPlus")]
		public void True_WhenValue(string value)
		{
			numberValidator.IsValidNumber(value).Should().Be(true);
		}

		[TestCase("++1.2", TestName = "InvalidNumber_WhenHaveTwoPlus")]
		[TestCase("+", TestName = "InvalidNumber_WhenHaveOnlyPlus")]
		[TestCase("-1", TestName = "InvalidNumber_WhenNegativeNumber")]
		public void False_WhenValue(string value)
		{
			numberValidator.IsValidNumber(value).Should().Be(false);
		}
	}

	[TestFixture]
	public class NumberValidator_NegativeNumber_Should
	{
		private NumberValidator numberValidator;

		[SetUp]
		public void SetUp()
		{
			numberValidator = new NumberValidator(4, 2, false);
		}

		[TestCase("-1,23", TestName = "ValidNumber_WhenNegativeNumber")]
		[TestCase("-1.23", TestName = "ValidNumber_WhenNegativeNumberWithDot")]
		[TestCase("-1", TestName = "ValidNumber_WhenNegativeNumberWithoutFractalPart")]
		public void True_WhenValue(string value)
		{
			numberValidator = new NumberValidator(4, 2, false);
			numberValidator.IsValidNumber(value).Should().Be(true);
		}

		[TestCase("--1.2", TestName = "InvalidNumber_WhenHaveTwoMinus")]
		[TestCase("-", TestName = "InvalidNumber_WhenHaveOnlyMinus")]
		public void False_WhenValue(string value)
		{
			numberValidator.IsValidNumber(value).Should().Be(false);
		}
	}

	[TestFixture]
	public class NumberValidator_GeneralCases_Should
	{
		private NumberValidator numberValidator;

		[SetUp]
		public void SetUp()
		{
			numberValidator = new NumberValidator(4, 2, false);
		}

		[TestCase("1", TestName = "ValidNumber_WhenNumberHaveCorrectFormatWithoutFractalPart")]
		[TestCase("1.23", TestName = "ValidNumber_WhenNumberHaveCorrectFormatWithFractalPart")]
		public void True_WhenValue(string value)
		{
			numberValidator.IsValidNumber(value).Should().Be(true);
		}

		[TestCase("12.345", TestName = "InvalidNumber_WhenNumberLongerThanPrecision")]
		[TestCase("1.234", TestName = "InvalidNumber_WhenFractionalPartNumberLongerThanScale")]
		[TestCase("a.b", TestName = "InvalidNumber_WhenNumberHaveNotDigitOrSignChar")]
		[TestCase(".1", TestName = "InvalidNumber_WhenStartWithDot")]
		[TestCase("1.", TestName = "InvalidNumber_WhenEndWithDot")]
		[TestCase(null, TestName = "InvalidNumber_WhenStringNull")]
		[TestCase("", TestName = "InvalidNumber_WhenStringEmpty")]
		[TestCase("1..2", TestName = "InvalidNumber_WhenHaveMoreThanOneDot")]
		[TestCase("1.,2", TestName = "InvalidNumber_WhenHaveCommaAndDot")]
		public void False_WhenValue(string value)
		{
			numberValidator.IsValidNumber(value).Should().Be(false);
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