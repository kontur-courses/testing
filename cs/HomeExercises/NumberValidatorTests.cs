using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[TestCase("12+4")]
		[TestCase("O")]
		[TestCase("10X2")]
		[TestCase(".2")]
		[TestCase("a.sd")]
		[TestCase("")]
		[TestCase("  ")]
		[TestCase(null)]
		public void NotNumber_ShouldBeNotValid(string number)
		{
			new NumberValidator(6, 2, true)
				.IsValidNumber(number)
				.Should()
				.BeFalse();
		}

		[TestCase(0)]
		[TestCase(-5)]
		public void Precision_ShouldBePositive(int precision)
			=> Assert.Throws<ArgumentException>(() => new NumberValidator(precision));

		[TestCase(-1)]
		[TestCase(6)]
		[TestCase(7)]
		public void Precision_ShouldBeNotNegative_AndLessThanPrecision(int scale)
			=> Assert.Throws<ArgumentException>(() => new NumberValidator(6, scale));

		[TestCase("-0.12")]
		[TestCase("+0.12")]
		[TestCase("+0.12")]
		[TestCase("12.12")]
		[TestCase("1234")]
		[TestCase("+123")]
		public void SymbolsCount_ShouldBeLessThanPrecision(string number)
		{
			new NumberValidator(3, 2, false)
				.IsValidNumber(number)
				.Should()
				.BeFalse();
		}

		[TestCase("1.123")]
		[TestCase("-1.123")]
		public void FractSymbolsCount_ShouldBeLessThanScale(string number)
		{
			new NumberValidator(5, 2, false)
				.IsValidNumber(number)
				.Should()
				.BeFalse();
		}

		[TestCase("-4")]
		[TestCase("-4.1")]
		public void OnlyPositiveValidator_ShouldNotValidateNegative(string number)
		{
			new NumberValidator(6, 4, true)
				.IsValidNumber(number)
				.Should()
				.BeFalse();
		}

		[TestCase("+1.1234")]
		[TestCase("12.1234")]
		[TestCase("123456")]
		[TestCase("0.000")]
		[TestCase("000.0")]
		public void OnlyPositiveValidator_ShouldValidatePositive(string number)
		{
			new NumberValidator(6, 4, true)
				.IsValidNumber(number)
				.Should()
				.BeTrue();
		}

		[TestCase("+1.1234")]
		[TestCase("12.1234")]
		[TestCase("-1.1234")]
		[TestCase("-12345")]
		[TestCase("-0.000")]
		[TestCase("-000.0")]
		public void NotOnlyPositiveValidator_ShouldValidateAnyNumber(string number)
		{
			new NumberValidator(6, 4, false)
				.IsValidNumber(number)
				.Should()
				.BeTrue();
		}

		[TestCase("+1,1234")]
		[TestCase("12,1234")]
		[TestCase("-1,1234")]
		[TestCase("-0,000")]
		[TestCase("-000,0")]
		public void Validator_ShouldValidateDecimal_WithCommaSeparator(string number)
        {
			new NumberValidator(6, 4, false)
				.IsValidNumber(number)
				.Should()
				.BeTrue();
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
			// описанным в Формате описи документов, направляемых в налоговый орган
			// в электронном виде по телекоммуникационным каналам связи:
			// Формат числового значения указывается в виде N(m.к),
			// где m – максимальное количество знаков в числе, включая знак (для отрицательного числа), 
			// целую и дробную часть числа без разделяющей десятичной точки,
			// k – максимальное число знаков дробной части числа. 
			// Если число знаков дробной части числа равно 0 (т.е. число целое),
			// то формат числового значения имеет вид N(m).

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