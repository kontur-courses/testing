using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[TestCase(-3, 2, true, "precision must be a positive number", TestName = "Exception if precision is negative")]
		[TestCase(1, 2, false, "precision must be a non-negative number less or equal than precision", TestName = "Exception if scale greater than precision")]
		[TestCase(1, -2, false, "precision must be a non-negative number less or equal than precision", TestName = "Exception if scale is negative")]
		public void NumberValidator_ShouldThrowException_IfIncorrectArguments(int precision, int scale,
			bool onlyPositive, string exceptionMessage)
		{
			Action act = () => new NumberValidator(precision, scale, onlyPositive);
			act.Should()
				.Throw<ArgumentException>()
				.WithMessage(exceptionMessage);
		}

		[TestCase(1, 0, true, TestName = "Precision is positive number")]
		[TestCase(2, 1, true, TestName = "Scale is non-negative and less than precision")]
		[TestCase(1, 0, false, TestName = "Not only positive numbers")]
		public void NumberValidator_DoesNotThrow_IfCorrectArguments(int precision, int scale, bool onlyPositive)
		{
			Action act = () => new NumberValidator(precision, scale, onlyPositive);
			act.Should().NotThrow();
		}
		
		
		[TestCase(17, 2, true, "1.123", ExpectedResult = false, TestName = "IsFalse_IfFracPartGreaterThanScale")]
		[TestCase(3, 2, true, "123.123", ExpectedResult = false, TestName = "IsFalse_IfNumberOfDigitsMoreThanPrecision")]
		[TestCase(3, 2, true, "-1.2", ExpectedResult = false, TestName = "IsFalse_IfNegativeNumberWhenOnlyPositiveIsAllowed")]
		[TestCase(3, 2, true, "-1.23", ExpectedResult = false,  TestName = "IsFalse_IfSignIsOutOfBounds")]
		[TestCase(3, 2, true, "abc", ExpectedResult = false, TestName = "IsFalse_IfPassedValueNotANumber")]
		[TestCase(3, 2, true, "a.bc", ExpectedResult = false, TestName = "IsFalse_IfPassedValueNotANumberAndHasPoint")]
		[TestCase(3, 2, true, "", ExpectedResult = false, TestName = "IsFalse_IfNumberIsEmpty")]
		[TestCase(3, 2, true, null, ExpectedResult = false, TestName = "IsFalse_IfNumberIsNull")]
		
		[TestCase(3, 2, true, "0", ExpectedResult = true, TestName = "IsTrue_IfValueIsZero")]
		[TestCase(3, 2, true, "1.5", ExpectedResult = true, TestName = "IsTrue_IfNumberInBounds")]
		[TestCase(3, 2, true, "1,5", ExpectedResult = true, TestName = "IsTrue_IfNumberHasCommaInsteadPoint")]
		[TestCase(3, 2, true, "+1.5", ExpectedResult = true, TestName = "IsTrue_IfNumberHasPlusSign")]
		[TestCase(3, 2, false, "-1.5", ExpectedResult = true, TestName = "IsTrue_IfNumberIsNegativeWhenNegativeIsAllowed")]
		[TestCase(3, 2, false, "12", ExpectedResult = true, TestName = "IsTrue_IfNumberNotHaveFracPart")]
		public bool IsValidNumber_ReturnBool_IfPassValue(int precision, int scale, bool onlyPositive, string number)
		{
			var validator = new NumberValidator(precision, scale, onlyPositive);

			return validator.IsValidNumber(number);
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