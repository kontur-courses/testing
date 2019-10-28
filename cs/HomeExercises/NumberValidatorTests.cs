﻿using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[TestCase(1, 0, TestName = "Constructor_PositivePrecisionZeroScale_Succeeds")]
		[TestCase(2, 1, TestName = "Constructor_PositivePrecisionPositiveScale_Succeeds")]
		public void Constructor_CorrectArguments_Succeeds(int precision, int scale)
		{
			Assert.DoesNotThrow(() => new NumberValidator(precision, scale, true));
			Assert.DoesNotThrow(() => new NumberValidator(precision, scale, false));
		}

		[TestCase(-1, 2, TestName = "Constructor_NegativePrecision_ExceptionThrown")]
		[TestCase(0, 2, TestName = "Constructor_ZeroPrecision_ExceptionThrown")]
		[TestCase(0, -1, TestName = "Constructor_NegativeScale_ExceptionThrown")]
		[TestCase(1, 1, TestName = "Constructor_ScaleEqualToPrecision_ExceptionThrown")]
		[TestCase(1, 2, TestName = "Constructor_ScaleGreaterThanPrecision_ExceptionThrown")]
		public void Constructor_IncorrectArguments_ExceptionThrown(int precision, int scale)
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(precision, scale, true));
			Assert.Throws<ArgumentException>(() => new NumberValidator(precision, scale, false));
		}

		[Test]
		public void IsValidNumber_NotUsesStaticFields()
		{
			NumberValidator numberValidator1 = new NumberValidator(5, 2, false);
			NumberValidator numberValidator2 = new NumberValidator(3, 0, true);
			Assert.IsTrue(numberValidator1.IsValidNumber("-0"));
			Assert.IsTrue(numberValidator1.IsValidNumber("0.00"));
			Assert.IsTrue(numberValidator1.IsValidNumber("00000"));
			Assert.IsFalse(numberValidator2.IsValidNumber("-0"));
			Assert.IsFalse(numberValidator2.IsValidNumber("0.00"));
			Assert.IsFalse(numberValidator2.IsValidNumber("00000"));
		}

		[TestCase("0", TestName = "IsValidNumber_Integer_True")]
		[TestCase("-0", true, TestName = "IsValidNumber_NegativeInteger_True")]
		[TestCase("+0", TestName = "IsValidNumber_IntegerWithPlus_True")]
		public void IsValidNumber_ValidIntegers_True(string value, bool negative = false)
		{
			NumberValidator numberValidator = new NumberValidator(17, 2, false);
			NumberValidator zeroScaleNumberValidator = new NumberValidator(17, 0, false);

			Assert.IsTrue(numberValidator.IsValidNumber(value), "should return true with positive scale");
			Assert.IsTrue(zeroScaleNumberValidator.IsValidNumber(value), "should return true with zero scale");

			if (!negative)
			{
				NumberValidator onlyPositiveNumberValidator = new NumberValidator(17, 2, true);
				NumberValidator zeroScaleOnlyPositiveNumberValidator = new NumberValidator(17, 0, true);
				
				Assert.IsTrue(onlyPositiveNumberValidator.IsValidNumber(value), "should return true when onlyPositive with positive scale");
				Assert.IsTrue(zeroScaleOnlyPositiveNumberValidator.IsValidNumber(value), "should return true when onlyPositive with zero scale");
			}
		}

		[TestCase("0.00", TestName = "IsValidNumber_Fraction_True")]
		[TestCase("0,00", TestName = "IsValidNumber_CommaFraction_True")]
		[TestCase("-0.00", true, TestName = "IsValidNumber_NegativeFraction_True")]
		[TestCase("+0.00", TestName = "IsValidNumber_FractionWithPlus_True")]
		public void IsValidNumber_ValidFractions_True(string value, bool negative = false)
		{
			NumberValidator numberValidator = new NumberValidator(17, 2, false);

			Assert.IsTrue(numberValidator.IsValidNumber(value), "fraction is valid when precision and scale aren't exceeded");

			if (!negative)
			{
				NumberValidator onlyPositiveNumberValidator = new NumberValidator(17, 2, true);
				
				Assert.IsTrue(onlyPositiveNumberValidator.IsValidNumber(value), "non-negative fraction is valid on onlyPositive validator when precision and scale aren't exceeded");
			}
		}

		[TestCase("", TestName = "IsValidNumber_EmptyString_False")]
		[TestCase(null, TestName = "IsValidNumber_Null_False")]
		public void IsValidNumber_NullOrEmpty_False(string value)
		{
			Assert.IsFalse(new NumberValidator(17, 2, false).IsValidNumber(value));
			Assert.IsFalse(new NumberValidator(17, 2, true).IsValidNumber(value));
			Assert.IsFalse(new NumberValidator(17, 0, false).IsValidNumber(value));
			Assert.IsFalse(new NumberValidator(17, 0, true).IsValidNumber(value));
		}

		[TestCase("abcdefg", TestName = "IsValidNumber_NonNumberString_False")]
		[TestCase("a.bc", TestName = "IsValidNumber_NonNumberStringWithPoint_False")]
		public void IsValidNumber_NonNumberStrings_False(string value)
		{
			Assert.IsFalse(new NumberValidator(17, 2, false).IsValidNumber(value));
			Assert.IsFalse(new NumberValidator(17, 2, true).IsValidNumber(value));
			Assert.IsFalse(new NumberValidator(17, 0, false).IsValidNumber(value));
			Assert.IsFalse(new NumberValidator(17, 0, true).IsValidNumber(value));
		}


		[TestCase("-0.00", TestName = "IsValidNumber_NegativeFractionExceededPrecision_False")]
		[TestCase("+0.00", TestName = "IsValidNumber_FractionWithPlusExceededPrecision_False")]
		[TestCase("00.00", TestName = "IsValidNumber_FractionExceededPrecision_False")]
		[TestCase("0000", TestName = "IsValidNumber_IntegerExceededPrecision_False")]
		[TestCase("-000", TestName = "IsValidNumber_NegativeIntegerExceededPrecision_False")]
		[TestCase("+000", TestName = "IsValidNumber_IntegerWithPlusExceededPrecision_False")]
		public void IsValidNumber_NumbersExceededPrecision_False(string value)
		{
			Assert.IsFalse(new NumberValidator(3, 2, false).IsValidNumber(value));
			Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber(value));
		}

		[TestCase("-0", TestName = "IsValidNumber_IntegerViolatedOnlyPositive_False")]
		[TestCase("-0.00", TestName = "IsValidNumber_FractionViolatedOnlyPositive_False")]
		public void IsValidNumber_NumbersViolatedOnlyPositive_False(string value)
		{
			Assert.IsFalse(new NumberValidator(17, 2, true).IsValidNumber(value));
		}

		[TestCase(2, "0.000")]
		[TestCase(2, "0.000")]
		[TestCase(0, "0.0")]
		[TestCase(0, "0.0")]
		public void IsValidNumber_FractionExceededScale_False(int scale, string value)
		{
			Assert.IsFalse(new NumberValidator(17, scale, false).IsValidNumber(value));
			Assert.IsFalse(new NumberValidator(17, scale, true).IsValidNumber(value));
		}

		[Test]
		public void IsValidNumber_IntegerWithPoint_False()
		{
			Assert.IsFalse(new NumberValidator(17, 2, false).IsValidNumber("0."));
			Assert.IsFalse(new NumberValidator(17, 2, true).IsValidNumber("0."));
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