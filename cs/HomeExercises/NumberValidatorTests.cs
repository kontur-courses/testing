using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	[TestFixture]
	public class NumberValidatorTests
	{
		[Test]
		[Category("Exeptions")]
		[Category("Сonstructor NumberValidator")]
		[TestCase(-1, 2, TestName = "NumberValidatorConstructor_ThrowArgumentException_WhenPrecisionIsNegative")]
		[TestCase(1, -2, TestName = "NumberValidatorConstructor_ThrowArgumentException_WhenScaleIsNegative")]
		[TestCase(1, 2, TestName = "NumberValidatorConstructor_ThrowArgumentException_WhenPrecisionLessThanScale")]
		[TestCase(0, 1, TestName = "NumberValidatorConstructor_ThrowArgumentException_WhenPrecisionEqualZero")]
		[TestCase(2, 2, TestName = "NumberValidatorConstructor_ThrowArgumentException_WhenPrecisionEqualScale")]
		public void СonstructorExeptions(int precision, int scale)
		{
			Action action = () => new NumberValidator(precision, scale);
			action.Should().Throw<ArgumentException>();
		}


		[Category("Invalid strings")]
		[TestCase("", ExpectedResult = false, TestName = "IsValidNumber_MustBeFalse_WhenAcceptsEmptyString")]
		[TestCase(null, ExpectedResult = false, TestName = "IsValidNumber_MustBeFalse_WhenAcceptsNull")]
		[TestCase("aaa", ExpectedResult = false, TestName = "IsValidNumber_MustBeFalse_WhenAcceptsStringWithLetters")]
		[TestCase("a.aa", ExpectedResult = false, TestName = "IsValidNumber_MustBeFalse_WhenAcceptsStringWithLettersAndDot")]
		[TestCase("a,aa", ExpectedResult = false, TestName = "IsValidNumber_MustBeFalse_WhenAcceptsStringWithLettersAndComma")]
		[TestCase("!@#$%^&*()\"№;:?\\/}{[]", ExpectedResult = false, TestName = "IsValidNumber_MustBeFalse_WhenAcceptsStringWithDiffrentSymbols")]
		[TestCase("   ", ExpectedResult = false, TestName = "IsValidNumber_MustBeFalse_WhenAcceptsWhiteSpaceString")]
		[TestCase("123,abc", ExpectedResult = false, TestName = "IsValidNumber_MustBeFalse_WhenAcceptsNumbersAndLettersInOneString")]
		[TestCase("1234567890123456789012345678", ExpectedResult = false, TestName = "IsValidNumber_MustBeFalse_WhenNumberLengthGreaterPrecision")]
		public bool IncorrrectStrings(string input)
		{
			var numberValidator = new NumberValidator(15);
			return numberValidator.IsValidNumber(input);
		}

		[Test]
		[Category("Invalid strings")]
		public void IsValidNumber_MustBeFalse_WhenNumberFractionalPartMoreThanScale()
		{
			var numberValidator = new NumberValidator(4, 2);
			numberValidator.IsValidNumber("0.000").Should().BeFalse();
		}

		[Test]
		[Category("Invalid strings")]
		public void IsValidNumber_MustBeFalse_WhenAcceptsNegativeNumber_When_NumberValidatorIsOnlyPositive()
		{
			var numberValidator = new NumberValidator(5, 1, true);
			numberValidator.IsValidNumber("1").Should().BeTrue();
			numberValidator.IsValidNumber("-1").Should().BeFalse();
		}

		[Category("Valid strings")]
		[TestCase("-12.00", ExpectedResult = true, TestName = "IsValidNumber_MustBeTrue_WhenAcceptsNumberWithMinusSymbol")]
		[TestCase("+12.00", ExpectedResult = true, TestName = "IsValidNumber_MustBeTrue_WhenAcceptsNumberWithPlusSymbol")]
		[TestCase("12.00", ExpectedResult = true, TestName = "IsValidNumber_MustBeTrue_WhenAcceptsNumber_When_DotIsSeparator")]
		[TestCase("12,00", ExpectedResult = true, TestName = "IsValidNumber_MustBeTrue_WhenAcceptsNumber_When_CommaIsSeparator")]
		[TestCase("12", ExpectedResult = true, TestName = "IsValidNumber_MustBeTrue_WhenAcceptsIntegerNumber")]
		public bool CorrrectStrings(string input)
		{
			var numberValidator = new NumberValidator(6, 3, false);
			return numberValidator.IsValidNumber(input);
		}

		[Test]
		[Category("Valid strings")]
		public void IsValidNumber_MustBeTrue_WhenNumberFractionalPartLessThanScale()
		{
			var numberValidator = new NumberValidator(4, 2);
			numberValidator.IsValidNumber("0.0").Should().BeTrue();
		}

		[Test]
		[Category("Extreme case")]
		public void IsValidNumber_MustBeInclude_PlusSymbolsInPrecisionValue()
		{
			var numberValidator = new NumberValidator(4, 2);
			numberValidator.IsValidNumber("+1.23").Should().BeTrue();
			numberValidator.IsValidNumber("+12.23").Should().BeFalse();
		}

		[Test]
		[Category("Extreme case")]
		public void IsValidNumber_MustBeInclude_MinusSymbolsInPrecisionValue()
		{
			var numberValidator = new NumberValidator(4, 2, false);
			numberValidator.IsValidNumber("-1.23").Should().BeTrue();
			numberValidator.IsValidNumber("-12.23").Should().BeFalse();
		}

		[Test]
		[Category("Extreme case")]
		public void IsValidNumber_MustBeTrue_WhenNumberLengthEqualPrecision()
		{
			var numberValidator = new NumberValidator(4);
			numberValidator.IsValidNumber("1234").Should().BeTrue();
		}

		[Test]
		[Category("Сonstructor NumberValidator")]
		public void NumberValidatorConstructor_ScaleDefaultValue_MustBeZero()
		{
			var numberValidator = new NumberValidator(5);
			numberValidator.IsValidNumber("1").Should().BeTrue();
			numberValidator.IsValidNumber("1.0").Should().BeFalse();
		}

		[Test]
		[Category("Сonstructor NumberValidator")]
		public void NumberValidatorConstructor_DefaultNumberValidator_MustBeNotOnlyPositive()
		{
			var numberValidator = new NumberValidator(5, 1);
			numberValidator.IsValidNumber("1").Should().BeTrue();
			numberValidator.IsValidNumber("-1").Should().BeTrue();
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
				throw new ArgumentException("scale must be a non-negative number and less than precision");
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