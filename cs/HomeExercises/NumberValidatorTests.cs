using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
      #region Creation Tests
		[TestCase(-1, 0, false, TestName = "ShouldThrowArgumentException_DuringCreationWithNegativePrecision")]
		[TestCase(0, 0, false, TestName = "ShouldThrowArgumentException_DuringCreationWithZeroPrecision")]
		[TestCase(1, 1, true, TestName = "ShouldThrowArgumentException_DuringCreation_WhenScaleEqualToPrecision")]
		[TestCase(1, 2, true, TestName = "ShouldThrowArgumentException_DuringCreation_WhenScaleGreaterThanPrecision")]
		[TestCase(1, -1, true, TestName = "houldThrowArgumentException_DuringCreationWithNegativeScale")]
		public void CreationThrowsArgumentExceptionTest(int precision, int scale, bool onlyPositive)
		{
         FluentActions.Invoking(() => new NumberValidator(precision, scale, onlyPositive)).Should().Throw<ArgumentException>();
      }

		[TestCase(1, 0, true, TestName = "NumberValidator_ShouldBeCreated_WhenPrecisionIsGreatenThenZero")]
		public void CreationNotThrowExceptions(int precision, int scale, bool onlyPositive)
		{
         FluentActions.Invoking(() => new NumberValidator(precision, scale, onlyPositive)).Should().NotThrow();
      }
      #endregion

      #region 'IsValidNumber' method tests
      public class IsValidNumberTests
		{
         [TestCase(1, 0, false, null, ExpectedResult = false, TestName = "Should_BeFalse_WhenInputValueIsNull")]
         [TestCase(1, 0, false, "", ExpectedResult = false, TestName = "Should_BeFalse_WhenInputValueIsEmptyString")]
         [TestCase(1, 0, false, " ", ExpectedResult = false, TestName = "Should_BeFalse_WhenInputValueIsWhitespace")]
         [TestCase(1, 0, false, "1.", ExpectedResult = false, TestName = "Should_IgnoreIntegerValueWithDot_WhenItsLengthIsLessThanPrecision")]
         [TestCase(1, 0, true, "+1", ExpectedResult = false, TestName = "Should_IgnoreValueWithPositiveSign_WhenPrecisionLengthIsLess")]
         [TestCase(1, 0, false, "-1", ExpectedResult = false, TestName = "Should_IgnoreValueWithNegativeSign_WhenPrecisionLengthIsLess")]
         [TestCase(2, 0, true, "-1", ExpectedResult = false, TestName = "Should_BeFalse_WhenInputsNegativeNumberStringInOnlyPositiveValidator")]
         [TestCase(2, 1, false, "17.1", ExpectedResult = false, TestName = "Should_BeFalse_WhenBothIntegerAndFractionalPartLengthAreGreaterThanPrecision")]
         [TestCase(4, 1, false, "1.11", ExpectedResult = false, TestName = "Should_BeFalse_WhenFractionalPartLengthIsGreaterThanScale")]
         [TestCase(3, 2, true, "a.bc", ExpectedResult = false, TestName = "Should_IgnoreNonNumericStrings")]
         public bool IsValidNumberTest(int precision, int scale, bool onlyPositive, string value)
         {
            return new NumberValidator(precision, scale, onlyPositive).IsValidNumber(value);
         }
		}
		#endregion
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