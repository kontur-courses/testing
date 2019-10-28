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
	    public void NumberValidator_ThrowsArgumentException_IfScaleLessThanZero()
	    {
	        Action action = () => new NumberValidator(17, -1, true);

	        action
	            .ShouldThrow<ArgumentException>()
	            .WithMessage("scale must be a non-negative number less than precision");
	    }

	    [Test]
	    public void NumberValidator_ThrowsArgumentException_IfPrecisionLessOrEqualToZero()
	    {
	        Action action = () => new NumberValidator(0, 0, true);

	        action
	            .ShouldThrow<ArgumentException>()
	            .WithMessage("precision must be a positive number");
	    }

	    [Test]
	    public void NumberValidator_NotThrowsArgumentException_IfScaleLessThanPrecision()
	    {
	        Action action = () => new NumberValidator(17, 16, true);

	        action
	            .ShouldNotThrow<ArgumentException>();
	    }

	    [Test]
	    public void NumberValidator_ThrowsArgumentException_IfScaleIsEqualToPrecision()
	    {
	        Action action = () => new NumberValidator(17, 17, true);

	        action
	            .ShouldThrow<ArgumentException>()
	            .WithMessage("scale must be a non-negative number less than precision");
	    }

	    [Test]
	    public void IsValidNumber_ShouldBeTrue_IfNumberScaleIsZero()
	    {
	        new NumberValidator(17, 2, true).IsValidNumber("123").Should().BeTrue();
	    }

	    [Test]
	    public void IsValidNumber_ShouldBeTrue_IfNumberScaleLessThanValidatorScale()
	    {
	        new NumberValidator(17, 2, true).IsValidNumber("123.1").Should().BeTrue();
	    }

	    [Test]
	    public void IsValidNumber_ShouldBeTrue_IfNumberScaleIsEqualToValidatorScale()
	    {
	        new NumberValidator(17, 2, true).IsValidNumber("123.12").Should().BeTrue();
	    }

	    [Test]
	    public void IsValidNumber_ShouldBeFalse_IfNumberScaleBiggerThanValidatorScale()
	    {
	        new NumberValidator(17, 2, true).IsValidNumber("123.123").Should().BeFalse();
	    }

	    [Test]
	    public void IsValidNumber_IntegerPartShouldIncreasePrecision()
	    {
	        new NumberValidator(5, 2, true).IsValidNumber("123.12").Should().BeTrue();
	        new NumberValidator(5, 2, true).IsValidNumber("1234.12").Should().BeFalse();
	    }

	    [Test]
	    public void IsValidNumber_FractionalPartShouldIncreasePrecision()
	    {
	        new NumberValidator(5, 2, true).IsValidNumber("123.12").Should().BeTrue();
	        new NumberValidator(5, 2, true).IsValidNumber("123.123").Should().BeFalse();
	    }

	    [Test]
	    public void IsValidNumber_SignShouldIncreasePrecision()
	    {
	        new NumberValidator(5, 2, true).IsValidNumber("123.12").Should().BeTrue();
	        new NumberValidator(5, 2).IsValidNumber("-123.12").Should().BeFalse();
	        new NumberValidator(5, 2, true).IsValidNumber("+123.12").Should().BeFalse();
	    }

	    [Test]
	    public void IsValidNumber_ShouldBeFalse_IfValueIsNotNumber()
	    {
	        new NumberValidator(5, 2, true).IsValidNumber("aaa.12").Should().BeFalse();
	        new NumberValidator(5, 2, true).IsValidNumber("123.aa").Should().BeFalse();
	        new NumberValidator(5, 2, true).IsValidNumber("123a12").Should().BeFalse();
	    }

	    [Test]
	    public void IsValidNumber_ShouldBeTrue_IfSeparatorIsСomma()
	    {
	        new NumberValidator(5, 2, true).IsValidNumber("123,12").Should().BeTrue();
	    }

	    [Test]
	    public void IsValidNumber_ShouldBeFalse_IfNegativeNumberAndOnlyPositiveValidator()
	    {
	        new NumberValidator(5, 2, true).IsValidNumber("-12.12").Should().BeFalse();
	    }

	    [Test]
	    public void IsValidNumber_ShouldBeTrue_IfNegativeNumberAndNotOnlyPositiveValidator()
	    {
	        new NumberValidator(5, 2).IsValidNumber("-12.12").Should().BeTrue();
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