using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[Test]
		public void NumberValidation_ThrowArgumentException_IfPrecisionIsNotPositive()
		{
			Action act = () => new NumberValidator(-1, 2, true);
			act.Should().Throw<ArgumentException>("precision is -1");
		}

		[Test]
		public void NumberValidation_ThrowArgumentException_IfScaleIsNegative()
		{
			Action act = () => new NumberValidator(1, -1, true);
			act.Should().Throw<ArgumentException>("scale is 1");
		}

		[TestCase(1, 1)]
		[TestCase(1, 2)]
		public void NumberValidation_ThrowArgumentException_IfScaleIsMoreOrEqualPrecision(int precision, int scale)
		{
			Action act = () => new NumberValidator(precision, scale, true);
			act.Should().Throw<ArgumentException>($"precision is {precision} and scale is {scale}");
		}

		[TestCase(1, 0, true)]
		[TestCase(2, 1, true)]
		[TestCase(1, 0, false)]
		
		public void NumberValidation_DoesNotThrowArgumentException_IfPrecisionIsPositiveAndScaleIsNotNegativeAndLessThanPrecision(
			int precision,
			int scale,
			bool onlyPositive
			)
		{
			Action act = () => new NumberValidator(precision, scale, onlyPositive);
			act.Should().NotThrow<ArgumentException>(
				$"precision is \"{precision}\" and scale is \"{scale}\", number validator positive: \"{onlyPositive}\""
				);
		}

		[Test]
		public void IsValidNumber_False_ValueIsNull()
		{
			var validator = new NumberValidator(1, 0, true);
			var	result = validator.IsValidNumber(null);
			result.Should().BeFalse("value is null");
		}

		[Test]
		public void IsValidNumber_False_ValueIsEmpty()
		{
			var validator = new NumberValidator(1, 0, true);
			var	result = validator.IsValidNumber("");
			result.Should().BeFalse("value is \"\"");
		}

		[TestCase("a")]
		[TestCase(".")]
		[TestCase("a.sd")]
		[TestCase("++1")]
		[TestCase("1+.3")]
		public void IsValidNumber_False_ValueIsNan(string value)
		{
			var validator = new NumberValidator(3, 2, true);
			var result = validator.IsValidNumber(value);
			result.Should().BeFalse($"value is \"{value}\"");
		}

		[TestCase("00.00")]
		[TestCase("-0.00")]
		[TestCase("+0.00")]
		public void IsValidNumber_False_SumOfIntAndFracPartsIsBiggerThanPrecision(string value)
		{
			var validator = new NumberValidator(3, 2, true); 
			var result = validator.IsValidNumber(value);
			result.Should().BeFalse($"value is \"{value}\"");
		}

		[Test]
		public void IsValidNumber_False_FracPartIsLongerThanScale()
		{
			var validator = new NumberValidator(17, 2, true);
			var result = validator.IsValidNumber("0.000");
			result.Should().BeFalse("value is 0.000");
		}

		[TestCase("-1", Description = "number validator is onlyPositive")]
		[TestCase("-54", Description = "number validator is onlyPositive")]
		public void IsValidNumber_False_ValueIsNegative(string value)
		{
			var validator = new NumberValidator(17, 2, true);
			var result = validator.IsValidNumber("-1");
			result.Should().BeFalse($"value is \"{value}\" and number validator is onlyPositive");
		}

		[TestCase("0", Description = "number validator is onlyPositive")]
		[TestCase("1", Description = "number validator is onlyPositive")]
		public void IsValidNumber_True_ValueIsNotNegative(string value)
		{
			var validator = new NumberValidator(17, 2, true); 
			var result = validator.IsValidNumber(value);
			result.Should().BeTrue($"value is \"{value}\" and number validator is onlyPositive");
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