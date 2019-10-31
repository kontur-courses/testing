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
		[TestCase(-1, 2, TestName = "PrecisionIsNegative")]
		[TestCase(1, -2, TestName = "ScaleIsNegative")]
		[TestCase(1, 2, TestName = "PrecisionLessThanScale")]
		public void СonstructorExeptions(int precision, int scale)
		{
			Action action = () => new NumberValidator(precision, scale);
			action.Should().Throw<ArgumentException>();
		}


		[Category("Invalid strings")]
		[TestCase("", ExpectedResult = false, TestName = "EmptyString")]
		[TestCase(null, ExpectedResult = false, TestName = "InputNull")]
		[TestCase("aaa", ExpectedResult = false, TestName = "StringWithLetters")]
		[TestCase("a.aa", ExpectedResult = false, TestName = "StringWithLettersAndDot")]
		[TestCase("a,aa", ExpectedResult = false, TestName = "StringWithLettersAndComma")]
		[TestCase("!@#$%^&*()\"№;:?\\/}{[]", ExpectedResult = false, TestName = "DiffrentSymbols")]
		[TestCase("   ", ExpectedResult = false, TestName = "WhiteSpaceString")]
		[TestCase("123,abc", ExpectedResult = false, TestName = "NumbersAndLettersInOneString")]
		public bool IncorrrectStrings(string input)
		{
			var numberValidator = new NumberValidator(30);
			return numberValidator.IsValidNumber(input);
		}

		[Test]
		[Category("Invalid strings")]
		public void FractionalPartMoreThanScale()
		{
			var numberValidator = new NumberValidator(4, 2);
			numberValidator.IsValidNumber("0.000").Should().BeFalse();
		}

		[Test]
		[Category("Invalid strings")]
		public void NegativeNumber_When_NumberValidatorIsOnlyPositive()
		{
			var numberValidator = new NumberValidator(5, 1, true);
			numberValidator.IsValidNumber("1").Should().BeTrue();
			numberValidator.IsValidNumber("-1").Should().BeFalse();
		}

		[Category("Valid strings")]
		[TestCase("-12.00", ExpectedResult = true, TestName = "StringWithMinusSymbol")]
		[TestCase("+12.00", ExpectedResult = true, TestName = "StringWithPlusSymbol")]
		[TestCase("12.00", ExpectedResult = true, TestName = "DotIsSeparator")]
		[TestCase("12,00", ExpectedResult = true, TestName = "CommaIsSeparator")]
		[TestCase("12", ExpectedResult = true, TestName = "IntegerNumber")]
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
		public void PlusSymbolsIncludeInPrecisionValue()
		{
			var numberValidator = new NumberValidator(4, 2);
			numberValidator.IsValidNumber("+1.23").Should().BeTrue();
			numberValidator.IsValidNumber("+12.23").Should().BeFalse();
		}

		[Test]
		[Category("Extreme case")]
		public void MinusSymbolsIncludeInPrecisionValue()
		{
			var numberValidator = new NumberValidator(4, 2, false);
			numberValidator.IsValidNumber("-1.23").Should().BeTrue();
			numberValidator.IsValidNumber("-12.23").Should().BeFalse();
		}

		[Test]
		[Category("Extreme case")]
		public void NumberLengthEqualPrecision()
		{
			var numberValidator = new NumberValidator(4);
			numberValidator.IsValidNumber("1234").Should().BeTrue();
		}

		[Test]
		[Category("Сonstructor NumberValidator")]
		public void ScaleDefaultValueIsZero()
		{
			var numberValidator = new NumberValidator(5);
			numberValidator.IsValidNumber("1").Should().BeTrue();
			numberValidator.IsValidNumber("1.0").Should().BeFalse();
		}

		[Test]
		[Category("Сonstructor NumberValidator")]
		public void DefaultNumberValidatorIsNotOnlyPositive()
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