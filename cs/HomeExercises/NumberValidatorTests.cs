using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[TestCase(-1, 2, "precision must be a positive number")]
		[TestCase(0, 2, "precision must be a positive number")]
		[TestCase(2, -2, "precision must be a non-negative number less or equal than precision")]
		[TestCase(2, 4, "precision must be a non-negative number less or equal than precision")]
		public void NumberRegistry_ConstructorWithIncorrectInput_ShouldThrowArgumentException(int precision, int scale, string exeptionMessage)
		{
			Action act = () => NumberRegistry.GetCurrentNumber(precision, scale);

			act.Should().Throw<ArgumentException>(exeptionMessage);
		}
		
		[Test]
		public void NumberRegistry_NormalInput_ShouldNotThrowExceptions()
		{
			Action act = () => NumberRegistry.GetCurrentNumber(1, 0);

			act.Should().NotThrow();
		}

		//"Num without sign"
		
		[TestCase(17, 2, true, "0.00", TestName = "{m}_OnNumWithoutSign")]
		[TestCase(4, 2, true, "+1.23", TestName = "{m}_OnCommonInput")]
		[TestCase(17, 2, true, "0", TestName = "{m}_OnSingleZero")]
		[TestCase(17, 2, true, "0.0", TestName = "{m}_OnZeroWithAnEmptyFractionalPart")]
		public void IsValidNumber_ShouldReturnTrue(int precision, int scale, bool onlyPositive, string value)
		{
			var numberValidator = NumberRegistry.GetCurrentNumber(precision, scale, onlyPositive);

			var validFlag = numberValidator.IsValidNumber(value);

			validFlag.Should().BeTrue();
		}

		[TestCase(3, 2, true, "-0.00", TestName = "{m}_OnNegativeInputWithOnlyPositiveValidator")]
		[TestCase(3, 2, true, "a.sd", TestName = "{m}_OnInputWithLetters")]
		[TestCase(17, 2, true, "0.000", TestName = "{m}_OnFractionalPartLongerThanScale")]
		[TestCase(3, 2, true, "+0.00", TestName = "{m}_OnNumLengthLongerThanPrecision")]
		[TestCase(3, 2, true, "00.00", TestName = "{m}_OnNumLengthLongerThanPrecision")]
		[TestCase(3, 2, true, null, TestName = "{m}_OnNullValue")]
		[TestCase(3, 2, true, "", TestName = "{m}_OnEmptyStringValue")]
		public void IsValidNumber_ShouldReturnFalse(int precision, int scale, bool onlyPositive, string value)
		{
			var numberValidator = NumberRegistry.GetCurrentNumber(precision, scale, onlyPositive);

			var validFlag = numberValidator.IsValidNumber(value);

			validFlag.Should().BeFalse();
		}
	}
}

	public class NumberRegistry
	{
		public static NumberValidator GetCurrentNumber(int precision, int scale = 0, bool onlyPositive = false)
		{
			return new NumberValidator(precision, scale, onlyPositive);
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
