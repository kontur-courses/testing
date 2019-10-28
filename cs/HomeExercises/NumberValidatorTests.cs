using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
    [TestFixture]
	public class NumberValidatorTests
	{
        [Test]
        [TestCase(1, 0, false, TestName = "Constructor_DoesNotThrow_WhenPositivePrecision")]
        [TestCase(2, 1, false, TestName = "Constructor_DoesNotThrow_WhenPrecisionGreaterScale")]
        [TestCase(1, 0, true, TestName = "Constructor_DoesNotThrow_WhenNotOnlyPositive")]
        public void Constructor_DoesNotThrow(int precision, int scale, bool onlyPositive)
        {
            Action action = () => new NumberValidator(precision, scale, onlyPositive);
            action.Should().NotThrow();
        }

        [Test]
        [TestCase(0, 2, true, TestName = "Constructor_ThrowArgumentException_WhenNonPositivePrecision")]
        [TestCase(2, -1, true, TestName = "Constructor_ThrowArgumentException_WhenNegativeScale")]
        [TestCase(1, 2, true, TestName = " Constructor_ThrowArgumentException_WhenScaleGreaterPrecision")]
        public void Constructor_ThrowArgumentException(int precision, int scale, bool onlyPositive)
        {
            Action action = () => new NumberValidator(precision, scale, onlyPositive);
            action.Should().Throw<ArgumentException>();
        }

        [Test]
        [TestCase(3, 2, true, "11.11", ExpectedResult = false, TestName = "IsValidNumber_False_WhenActualPrecisionGreaterExpected")]
        [TestCase(3, 2, true, "12.34", ExpectedResult = false, TestName = "IsValidNumber_False_WhenOnlyPositiveExpectedButWasNegative")]
        [TestCase(17, 2, true, "0.000", ExpectedResult = false, TestName = "IsValidNumber_False_WhenActualScaleGreaterExpected")]
        [TestCase(17, 2, true, "a.sd", ExpectedResult = false, TestName = "IsValidNumber_False_WhenNotANumber")]
        [TestCase(10, 2, true, null, ExpectedResult = false, TestName = "IsValidNumber_False_WhenNullValue")]
        [TestCase(10, 2, true, "", ExpectedResult = false, TestName = "IsValidNumber_False_WhenEmptyString")]
        [TestCase(3, 2, false, "-1.23", ExpectedResult = false, TestName = "IsValidNumber_False_SignTakenIntoAccountPrecision")]
        [TestCase(3, 2, true, "++1", ExpectedResult = false, TestName = "IsValidNumber_False_WhenSeveralSigns")]
        [TestCase(3, 2, true, "1..2", ExpectedResult = false, TestName = "IsValidNumber_False_WhenSeveralSeparators")]
        [TestCase(3, 2, true, "1.", ExpectedResult = false, TestName = "IsValidNumber_False_WhenEndsOnDot")]
        [TestCase(3, 2, true, ".1", ExpectedResult = false, TestName = "IsValidNumber_False_WhenStartsOnDot")]

        [TestCase(10, 2, false, "-1.23", ExpectedResult = true, TestName = "IsValidNumber_True_WhenNegativeNumber")]
        [TestCase(4, 2, true, "+1.23", ExpectedResult = true, TestName = "IsValidNumber_True_WhenPositiveNumber")]
        [TestCase(4, 1, true, "1234", ExpectedResult = true , TestName = "IsValidNumber_True_WhenValidatorPrecisionSameAsValue")]
        [TestCase(3, 2, true, "1.23", ExpectedResult = true, TestName = "IsValidNumber_True_WhenValidatorScaleSameAsValue")]
        [TestCase(3, 1, true, "1", ExpectedResult = true, TestName = "IsValidNumber_True_WhenInteger")]
        [TestCase(3, 2, true, "1,23", ExpectedResult = true, TestName = "IsValidNumber_False_WhenSeparatorIsNotPoint")]
        public bool IsValidNumberTest(int precision, int scale, bool onlyPositive, string value)
        {
            return new NumberValidator(precision, scale, onlyPositive).IsValidNumber(value);
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