using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	[TestFixture]
	public class NumberValidatorTests
	{
		[TestCase(-1)]
		[TestCase(0)]
		public void NumberValidatorConstructor_ThrowsArgumentException_WhenPrecisionIsNotPositive(int precision, int scale = 0, bool onlyPositive = false)
		{
			Action act = () => new NumberValidator(precision, scale, onlyPositive);
			act.Should().Throw<ArgumentException>();
		}

		[Test]
		public void NumberValidatorConstructor_ThrowsArgumentException_WhenScaleIsBiggerThanPrecision()
		{
			Action act = () => new NumberValidator(1, 2);
			act.Should().Throw<ArgumentException>();
		}

		[TestCase("00", 1)]
		public void IsValidNumber_ReturnsFalse_WhenNumberDoNotMatchPrecision(string number, int precision, int scale = 0, bool onlyPositive = false)
		{
			var numberValidator = new NumberValidator(precision, scale, onlyPositive);
			numberValidator.IsValidNumber(number).Should().BeFalse();
		}

		[TestCase("0", 1)]
		[TestCase("00", 2)]
		public void IsValidNumber_ReturnsTrue_WhenNumberMatchPrecision(string number, int precision, int scale = 0, bool onlyPositive = false)
		{
			var numberValidator = new NumberValidator(precision, scale, onlyPositive);
			numberValidator.IsValidNumber(number).Should().BeTrue();
		}

		[TestCase("-1.23", false, 3, 2)]
		[TestCase("-1", false, 1)]
		[TestCase("-10.68", true, 5, 2)]
		public void IsValidNumber_ConsiderMinus_WhenNumberIsNegative(string number, bool expected, int precision, int scale = 0, bool onlyPositive = false)
		{
			var numberValidator = new NumberValidator(precision, scale, onlyPositive);
			numberValidator.IsValidNumber(number).Should().Be(expected);
		}

		[TestCase("+1.23", 3, 2)]
		[TestCase("+1", 1)]
		public void IsValidNumber_DoNotConsiderPlus_WhenNumberIsPositive(string number, int precision, int scale = 0, bool onlyPositive = false)
		{
			var numberValidator = new NumberValidator(precision, scale, onlyPositive);
			numberValidator.IsValidNumber(number).Should().BeTrue();
		}

		[TestCase("-1.23", 4, 2, true)]
		[TestCase("-1", 2, 0, true)]
		[TestCase("-10.68", 5, 2, true)]
		public void IsValidNumber_ReturnsFalse_WhenNumberIsNegativeWithTrueOnlyPositive(string number, int precision, int scale = 0, bool onlyPositive = false)
		{
			var numberValidator = new NumberValidator(precision, scale, onlyPositive);
			numberValidator.IsValidNumber(number).Should().BeFalse();
		}

		[TestCase("a.sd", 3, 2)]
		[TestCase("", 1)]
		[TestCase(null, 1)]
		[TestCase("4a", 3)]
		[TestCase("4.a", 3)]
		public void IsValidNumber_ReturnsFalse_WhenNumberIsIncorrect(string number, int precision, int scale = 0, bool onlyPositive = false)
		{
			var numberValidator = new NumberValidator(precision, scale, onlyPositive);
			numberValidator.IsValidNumber(number).Should().BeFalse();
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
			if (scale < 0 || scale > precision)
				throw new ArgumentException("scale must be a non-negative number less or equal than precision");
			numberRegex = new Regex(@"^(?<sign>[+-]?)(?<intPart>\d+)([.,](?<fracPart>\d+))?$", RegexOptions.IgnoreCase);
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
			var intPart = match.Groups["sign"].Value == "+"
				? match.Groups["intPart"].Value.Length
				: match.Groups["sign"].Value.Length + match.Groups["intPart"].Value.Length;
			// Дробная часть
			var fracPart = match.Groups["fracPart"].Value.Length;

			if (intPart + fracPart > precision || fracPart > scale)
				return false;

			if (onlyPositive && match.Groups["sign"].Value == "-")
				return false;
			return true;
		}
	}
}