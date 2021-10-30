using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[Test]
		public void Ctor_DoesNotThrow_WithValidArgs()
		{
			Action action = () => new NumberValidator(5, 1, true);
			action.Should().NotThrow<ArgumentException>();
		}

		[TestCase(-1, 2, TestName = "PrecisionIsNegative_ThrowsArgumentException")]
		[TestCase(0, 2, TestName = "PrecisionIsZero_ThrowsArgumentException")]
		[TestCase(3, -2, TestName = "ScaleIsNegative_ThrowsArgumentException")]
		[TestCase(1, 2, TestName = "ScaleIsGreaterThanPrecision_ThrowsArgumentException")]
		[TestCase(1, 1, TestName = "ScaleEqualsPrecision_ThrowsArgumentException")]
		public void Ctor_Throws_WithInvalidArgs(int precision, int scale)
		{ 
			Action action = () => new NumberValidator(precision, scale);
			action.Should().Throw<ArgumentException>();
		}

		[TestCase("30.99", 4, 1, TestName = "FracPartIsMoreThanScale_False", ExpectedResult = false)]
		[TestCase("12345", 4, 2, TestName = "IntPartIsMoreThanPrecision_False", ExpectedResult = false)]
		[TestCase("-1234", 4, 2, TestName = "IntPartWithSignIsMoreThanPrecision_False", ExpectedResult = false)]
		[TestCase("30.99", 3, 2, TestName = "IntPartPlusFracPartIsMoreThanPrecision_False", ExpectedResult = false)]
		[TestCase("-1.5", 4, 2, true, TestName = "IsNegativeWhenOnlyPositiveTrue_False", ExpectedResult = false)]
		[TestCase("", 2, 1, TestName = "EmptyString_False", ExpectedResult = false)]
		[TestCase(null, 2, 1, TestName = "NullString_False", ExpectedResult = false)]
		[TestCase("0", 4, 2, TestName = "OnlyIntPart_True", ExpectedResult = true)]
		[TestCase("0.002", 4, 3, TestName = "ValidIntAndFracPart_True", ExpectedResult = true)]
		[TestCase("+5", 4, 2, TestName = "PositiveSign_True", ExpectedResult = true)]
		[TestCase("a.sd", 4, 2, TestName = "StringWithLetters_False", ExpectedResult = false)]
		public bool IsValidNumber(string value, int precision, int scale, bool onlyPositive = false)
		{
			var validator = new NumberValidator(precision, scale, onlyPositive);
			return validator.IsValidNumber(value);
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