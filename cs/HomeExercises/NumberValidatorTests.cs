using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[TestCase(1, 0, true, TestName = "DoesNotThrow_IfPositivePrecisionZeroScaleOnlyPositive")]
		[TestCase(1, 0, false, TestName = "DoesNotThrow_IfPositivePrecisionZeroScale")]
		[TestCase(2, 1, true, TestName = "DoesNotThrow_IfPositivePrecisionPositiveScaleOnlyPositive")]
		[TestCase(2, 1, false, TestName = "DoesNotThrow_IfPositivePrecisionPositiveScale")]
        public void DoesNotThrow_WithCorrectArguments(int precision, int scale, bool onlyPositive)
		{
			Assert.DoesNotThrow(() => new NumberValidator(precision, scale, onlyPositive));
		}

		[TestCase(-1, 2, TestName = "Throws_IfNegativePrecision")]
		[TestCase(0, 2, TestName = "Throws_IfZeroPrecision")]
		[TestCase(0, -1, TestName = "Throws_IfNegativeScale")]
		[TestCase(1, 1, TestName = "Throws_IfScaleAndPrecisionAreEqual")]
		[TestCase(1, 2, TestName = "Throws_IfScaleGreaterThanPrecision")]
        public void ThrowsArgumentException_WithIncorrectArguments(int precision, int scale)
        {
	        Assert.Throws<ArgumentException>(() => new NumberValidator(precision, scale, true));
	        Assert.Throws<ArgumentException>(() => new NumberValidator(precision, scale, false));
        }

		[TestCase(17, 2, true, "0", TestName = "IsValidNumber_Integer_Valid")]
		[TestCase(17, 2, false, "0", TestName = "OnlyPositive_IsValidNumber_Integer_Valid")]
		[TestCase(17, 2, false, "-0", TestName = "IsValidNumber_NegativeInteger_Valid")]
		[TestCase(17, 2, true, "+0", TestName = "OnlyPositive_IsValidNumber_PlusInteger_Valid")]
		[TestCase(17, 2, false, "+0", TestName = "IsValidNumber_PlusInteger_Valid")]

        [TestCase(17, 2, true, "0.00", TestName = "IsValidNumber_Fraction_Valid")]
        [TestCase(17, 2, true, "0.00", TestName = "IsValidNumber_CommaFraction_Valid")]
        [TestCase(17, 2, false, "0.00", TestName = "OnlyPositive_IsValidNumber_Fraction_Valid")]
        [TestCase(17, 2, false, "-0.00", TestName = "IsValidNumber_NegativeFraction_Valid")]
        [TestCase(17, 2, true, "+0.00", TestName = "OnlyPositive_IsValidNumber_PlusFraction_Valid")]
        [TestCase(17, 2, false, "+0.00", TestName = "IsValidNumber_PlusFraction_Valid")]
        public void IsValidNumber_Valid(int precision, int scale, bool onlyPositive, string value)
        {
			Assert.IsTrue(new NumberValidator(precision, scale, onlyPositive).IsValidNumber(value));
        }

		[TestCase(17, 2, false, "", TestName = "IsValidNumber_Empty_IsNotValid")]
		[TestCase(17, 2, false, null, TestName = "IsValidNumber_Null_IsNotValid")]
		[TestCase(17, 2, true, "", TestName = "OnlyPositive_IsValidNumber_Empty_IsNotValid")]
		[TestCase(17, 2, true, null, TestName = "OnlyPositive_IsValidNumber_Null_IsNotValid")]

		[TestCase(17, 2, false, "a.bc", TestName = "IsValidNumber_NonNumber_IsNotValid")]
		[TestCase(17, 2, true, "a.bc", TestName = "OnlyPositive_IsValidNumber_NonNumber_IsNotValid")]

        [TestCase(17, 2, true, "-0", TestName = "OnlyPositive_IsValidNumber_NegativeInteger_IsNotValid")]
		[TestCase(17, 2, true, "-0.00", TestName = "OnlyPositive_IsValidNumber_NegativeFraction_IsNotValid")]

		[TestCase(3, 2, false, "-0.00", TestName = "IsValidNumber_NegativeFraction_ExceededPrecision_IsNotValid")]
		[TestCase(3, 2, false, "+0.00", TestName = "IsValidNumber_PlusFraction_ExceededPrecision_IsNotValid")]
		[TestCase(3, 2, false, "00.00", TestName = "IsValidNumber_Fraction_ExceededPrecision_IsNotValid")]
		[TestCase(3, 2, false, "0000", TestName = "IsValidNumber_Integer_ExceededPrecision_IsNotValid")]
		[TestCase(3, 2, false, "-000", TestName = "IsValidNumber_NegativeInteger_ExceededPrecision_IsNotValid")]
		[TestCase(3, 2, false, "+000", TestName = "IsValidNumber_PlusInteger_ExceededPrecision_IsNotValid")]

		[TestCase(3, 2, true, "-0.00", TestName = "OnlyPositive_IsValidNumber_NegativeFraction_ExceededPrecision_IsNotValid")]
		[TestCase(3, 2, true, "+0.00", TestName = "OnlyPositive_IsValidNumber_PlusFraction_ExceededPrecision_IsNotValid")]
		[TestCase(3, 2, true, "00.00", TestName = "OnlyPositive_IsValidNumber_Fraction_ExceededPrecision_IsNotValid")]
		[TestCase(3, 2, true, "0000", TestName = "OnlyPositive_IsValidNumber_Integer_ExceededPrecision_IsNotValid")]
		[TestCase(3, 2, true, "-000", TestName = "OnlyPositive_IsValidNumber_NegativeInteger_ExceededPrecision_IsNotValid")]
		[TestCase(3, 2, true, "+000", TestName = "OnlyPositive_IsValidNumber_PlusInteger_ExceededPrecision_IsNotValid")]

        [TestCase(17, 2, false, "0.000", TestName = "IsValidNumber_ExceededScale_IsNotValid")]
        [TestCase(17, 2, false, "-0.000", TestName = "IsValidNumber_Negative_ExceededScale_IsNotValid")]
        [TestCase(17, 2, false, "+0.000", TestName = "IsValidNumber_Plus_ExceededScale_IsNotValid")]
		public void IsValidNumber_IsNotValid(int precision, int scale, bool onlyPositive, string value)
        {
	        Assert.IsFalse(new NumberValidator(precision, scale, onlyPositive).IsValidNumber(value));
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