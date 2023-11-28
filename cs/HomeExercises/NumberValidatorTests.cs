using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		private static IEnumerable<TestCaseData> ArgExcCaseTestData
		{
			get
			{
				yield return new TestCaseData(-1, 2, true).SetName("WhenPassNegativePrecision");
				yield return new TestCaseData(1, -2, true).SetName("WhenPassNegativeScale");
				yield return new TestCaseData(2, 2, true).SetName("WhenPrecisionIsEqualToTheScale");
			}
		}

		[Test]
        [TestCaseSource(nameof(ArgExcCaseTestData))]
        public void NumberValidatorCtor_WhenPassInvalidArguments_ShouldThrowArgumentException(int precision,
	        int scale, bool onlyPositive)
		{
			TestDelegate testDelegate = () => new NumberValidator(precision, scale, onlyPositive);

			Assert.Throws<ArgumentException>(testDelegate);
		}

		[Test]
		public void NumberValidatorCtor_WhenPassValidArguments_ShouldNotThrows()
		{
			TestDelegate testDelegate = () => new NumberValidator(1, 0, true);

			Assert.DoesNotThrow(testDelegate);
		}

		private static IEnumerable<TestCaseData> InvalidArgumentCasesTestData
		{
			get
			{
				yield return new TestCaseData("a.sd", false).SetName("WhenLettersInsteadOfNumber");
				yield return new TestCaseData("2.!", false).SetName("WhenSymbolsInsteadOfNumber");
				yield return new TestCaseData(null!, false).SetName("WhenPassNumberIsNull");
				yield return new TestCaseData("", false).SetName("WhenPassNumberIsEmpty");
				yield return new TestCaseData("-0.00", false).SetName("WhenIntPartWithNegativeSignMoreThanPrecision");
				yield return new TestCaseData("+1.23", false).SetName("WhenIntPartWithPositiveSignMoreThanPrecision");
				yield return new TestCaseData("0.000", false).SetName("WhenFractionalPartMoreThanScale");
            }
		}
		private static IEnumerable<TestCaseData> ValidArgumentCasesTestData
		{
			get
			{
				yield return new TestCaseData("0", true).SetName("WhenFractionalPartIsMissing");
				yield return new TestCaseData("0.0", true).SetName("WhenNumberIsValid");
			}
		}

        private NumberValidator validator;

		[SetUp]
		public void SetUp()
		{
			validator = new NumberValidator(3, 2, true);
		}

		[TestCaseSource(nameof(ValidArgumentCasesTestData))]
		[TestCaseSource(nameof(InvalidArgumentCasesTestData))]
		public void WhenPassInvalidArguments_ShouldReturnFalse(string number, bool expectedResult)
		{
			var actualResult = validator.IsValidNumber(number);

			Assert.AreEqual(expectedResult, actualResult);
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