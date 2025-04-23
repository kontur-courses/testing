﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		private static NumberValidator _numberValidatorOnlyPositiv = new NumberValidator(4, 2, true);
		private static NumberValidator _numberValidatorAllValues = new NumberValidator(4, 2);

		[TestCase(-1, 2, true, TestName = "When_Precision_Is_Not_Positive")]
		[TestCase(1, -2, true, TestName = "When_Scale_Is_Negative(")]
		[TestCase(12, 14, true, TestName = "When_Scale_More_Than_Precision")]
		public void NumberValidator_Should_Throw_ArgumentException(int precision, int scale, bool onlyPositive)
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(precision, scale, onlyPositive));
		}

		[TestCaseSource(nameof(IsValidNumberShouldReturnFalse))]
		[TestCaseSource(nameof(IsValidNumberShouldReturnFalse))]
		public bool IsValidNumber_Should_Return(NumberValidator numberValidator, string value)
		{
			return numberValidator.IsValidNumber(value);
		}

		private static IEnumerable<TestCaseData> IsValidNumberShouldReturnFalse()
		{
			yield return new TestCaseData(_numberValidatorAllValues, null)
				.SetName("False_When_Value_Is_Null")
				.Returns(false);

			yield return new TestCaseData(_numberValidatorAllValues, "")
				.SetName("False_When_Value_Is_Empty")
				.Returns(false);

			yield return new TestCaseData(_numberValidatorAllValues, "ab.c")
				.SetName("False_When_Value_Is_Not_Match_Regex")
				.Returns(false);

			yield return new TestCaseData(_numberValidatorAllValues, "000.00")
				.SetName("False_When_Value_Length_More_Than_Precision")
				.Returns(false);

			yield return new TestCaseData(_numberValidatorOnlyPositiv, "-0.0")
				.SetName("False_When_OnlyPositive_Is_True_And_Value_Is_Negative")
				.Returns(false);

			yield return new TestCaseData(_numberValidatorAllValues, "+00.00")
				.SetName("False_When_Value_Starts_With_Sign_And_Numbers_Length_Is_Equal_To_Precision")
				.Returns(false);

			yield return new TestCaseData(_numberValidatorAllValues, " +00.00")
				.SetName("False_When_Value_Has_Symbols_Before_Number")
				.Returns(false);

			yield return new TestCaseData(_numberValidatorAllValues, "+00.00 ")
				.SetName("False_When_Value_Has_Symbols_After_Number")
				.Returns(false);
		}

		private static IEnumerable<TestCaseData> IsValidNumberShouldReturnTrue()
		{
			yield return new TestCaseData(_numberValidatorAllValues, "+0.0")
				.SetName("True_When_Value_Starts_With_Plus")
				.Returns(true);

			yield return new TestCaseData(_numberValidatorAllValues, "0.0")
				.SetName("True_When_Value_Is_Positive")
				.Returns(true);

			yield return new TestCaseData(_numberValidatorAllValues, "0,0")
				.SetName("True_When_IntPart_And_FracPart_Separate_By_Comma")
				.Returns(true);

			yield return new TestCaseData(_numberValidatorAllValues, "0!0")
				.SetName("True_When_IntPart_And_FracPart_Separate_By_Another_Symbol_As_Comma_Or_Dot")
				.Returns(false);
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