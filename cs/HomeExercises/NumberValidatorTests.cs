using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using FluentAssertions.Execution;
using NUnit.Framework;

namespace HomeExercises
{
    [TestFixture]
    public class NumberValidatorTests
    {
        [TestCase(-1, 0, true)]
        public void NumberValidator_NegativePrecision_ArgumentException(int precision, int scale, bool onlyPositive)
        {
            new Action(() => new NumberValidator(precision, scale, onlyPositive)).Should().Throw<ArgumentException>();
        }

        [TestCase(1, 0, true)]
        public void NumberValidator_PositivePrecision_NotThrow(int precision, int scale, bool onlyPositive)
        {
            new Action(() => new NumberValidator(precision, scale, onlyPositive)).Should().NotThrow();
        }

        [TestCase(-1, 0, true)]
        public void NumberValidator_NegativeScale_ArgumentException(int precision, int scale, bool onlyPositive)
        {
            new Action(() => new NumberValidator(precision, scale, onlyPositive)).Should().Throw<ArgumentException>();
        }

        [TestCase(1, 0, true)]
        public void NumberValidator_PositiveScale_NotThrow(int precision, int scale, bool onlyPositive)
        {
            new Action(() => new NumberValidator(precision, scale, onlyPositive)).Should().NotThrow();
        }

        [TestCase(1, 3, true)]
        public void NumberValidator_ScaleGreaterPrecision_ArgumentException(int precision, int scale, bool onlyPositive)
        {
            new Action(() => new NumberValidator(precision, scale, onlyPositive)).Should().Throw<ArgumentException>();
        }

        [TestCase(17, 2, true, "0.0")]
        [TestCase(17, 2, true, "0")]
        [TestCase(4, 2, true, "+1.23")]
        public void IsValidNumber_ValidNumber(int precision, int scale, bool onlyPositive, string value)
        {
            new NumberValidator(precision, scale, onlyPositive).IsValidNumber(value).Should().BeTrue();
        }

        [TestCase(3, 2, true, "00.00")]
        [TestCase(3, 2, true, "-0.00")]
        [TestCase(3, 2, true, "+0.00")]
        [TestCase(3, 2, true, "+1.23")]
        [TestCase(17, 2, true, "0.000")]
        [TestCase(3, 2, true, "-1.23")]
        [TestCase(3, 2, true, "a.sd")]
        [TestCase(3, 2, true, ".")]
        [TestCase(3, 2, true, "")]
        [TestCase(3, 2, true, null)]
        public void IsValidNumber_NotValidNumber(int precision, int scale, bool onlyPositive, string value)
        {
            new NumberValidator(precision, scale, onlyPositive).IsValidNumber(value).Should().BeFalse();
        }

        [TestCase(5, 2, false, "-0.3")]
        [TestCase(5, 2, false, "-1")]
        public void IsValidNumber_NegativeNumberNotOnlyPositive(int precision, int scale, bool onlyPositive, string value)
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