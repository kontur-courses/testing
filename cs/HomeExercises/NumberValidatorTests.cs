using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
    
    public class NumberValidatorTests
    {
        [TestCase(-1, 2, TestName = "WhenPrecisionIsNegative")]
        [TestCase(0, 2, TestName = "WhenPrecisionIsZero")]
        [TestCase(1, -1, TestName = "WhenScaleIsNegative")]
        [TestCase(1, 3, TestName = "WhenScaleIsGreaterThenPrecision")]
        public void NumberValidator_RaiseArgumentException(int precision, int scale)
        {
            new Action(() => new NumberValidator(precision, scale)).Should().Throw<ArgumentException>();
        }

        [TestCase(17, 2, false, "0.0", TestName = "WhenStringContainDot_CountNumberLessThenPrecision_FracPartLessScale")]
        [TestCase(17, 2, false, "0,0", TestName = "WhenStringContainDot_CountNumberLessThenPrecision_FracPartLessScale")]
        [TestCase(17, 2, false, "0", TestName = "WhenStringNotContainDot_CountNumberLessThenPrecision")]
        [TestCase(17, 2, false, "+12", TestName = "WhenStringContainPlas")]
        [TestCase(17, 2, false, "-12", TestName = "WhenStringContainMinus")]
        [TestCase(17, 0, false, "222", TestName = "WhenStrindIsInteger_ScaleIsZero")]
        [TestCase(17, 2, true, "12", TestName = "WhenStringNotContainSign_OnlyPositiveIsTrue")]
        [TestCase(17, 2, true, "+12", TestName = "WhenStringContainPlas_OnlyPositiveIsTrue")]
        public void IsValidNumber_ReturnsTrue(int precision, int scale, bool onlyPositive, string value)
        {
            new NumberValidator(precision, scale, onlyPositive).IsValidNumber(value).Should().BeTrue();
        }

        [TestCase(3, 2, false, "0000", TestName = "WhenCountFracPartIsGreaterThenPrecision")]
        [TestCase(3, 1, false, "0.00", TestName = "WhenCountFracPartIsGreaterThenScale")]
        [TestCase(3, 2, false, "+000", TestName = "WhenCountNumberWithPlasIsGreaterThenPrecision")]
        [TestCase(3, 2, false, "-000", TestName = "WhenCountNumberWithMinusIsGreaterThenPrecision")]
        [TestCase(10, 2, false, "3+4", TestName = "WhenPlasIsInsideNumber")]
        [TestCase(10, 2, false, "3-4", TestName = "WhenMinusIsInsideNumber")]
        [TestCase(10, 2, false, "34+", TestName = "WhenPlasIsInsideNumber")]
        [TestCase(10, 2, false, "34-", TestName = "WhenMinusIsInsideNumber")]
        [TestCase(10, 2, false, "3 4", TestName = "WhenSpaceIsInsideNumber")]
        [TestCase(10, 2, false, "34 ", TestName = "WhenSpaceIsAfterNumber")]
        [TestCase(10, 2, false, " 34", TestName = "WhenSpaceIsBeforeNumber")]
        [TestCase(3, 2, false, "a", TestName = "WhenStringContainOtherSymbol")]
        [TestCase(3, 2, false, ".", TestName = "WhenStringContainOnlyDot")]
        [TestCase(3, 2, false, ".0", TestName = "WhenStringStartIsDot")]
        [TestCase(3, 2, false, "0.", TestName = "WhenStringEndIsDot")]
        [TestCase(3, 2, false, "", TestName = "WhenStringIsEmpty")]
        [TestCase(3, 2, false, " ", TestName = "WhenStringContainOnlySpace")]
        [TestCase(3, 2, false, null, TestName = "WhenStringIsNull")]
        [TestCase(3, 2, true, "-456", TestName = "WhenStringStartIsMinus_OnlyPositiveIsTrue")]
        public void IsValidNumber_ReturnFalse(int precision, int scale, bool onlyPositive, string value)
        {
            new NumberValidator(precision, scale, onlyPositive).IsValidNumber(value).Should().BeFalse();
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