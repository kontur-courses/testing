using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
    [TestFixture]
	public class NumberValidatorTests
	{
        #region Instance Tests
        [Test, Category("Instance")]
        [TestCase(true)]
        [TestCase(false)]
		public void Instance_NegativePrecisionAnySign_ShouldThrowException(bool onlyPositive)
        {
			Action action = () => new NumberValidator(-1, 2, onlyPositive);
			action.Should().Throw<ArgumentException>();
		}

		[Test, Category("Instance")]
		[TestCase(true)]
		[TestCase(false)]
		public void Instance_PositivePrecisionSuitableScaleAnySign_ShouldNotThrowException(bool onlyPositive)
		{
			Action action = () => new NumberValidator(1, 0, onlyPositive);
			action.Should().NotThrow<ArgumentException>();
		}

		[Test, Category("Instance")]
		[TestCase(true)]
		[TestCase(false)]
		public void Instance_PositivePrecisionScaleGreaterThanPrecisionAnySign_ShouldThrowException(bool onlyPositive)
		{
			Action action = () => new NumberValidator(1, 2, onlyPositive);
			action.Should().Throw<ArgumentException>();
		}

		[Test, Category("Instance")]
		[TestCase(true)]
		[TestCase(false)]
		public void Instance_PositivePrecisionNegativeScaleAnySign_ShouldThrowException(bool onlyPositive)
		{
			Action action = () => new NumberValidator(1, -1, onlyPositive);
			action.Should().Throw<ArgumentException>();
		}
        #endregion

        #region IsValidNumber Tests
        [Test, Category("IsValidNumber")]
		public void IsValidNumber_NonDigitChars_ShouldBeFalse()
        {
			NumberValidator numberValidator = new NumberValidator(3, 2, true);
			numberValidator.IsValidNumber("a.sd").Should().BeFalse();
        }

		[Test, Category("IsValidNumber")]
		public void IsValidNumber_NumberWithoutDigitsAfterPoint_ShouldBeTrue()
        {
			NumberValidator numberValidator = new NumberValidator(17, 2, true);
			numberValidator.IsValidNumber("0").Should().BeTrue();
		}

		[Test, Category("IsValidNumber")]
		public void IsValidNumber_NumberWithDigitAfterPoint_ShouldBeTrue()
		{
			NumberValidator numberValidator = new NumberValidator(17, 2, true);
			numberValidator.IsValidNumber("0.0").Should().BeTrue();
		}

		[Test, Category("IsValidNumber")]
		public void IsValidNumber_NumberWithPrecisionGreaterThanValidatorPrecision_ShouldBeFalse()
		{
			NumberValidator numberValidator = new NumberValidator(3, 2, true);
			numberValidator.IsValidNumber("00.00").Should().BeFalse();
		}

		[Test, Category("IsValidNumber")]
		public void IsValidNumber_NumberWithScaleGreaterThanValidatorScale_ShouldBeFalse()
		{
			NumberValidator numberValidator = new NumberValidator(17, 2, true);
			numberValidator.IsValidNumber("0.000").Should().BeFalse();
		}

		[Test, Category("IsValidNumber")]
		public void IsValidNumber_SignedNumberWithPrecisionGreaterThanValidatorPrecision_ShouldBeFalse()
        {
			NumberValidator numberValidator = new NumberValidator(3, 2, true);
			numberValidator.IsValidNumber("+1.23").Should().BeFalse();
		}

		[Test, Category("IsValidNumber")]
		public void IsValidNumber_SignedNumberWithPrecisionEqualToValidatorPrecision_ShouldBeTrue()
		{
			NumberValidator numberValidator = new NumberValidator(4, 2, true);
			numberValidator.IsValidNumber("+1.23").Should().BeTrue();
		}

		[Test, Category("IsValidNumber")]
		public void IsValidNumber_NegativeNumberWithValidatorOnlyPositiveAttribute_ShouldBeFalse()
        {
			NumberValidator numberValidator = new NumberValidator(10, 5, true);
			numberValidator.IsValidNumber("-1.23").Should().BeFalse();
		}

		[Test, Category("IsValidNumber")]
		public void IsValidNumber_NumberWithoutIntegerPart_ShouldBeFalse()
		{
			NumberValidator numberValidator = new NumberValidator(10, 5, true);
			numberValidator.IsValidNumber(".23").Should().BeFalse();
		}

		[Test, Category("IsValidNumber")]
		public void IsValidNumber_NumberWithPointButWithoutFractionPart_ShouldBeFalse()
		{
			NumberValidator numberValidator = new NumberValidator(10, 5, true);
			numberValidator.IsValidNumber("1.").Should().BeFalse();
		}
		#endregion
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