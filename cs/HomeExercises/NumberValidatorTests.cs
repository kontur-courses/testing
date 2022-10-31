using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[TestCase(1,0,true)]
		public void NumberValidator_WithСorrectParameters_DoesNotThrow(int precision, int scale, bool onlyPositive)
		{
			Assert.DoesNotThrow(() => new NumberValidator(precision, scale, onlyPositive));
		}
		
		[TestCase(0, 0, TestName = "NumberValidator_WithPrecisionZero_ThrowArgumentException")]
		[TestCase(-1, 0, TestName = "NumberValidator_WithNegativePrecision_ThrowArgumentException")]
		[TestCase(1, -1, TestName = "NumberValidator_WithNegativeScale_ThrowArgumentException")] 
		[TestCase(1, 1, TestName = "NumberValidator_WithPrecisionEqualScale_ThrowArgumentException")]
		[TestCase(1, 2, TestName = "NumberValidator_WithPrecisionLessScale_ThrowArgumentException")]
		public void NumberValidator_WithNotCorrectParameters_ThrowArgumentException(int precision, int scale)
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(precision, scale));
		}

		[TestCase("")]
		[TestCase(null)]
		public void IsValidNumber_IsFalse_WithNullOrEmptyString(string input)
		{
			var numberValidator = new NumberValidator(3, 2, true);
			numberValidator.IsValidNumber(input).Should().BeFalse();
		}

		[TestCase(".0")]
		[TestCase("0.")]
		[TestCase("a.0")]
        [TestCase("0.a")]
        [TestCase("0 0")]
        public void IsValidNumber_IsFalse_WithNotRegexString(string input)
        {
	        var numberValidator = new NumberValidator(3, 2, true);
	        numberValidator.IsValidNumber(input).Should().BeFalse();
        }

        [TestCase("+0.00")]
        [TestCase("00.00")]
        public void IsValidNumber_False_WhenLenMorePrecisionString(string input)
        {
	        var numberValidator = new NumberValidator(3, 2, true);
	        numberValidator.IsValidNumber(input).Should()
		        .BeFalse("для NumberValidator с precision=3 количество знаков без точки должно быть меньше 4");
        }

		[Test]
		public void IsValidNumber_False_WhenLenFracMoreScale()
		{
			var numberValidator = new NumberValidator(4, 2, true);
			numberValidator.IsValidNumber("0.111").Should()
				.BeFalse("для NumberValidator с scale=2 количество знаков после запятой должно быть меньше 3");
		}

		[Test] 
		public void IsValidNumber_False_WhenOnlyPositiveTrueButNumberNegative()
		{
			var numberValidator = new NumberValidator(3, 2, true);
			numberValidator.IsValidNumber("-2.2").Should()
				.BeFalse("для NumberValidator с onlyPositive=true число \"-2.2\" должно быть без минуса");
		}

		[TestCase("0")]
		[TestCase("000")]
		[TestCase("1.1")]
		[TestCase("1,11")]
		[TestCase("+0.0")]
		[TestCase("-2.2")]
		public void IsValidNumber_IsTrue_WithCorrectInput(string input)
		{
			var numberValidator = new NumberValidator(3, 2, false);
			numberValidator.IsValidNumber(input).Should()
				.BeTrue("для NumberValidator(3,2,false) IsValidNumber для строки \""+input+"\" должен быть true");
		}

		[TestCase("+012343838388330.00")]
		[TestCase("2.22555555555555555")]
		public void IsValidNumber_True_WithBigPrecisionAndScale(string input)
		{
			var numberValidator2 = new NumberValidator(21, 18, true);
			numberValidator2.IsValidNumber(input).Should()
				.BeTrue("для NumberValidator(21,18,true) IsValidNumber для строки \"" + input + "\" должен быть true");
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