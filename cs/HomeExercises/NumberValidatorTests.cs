using System;
using System.Collections;
using System.Dynamic;
using System.Text.RegularExpressions;
using FluentAssertions;

using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[Test, TestCaseSource(nameof(IncorrectArgumentsTestCases))]
		public void NumberValidator_Should_ThrowArgumentException_When_IncorrectArguments(int precision, int scale)
		{
			Action action = () => { new NumberValidator(precision, scale); };
			action.ShouldThrow<ArgumentException>("arguments are incorrect");
		}
		
		[Test, TestCaseSource(nameof(CorrectArgumentsTestCases))]
		public void NumberValidator_Should_NotThrowExceptions_When_CorrectArguments(int precision, int scale, bool onlyPositive)
		{
			Action action = () => { new NumberValidator(precision, scale, onlyPositive); };
			action.ShouldNotThrow("arguments are correct");
		}
		
		[Test, TestCaseSource(nameof(ValidTestCases))]
		public void IsValidNumber_Should_ReturnTrue_When_NumberIsValid(int precision, int scale, bool onlyPositive, string number)
		{
			var numberValidator = new NumberValidator(precision, scale, onlyPositive);
			numberValidator.IsValidNumber(number).Should().BeTrue("all input data is correct");
		}

		[Test, TestCaseSource(nameof(InvalidTestCases))]
		public void IsValidNumber_Should_ReturnFalse_When_NumberIsInvalid(int precision, int scale, bool onlyPositive, string number)
		{
			var numberValidator = new NumberValidator(precision, scale, onlyPositive);
			numberValidator.IsValidNumber(number).Should().BeFalse("all input data is incorrect");
		}
		
		
		private static IEnumerable CorrectArgumentsTestCases
		{
			get
			{
				yield return new TestCaseData(1, 0, true).SetName("correct on (1, 0, true)");
				yield return new TestCaseData(7, 5, false).SetName("correct on (7, 5, false)");
			}
		}
		
		private static IEnumerable IncorrectArgumentsTestCases
		{
			get
			{
				yield return new TestCaseData(-1, 0).SetName("precision is negative: -1");
				yield return new TestCaseData(0, 0).SetName("precision is zero");
				yield return new TestCaseData(7, 7).SetName("scale is equal to precision");
				yield return new TestCaseData(7, 8).SetName("scale is greater than precision");
				yield return new TestCaseData(7, -1).SetName("scale is negative: -1");
			}
		}
		private static IEnumerable ValidTestCases
		{
			get
			{
				yield return new TestCaseData(2, 1, true, "0.0").SetName("valid on (2, 1, true) and value \"0.0\"");
				yield return new TestCaseData(17, 2, true, "0").SetName("valid on (17, 2, true) and value \"0\"");
				yield return new TestCaseData(2, 1, true, "0,0").SetName("valid on (2, 1, true) and value \"0,0\"");
				yield return new TestCaseData(17, 2, false, "-1.0").SetName("valid on (17, 2, false) and value \"-1.0\"");
				yield return new TestCaseData(9, 4, true, "12345.6789").SetName("valid on (9, 4, true) and value \"12345.6789\"");
				yield return new TestCaseData(9, 4, false, "+145.69").SetName("valid on (9, 4, false) and value \"+145.69\"");
				yield return new TestCaseData(10, 0, true, int.MaxValue.ToString()).SetName("valid on (10, 0, true) and value int.MaxValue");
				yield return new TestCaseData(11, 0, false, int.MinValue.ToString()).SetName("valid on (11, 0, false) and value int.MinValue");
				yield return new TestCaseData(3, 1, true, "+5.0").SetName("valid on (3, 1, true) and value \"+5.0\"");
			}
		}
		
		private static IEnumerable InvalidTestCases
		{
			get
			{
				yield return new TestCaseData(3, 2, true, "00.00").SetName("invalid on (3, 2, true) and value \"00.00\"");
				yield return new TestCaseData(3, 2, true, ".0").SetName("invalid on (3, 2, true) and value \".0\"");
				yield return new TestCaseData(3, 2, true, "0.").SetName("invalid on (3, 2, true) and value \"0.\"");
				yield return new TestCaseData(17, 2, true, "125.001").SetName("invalid on (17, 2, true) and value \"125.001\"");
				yield return new TestCaseData(17, 2, true, "100 01").SetName("invalid on (17, 2, true) and value \"100 01\"");
				yield return new TestCaseData(3, 1, true, "-1.0").SetName("invalid on (3, 1, true) and value \"-1.0\"");
				yield return new TestCaseData(3, 1, false, "-.0").SetName("invalid on (3, 1, true) and value \"-.0\"");
				yield return new TestCaseData(3, 1, false, "-").SetName("invalid on (3, 1, true) and value \"-\"");
				yield return new TestCaseData(3, 1, true, "+").SetName("invalid on (3, 1, true) and value \"+\"");
				yield return new TestCaseData(2, 1, true, "").SetName("invalid on (2, 1, true) and value empty string");
				yield return new TestCaseData(2, 1, true, "    ").SetName("invalid on (2, 1, true) and value whitespace string");
				yield return new TestCaseData(2, 1, true, null).SetName("invalid on (2, 1, true) and value null");
				yield return new TestCaseData(2, 1, true, "qwerty").SetName("invalid on (2, 1, true) and value \"qwerty\"");
			}
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