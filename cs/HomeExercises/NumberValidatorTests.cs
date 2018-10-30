using System;
using System.Collections;
using System.Net.Mime;
using System.Text.RegularExpressions;
using FluentAssertions;
using Microsoft.Win32;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[Test]
		public void Constructor_ShouldFail_WhenPrecisionNegative()
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(-3, 2, true));
		}

		[Test]
		public void Constructor_ShouldFail_WhenScaleNegative()
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(3, -2, true));
		}

		[Test]
		public void Constructor_ShouldFail_WhenScaleIsLargerThanPrecision()
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(1, 2, true));
		}

		[Test]
		public void Constructor_ShouldFail_WhenScaleEqualPrecision()
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(2, 2, true));
		}

		[Test]
		public void Constructor_ShouldNotFail_WhenPositivePrecisionAndScale()
		{
			Assert.DoesNotThrow(() => new NumberValidator(2, 1, true));
		}
		
		private static IEnumerable ValidTestCases
		{
			get
			{
				yield return new TestCaseData(new NumberValidator(17, 2, true), "0.0")
					.Returns(true)
					.SetName("WhenHasFractionalPart");
				yield return new TestCaseData(new NumberValidator(17, 2, true), "0")
					.Returns(true)
					.SetName("WhenHasNotFractionalPart");
				yield return new TestCaseData(new NumberValidator(4, 2, true), "+1.23")
					.Returns(true)
					.SetName("WhenHasFractionalPartAndPlusSign");
				yield return new TestCaseData(new NumberValidator(4, 2, false), "-1.23")
					.Returns(true)
					.SetName("WhenNegativeAndHasFractionalPart");
				yield return new TestCaseData(new NumberValidator(4, 2, false), "-1")
					.Returns(true)
					.SetName("WhenNegativeAndHasNotFractionalPart");
				yield return new TestCaseData(new NumberValidator(4, 2, false), "-001")
					.Returns(true)
					.SetName("WhenNegativeAndHasZerosBetweenSignAndNonZeroNumber");
			}
		}
		
		[Test, TestCaseSource(nameof(ValidTestCases))]
		public bool IsValidNumber_ShouldBeTrue(NumberValidator validator, string representation)
		{
			return validator.IsValidNumber(representation);
		}
		
		private static IEnumerable InvalidTestCases
		{
			get
			{
				yield return new TestCaseData(new NumberValidator(4, 2, true), "-1.23")
					.Returns(false)
					.SetName("WhenOnlyPositiveAndSignIsMinus");
				yield return new TestCaseData(new NumberValidator(3, 2, false), "-1.23")
					.Returns(false)
					.SetName("WhenPrecisionIsSameAsExpectedButValueIsSigned");
				yield return new TestCaseData(new NumberValidator(17, 2, true), "1.230")
					.Returns(false)
					.SetName("WhenScaleIsLargerThanExpected");
				yield return new TestCaseData(new NumberValidator(17, 2, true), "a.1d")
					.Returns(false)
					.SetName("WhenNumberStringHasNotOnlyNumbers");
			}
		}
		
		[Test, TestCaseSource(nameof(InvalidTestCases))]
		public bool IsValidNumber_ShouldBeFalse(NumberValidator validator, string representation)
		{
			return validator.IsValidNumber(representation);
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
			// описанным в Формате описи документов, направляемых в налоговый орган в электронном виде по телекоммуникационным
			// каналам связи:
			// Формат числового значения указывается в виде N(m.к), где m – максимальное количество знаков в числе,
			// включая знак (для отрицательного числа), 
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