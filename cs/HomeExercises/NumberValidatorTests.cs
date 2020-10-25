using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		private NumberValidator defaultValidator;
		private NumberValidator smallPrecValidator;
		private NumberValidator onlyPositiveValidator;
			
		[SetUp]
		public void Init()
		{
			defaultValidator = new NumberValidator(17, 2);
			smallPrecValidator = new NumberValidator(3, 2);
			onlyPositiveValidator = new NumberValidator(17, 2, true);
		}
		
		[Test]
		public void TestNotValidWhenNotADigit()
		{
			defaultValidator.IsValidNumber("as.d")
				.Should().BeFalse();
		}
		
		[Test]
		[TestCase(null)]
		[TestCase("")]
		public void TestNotValidWhenNullOrWhitespace(string number)
		{
			defaultValidator.IsValidNumber(number)
				.Should().BeFalse();
		}
		
		[Test]
		public void TestValidDifferentDigits()
		{
			defaultValidator.IsValidNumber("1234567890")
				.Should().BeTrue();
		}
		
		[Test]
		[TestCase("0.000")]
		[TestCase("00000")]
		[TestCase("000.00")]
		[TestCase("+0000")]
		[TestCase("+00.00")]
		public void TestNotValidWhenOutOfBond(string number)
		{
			smallPrecValidator.IsValidNumber(number)
				.Should().BeFalse();
		}

		[Test]
		public void TestNotValidWhenNegativeNumberOnlyPosValidator()
		{
			onlyPositiveValidator.IsValidNumber("-0.0")
				.Should().BeFalse();
		}
		
		[Test]
		[TestCase("0.0")]
		[TestCase("+0.0")]
		public void TestValidWhenPosNumberOnlyPosValidator(string number)
		{
			onlyPositiveValidator.IsValidNumber(number)
				.Should().BeTrue();
		}
		
		[Test]
		[TestCase("0.0")]
		[TestCase("+0.0")]
		[TestCase("-0.0")]
		public void TestValidAnySignWhenNonOnlyPos(string number)
		{
			defaultValidator.IsValidNumber(number)
				.Should().BeTrue();
		}

		[Test]
		[TestCase('.')]
		[TestCase(',')]
		public void TestAllowedDecimalSeparator(char separator)
		{
			defaultValidator.IsValidNumber($"0{separator}0")
				.Should().BeTrue();
		}
		
		[Test]
		[TestCase('!')]
		[TestCase(':')]
		public void TestNotAllowedDecimalSeparator(char separator)
		{
			defaultValidator.IsValidNumber($"0{separator}0")
				.Should().BeFalse();
		}

		[Test]
		[TestCase(-1, 0, 
			"precision must be a positive number")]
		[TestCase(10, -1, 
			"scale must be a non-negative number less or equal than precision")]
		[TestCase(10, 11,
			"scale must be a non-negative number less or equal than precision")]
		public void TestExceptionOnIncorrectCreation(int precision, int scale, string message)
		{
			Action makeIncorrectValidator = () =>
				new NumberValidator(precision, scale);
			makeIncorrectValidator.Should()
				.Throw<ArgumentException>()
				.WithMessage(message);
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
				//Тут была ошибка в сообщении: "precision must be..."
				throw new ArgumentException("scale must be a non-negative number less or equal than precision");
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