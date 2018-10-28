using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	[TestFixture, Category("NumberValidator::Creation")]
	public class NumberValidatorTests_Should
	{
		[TestCase(-1, 0, true, "precision must be a positive number", TestName = "when precision is negative")]
		[TestCase(0, 0, true, "precision must be a positive number", TestName = "when precision is zero")]
		[TestCase(1, -1, true, "scale must be a non-negative number less than precision", TestName = "when scale is negative")]
		[TestCase(1, 2, true, "scale must be a non-negative number less than precision", TestName = "when scale is larger than precision")]
		[TestCase(1, 1, true, "scale must be a non-negative number less than precision", TestName = "when scale equals precision")]
        public void ConstructorShouldThrowArgumentException(int precision, int scale, bool onlyPositive, string message)
		{
			Action constructor = () => new NumberValidator(precision, scale, onlyPositive);
			constructor.Should().Throw<ArgumentException>().WithMessage(message);
        }

		[TestCase(1, 0, true, TestName = "when scale is zero")]
		[TestCase(2, 1, true, TestName = "when scale is less than precision")]
		public void ConstructorShouldNotThrowArgumentException(int precision, int scale, bool onlyPositive)
		{
			Action constructor = () => new NumberValidator(precision, scale, onlyPositive);
			constructor.Should().NotThrow<ArgumentException>();
        }
	}

	[TestFixture, Category("NumberValidator::IsValidNumber")]
	public class NumberValidatorIsValidNumberTests_Should
	{
		[TestCase(1, 0, true, null, TestName = "when value is null")]
		[TestCase(1, 0, true, "", TestName = "when value is empty string")]
		[TestCase(6, 5, true, "a.cd", TestName = "when value is non-number string")]
		[TestCase(6, 5, true, " \n\t\r", TestName = "when value is white space")]
		[TestCase(6, 5, true, "0,,0", TestName = "when delimited by several comas")]
		[TestCase(6, 5, true, "0..0", TestName = "when delimited by several decimal points")]
		[TestCase(10, 2, true, "0.003", TestName = "when fraction part is longer than scale")]
		[TestCase(3, 2, true, "00.00", TestName = "when number is longer than precision")]
		[TestCase(3, 2, true, "0.", TestName = "when without fraction part, but with delimiter")]
		[TestCase(4, 2, true, "-0.0", TestName = "when onlyPositive is set on negative number")]
		[TestCase(3, 2, false, "-00.0", TestName = "when number with minus sign is longer than precision")]
		[TestCase(3, 2, true, "+00.0", TestName = "when number with plus sign is longer than precision")]
		[TestCase(3, 2, true, ".0", TestName = "when no integer part")]
		[TestCase(10, 9, true, "5.5.5", TestName = "when more than one fraction part")]
		[TestCase(6, 5, true, "++5.6", TestName = "when more than one sign")]
        public void ShouldBeFalse(int precision, int scale, bool onlyPositive, string value)
		{
			new NumberValidator(precision, scale, onlyPositive).IsValidNumber(value).Should().BeFalse();
		}

		[TestCase(2, 1, true, "0", TestName = "when no fraction part")]
		[TestCase(3, 2, true, "0,1", TestName = "when delimited by coma")]
		[TestCase(4, 2, true, "+0.2", TestName = "when number with plus sign")]
		[TestCase(4,2, false, "-0.0", TestName = "when onlyPositive is false on negative number")]
		public void ShouldBeTrue(int precision, int scale, bool onlyPositive, string value)
		{
			new NumberValidator(precision, scale, onlyPositive).IsValidNumber(value).Should().BeTrue();
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
				throw new ArgumentException("scale must be a non-negative number less than precision");
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