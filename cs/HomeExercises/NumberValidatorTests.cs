using System;
using System.Collections;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[TestCase(-1, TestName = "precision is negative")]
		[TestCase(0, TestName = "precision is zero")]
		public void Should_ThrowArgumentException_When_PrecisionIsLessThanOrEqualToZero(int precision)
		{
			Action action = () =>
			{
				new NumberValidator(precision);
			};
			action.ShouldThrow<ArgumentException>("precision is incorrect");
		}
		
		[TestCase(7, TestName = "scale is equal to precision")]
		[TestCase(8, TestName = "scale is greater than precision")]
		public void Should_ThrowArgumentException_When_ScaleIsGreaterThanOrEqualToPrecision(int scale)
		{
			Action action = () =>
			{
				new NumberValidator(7, scale);
			};
			action.ShouldThrow<ArgumentException>("scale is incorrect");
		}
		
		[Test]
		public void Should_ThrowArgumentException_When_ScaleIsNegative()
		{
			Action action = () =>
			{
				new NumberValidator(7, -1);
			};
			action.ShouldThrow<ArgumentException>("scale is negative");
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

		private static IEnumerable ValidTestCases
		{
			get
			{
				yield return new TestCaseData(2, 1, true, "0.0");
				yield return new TestCaseData(17, 2, true, "0");
				yield return new TestCaseData(2, 1, true, "0,0");
				yield return new TestCaseData(17, 2, false, "-1.0");
				yield return new TestCaseData(9, 4, true, "12345.6789");
				yield return new TestCaseData(17, 2, true, "900000000000000.01");
				yield return new TestCaseData(3, 1, true, "+5.0");
			}
		}
		
		private static IEnumerable InvalidTestCases
		{
			get
			{
				yield return new TestCaseData(3, 2, true, "00.00");
				yield return new TestCaseData(3, 1, true, "-1.0");
				yield return new TestCaseData(2, 1, true, "");
				yield return new TestCaseData(2, 1, true, null);
				yield return new TestCaseData(17, 2, true, "100.001");
				yield return new TestCaseData(17, 2, true, "100 01");
				yield return new TestCaseData(2, 1, true, "qwerty");
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