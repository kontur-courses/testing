using System;
using System.Net;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace HomeExercises
{
	[TestFixture]
	public class NumberValidator_Should
	{
		[TestCase(0, 1, true, TestName = "Precision_LessOrEqualZero")]
		[TestCase(2, -1, true, TestName = "Scale_LessThanZero")]
		[TestCase(0, 1, true, TestName = "Scale_GreatThanPrecision")]
		public void ThrowArgumentException(int precision, int scale, bool isOnlyPositive)
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(precision, scale, isOnlyPositive));
		}
	}

	[TestFixture]
	public class IsValidNumber_Should
	{
		private readonly NumberValidator numberValidator = new NumberValidator(5, 2);
		private readonly NumberValidator onlyPositiveNumberValidator = new NumberValidator(4, 2, true);

		[TestCase("", TestName = "StringIsEmpty")]
		[TestCase(null, TestName = "StringIsNull")]
		[TestCase("^2.15", TestName = "IsNotValidSign")]
		[TestCase("s.15", TestName = "IntPartIsNotDigit")]
		[TestCase("0.s", TestName = "FracPartIsNotDigit")]
		[TestCase(".15", TestName = "HasNoIntPart")]
		[TestCase("0.", TestName = "IsNotFracPart_ButPoint")]
		[TestCase("1.1234", TestName = "NotEnoughLengthForFracPart_ButEnoughForWholeNumber")]
		[TestCase("123.123", TestName = "WholeNumberIsLongerThanPrecision")]
		[TestCase("123456", TestName = "IntPartIsLongerThanPrecision")]
		public void ReturnFalse(string number)
		{
			numberValidator.IsValidNumber(number).Should().BeFalse();
		}

		[TestCase("5", TestName = "IntPartOnly")]
        [TestCase("+123.12", TestName = "WholeNumberLengthIsEqualPrecision_ButIsPlus")]
		[TestCase("0.15", TestName = "SeparatorIsPoint")]
		[TestCase("0,15", TestName = "SeparatorIsComma")]
		[TestCase("+0.15", TestName = "SignIsPlus")]
		[TestCase("-0.15", TestName = "SignIsMinus")]
		[TestCase("00.00", TestName = "IntPartWithOnlyZeros")]
        public void ReturnTrue(string number)
		{
			numberValidator.IsValidNumber(number).Should().BeTrue();
		}

        public void ReturnFalse_OnlyPossitiveNumberValidatorWithMinus()
		{
			onlyPositiveNumberValidator.IsValidNumber("-1.12").Should().BeFalse();
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
			// Здесь не учитывалось, что "+" - не нужно считать.
			var intPart = match.Groups[1].Value == "+" ?
				match.Groups[2].Value.Length :
				match.Groups[1].Value.Length + match.Groups[2].Value.Length;

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