using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	[TestFixture]
	public class NumberValidatorTests
	{
		[TestCase("-42.42")]
		[TestCase("-42,42")]
		[TestCase("-42")]
		public void IsValidNumber_WithNegativeNumberWhenOnlyPositiveIsTrue_ShouldReturnFalse(string number)
		{
			var numberValidator = new NumberValidator(3, 2, true);
			var actual = numberValidator.IsValidNumber(number);
			actual.Should().BeFalse();
		}

		[TestCase("")]
		[TestCase(null)]
		public void IsValidNumber_NumberIsNullOrEmpty_ShouldReturnFalse (string number)
		{
			var numberValidator = new NumberValidator(4, 2, true);
			var actual = numberValidator.IsValidNumber(number);
			actual.Should().BeFalse();
		}

		#region FractionalPartIsNotNumber

		[TestCase(17, 2, true, "42.a")]
		[TestCase(17, 2, true, "+42.a")]
		[TestCase(17, 2, false, "-42.a")]
		[TestCase(17, 2, true, "42,a")]
		[TestCase(17, 2, true, "+42,a")]
		[TestCase(17, 2, false, "-42,a")]
		public void IsValidNumber_FractionalPartOfNumberIsNotNumber_ShouldReturnFalse(
			int precision, int scale, bool onlyPositive, string number)
		{
			var numberValidator = new NumberValidator(precision, scale, onlyPositive);
			var actual = numberValidator.IsValidNumber(number);
			actual.Should().BeFalse();
		}

		#endregion

		#region IntegerPartIsNotNumber

		[TestCase(3, 2, true, "a")]
		[TestCase(3, 2, true, "+a")]
		[TestCase(3, 2, false, "-a")]
		[TestCase(3, 2, false, "-")]
		[TestCase(3, 2, true, "+")]
		[TestCase(3, 2, true, "a.42")]
		[TestCase(3, 2, true, "+a.42")]
		[TestCase(3, 2, false, "-a.42")]
		[TestCase(3, 2, true, "a,42")]
		[TestCase(3, 2, true, "+a,42")]
		[TestCase(3, 2, false, "-a,42")]
		public void IsValidNumber_InvalidIntegerPartOfNumber_ShouldReturnFalse(
			int precision, int scale, bool onlyPositive, string number)
		{
			var numberValidator = new NumberValidator(precision, scale, onlyPositive);
			var actual = numberValidator.IsValidNumber(number);
			actual.Should().BeFalse();
		}

		#endregion

		#region NumberLongerThanPossible

		[TestCase(1, 0, true, "42")]
		[TestCase(2, 0, true, "+42")]
		[TestCase(2, 0, false, "-42")]
		[TestCase(3, 2, true, "42.42")]
		[TestCase(3, 2, true, "+1.23")]
		[TestCase(3, 2, false, "-1.23")]
		[TestCase(17, 2, true, "0.000")]
		[TestCase(3, 2, true, "42,42")]
		[TestCase(3, 2, true, "+1,23")]
		[TestCase(3, 2, false, "-1,23")]
		[TestCase(17, 2, true, "0,000")]
		public void IsValidNumber_NumberLongerThanPossible_ShouldReturnFalse(
			int precision, int scale, bool onlyPositive, string number)
		{
			var numberValidator = new NumberValidator(precision, scale, onlyPositive);
			var actual = numberValidator.IsValidNumber(number);
			actual.Should().BeFalse();
		}

		#endregion

		#region ValidNumber

		[TestCase(17, 2, true, "42")]
		[TestCase(17, 2, false, "-42")]
		[TestCase(17, 2, true, "+42")]
		[TestCase(4, 2, true, "+1.23")]
		[TestCase(4, 2, false, "-1.23")]
		[TestCase(4, 2, true, "42.42")]
		[TestCase(4, 2, true, "+1,23")]
		[TestCase(4, 2, false, "-1,23")]
		[TestCase(4, 2, true, "42,42")]
		public void IsValidNumber_ArgumentIsValidNumber_ShouldReturnTrue(
			int precision, int scale, bool onlyPositive, string number)
		{
			var numberValidator = new NumberValidator(precision, scale, onlyPositive);
			var actual = numberValidator.IsValidNumber(number);
			actual.Should().BeTrue();
		}

		#endregion

		#region NumberIsSubstring

		[TestCase("42.42lemon")]
		[TestCase("lemon42.42")]
		[TestCase("le42.42mon")]
		[TestCase("42le.mon42")]
		[TestCase("42le.42")]
		[TestCase("42.mon42")]
		[TestCase("42,42lemon")]
		[TestCase("lemon42,42")]
		[TestCase("le42,42mon")]
		[TestCase("42le,mon42")]
		[TestCase("42le,42")]
		[TestCase("42,mon42")]
		[TestCase("42lemon")]
		[TestCase("lemon42")]
		[TestCase("le42mon")]
		[TestCase("4lemon2")]
		public void IsValidNumber_NumberIsSubstring_ShouldReturnFalse(string number)
		{
			var numberValidator = new NumberValidator(4, 2, true);
			var actual = numberValidator.IsValidNumber(number);
			actual.Should().BeFalse();
		}
		
		#endregion

		#region NumberWithSeparatorWithoutPartOfNumber

		[TestCase("42.")]
		[TestCase("42,")]
		[TestCase("-42.")]
		[TestCase("-42,")]
		[TestCase("+42.")]
		[TestCase("+42,")]
		public void IsValidNumber_NumberWithSeparatorWithoutFractionalPart_ShouldReturnFalse(string number)
		{
			var numberValidator = new NumberValidator(4, 2, false);
			var actual = numberValidator.IsValidNumber(number);
			actual.Should().BeFalse();
		}

		[TestCase(".42")]
		[TestCase(",42")]
		[TestCase("-.42")]
		[TestCase("-,42")]
		[TestCase("+.42")]
		[TestCase("+,42")]
		public void IsValidNumber_NumberWithSeparatorWithoutIntegerPart_ShouldReturnFalse(string number)
		{
			var numberValidator = new NumberValidator(4, 2, false);
			var actual = numberValidator.IsValidNumber(number);
			actual.Should().BeFalse();
		}

		[TestCase(".")]
		[TestCase(",")]
		[TestCase("-.")]
		[TestCase("-,")]
		[TestCase("+.")]
		[TestCase("+,")]
		public void IsValidNumber_NumberWithSeparatorWithoutIntegerAndFractionParts_ShouldReturnFalse(string number)
		{
			var numberValidator = new NumberValidator(4, 2, false);
			var actual = numberValidator.IsValidNumber(number);
			actual.Should().BeFalse();
		}

		#endregion


		[TestCase(-1, 2, true, Description = "argument precision is negative")]
		[TestCase(1, -2, false, Description = "argument scale is negative")]
		[TestCase(1, 4, true, Description = "argument scale more than equals")]
		public void CreateNumberValidator_WithInvalidArguments_ShouldThrowException(
			int precision, int scale, bool onlyPositive)
		{
			Action act = () => new NumberValidator(precision, scale, onlyPositive);
			act.Should().Throw<ArgumentException>();
		}

		[Test]
		public void CreateNumberValidator_WithValidArguments_ShouldNotThrowException()
		{
			Action act = () => new NumberValidator(1, 0, true);
			act.Should().NotThrow();
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