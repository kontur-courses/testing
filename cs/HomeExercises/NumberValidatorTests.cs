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
				yield return new TestCaseData(1, 0, true).SetName("correct on correct data");
				yield return new TestCaseData(7, 5, false).SetName("correct on correct data with non-zero scale");
			}
		}
		
		private static IEnumerable IncorrectArgumentsTestCases
		{
			get
			{
				yield return new TestCaseData(-1, 0).SetName("precision is negative");
				yield return new TestCaseData(0, 0).SetName("precision is zero");
				yield return new TestCaseData(7, 7).SetName("scale is equal to precision");
				yield return new TestCaseData(7, 8).SetName("scale is greater than precision");
				yield return new TestCaseData(7, -1).SetName("scale is negative");
			}
		}
		private static IEnumerable ValidTestCases
		{
			get
			{
				yield return new TestCaseData(2, 1, true, "0.0").SetName("when zero with decimal point");
				yield return new TestCaseData(17, 2, true, "0").SetName("when zero without decimal point");
				yield return new TestCaseData(2, 1, true, "0,0").SetName("when zero with comma");
				yield return new TestCaseData(4, 2, true, "00.00").SetName("when additional zeros");
				yield return new TestCaseData(17, 2, false, "-1.0").SetName("when negative value");
				yield return new TestCaseData(9, 4, true, "12345.6789").SetName("when value with all digits");
				yield return new TestCaseData(9, 4, false, "+145.69").SetName("when value with plus sign");
				yield return new TestCaseData(10, 0, true, int.MaxValue.ToString()).SetName("when value is int.MaxValue");
				yield return new TestCaseData(11, 0, false, int.MinValue.ToString()).SetName("when value is int.MinValue");
			}
		}
		
		private static IEnumerable InvalidTestCases
		{
			get
			{
				yield return new TestCaseData(3, 2, true, ".0").SetName("when point is on the left from zero");
				yield return new TestCaseData(3, 2, true, "0.").SetName("when point is on the right from zero");
				yield return new TestCaseData(17, 2, true, "125.001").SetName("when fractional part length is bigger than scale");
				yield return new TestCaseData(17, 2, true, "100 01").SetName("when whitespace is between digits");
				yield return new TestCaseData(3, 1, true, "-1.0").SetName("when value is negative and onlyPositive is true");
				yield return new TestCaseData(3, 1, false, "1!.3").SetName("when unexpected symbol");
				yield return new TestCaseData(3, 1, false, "-.0").SetName("when minus before point");
				yield return new TestCaseData(3, 1, false, "-").SetName("when minus without digits");
				yield return new TestCaseData(3, 1, true, "+").SetName("when plus without digits");
				yield return new TestCaseData(2, 1, true, "").SetName("when value is empty");
				yield return new TestCaseData(2, 1, true, "    ").SetName("when value is whitespaces");
				yield return new TestCaseData(2, 1, true, null).SetName("when value is null");
				yield return new TestCaseData(2, 1, true, "qwerty").SetName("when value is letters");
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