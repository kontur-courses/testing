using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{

        [Test]
        public void Constructor_ThrowsException_OnNegativePrecision()
        {
            Action act = () => new NumberValidator(-1, 2, true);
            act.ShouldThrow<ArgumentException>()
                .And.Message.Should().Be("precision must be a positive number");
        }

        [Test]
        public void Constructor_ThrowsException_OnNegativeScale()
        {
            Action act = () => new NumberValidator(1, -2, true);
            act.ShouldThrow<ArgumentException>()
                .And.Message.Should().Be("precision must be a non-negative number less or equal than precision");
        }

        [Test]
        public void Constructor_ThrowsException_IfScaleGreaterOrEqualToPrecision()
        {
            Action act = () => new NumberValidator(3, 6, true);
            act.ShouldThrow<ArgumentException>()
                .And.Message.Should().Be("precision must be a non-negative number less or equal than precision");
        }

        [Test]
        public void IsValid_ReturnFalse_OnEmptyString()
        {
            new NumberValidator(10, 3, true).IsValidNumber("").Should().BeFalse();
        }

        [Test]
        public void IsValid_ReturnFalse_OnNonDigitString()
        {
            new NumberValidator(10, 3, true).IsValidNumber("a.sd").Should().BeFalse();
        }

        [Test]
        public void IsValid_ReturnFalse_OnOnlyPositiveWithNegative()
        {
            new NumberValidator(4, 2, true).IsValidNumber("-1.23").Should().BeFalse();
        }

        [Test]
        public void IsValid_ReturnFalse_IfIntAndFracPartsGreaterPrecision()
        {
            new NumberValidator(3, 2, true).IsValidNumber("-12.23").Should().BeFalse();
        }

        [Test]
        public void IsValid_ReturnFalse_IfFracPartGreaterScale()
        {
            new NumberValidator(5, 2, true).IsValidNumber("0.000").Should().BeFalse();
        }

        [Test]
        public void IsValid_ReturnTrue_PrecisionAndScaleAreEqualToParts()
        {
            new NumberValidator(6, 3, true).IsValidNumber("+12.000").Should().BeTrue();
        }

        [Test]
        public void IsValid_ReturnTrue_PrecisionAndScaleAreGreaterThanParts()
        {
            new NumberValidator(8, 4, true).IsValidNumber("+12.000").Should().BeTrue();
        }

        [Test]
        public void IsValid_CountsSign()
        {
            new NumberValidator(3, 2).IsValidNumber("-1.00").Should().BeFalse();
        }

        [Test]
        public void IsValid_ReturnTrue_OnInteger()
        {
            new NumberValidator(3).IsValidNumber("+56").Should().BeTrue();
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