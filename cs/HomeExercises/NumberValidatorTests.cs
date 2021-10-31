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
		public void NumberValidation_ThrowArgumentException_IfScaleIsMoreOrEqualPrecision(int p, int s)
		{
			Action act = () => new NumberValidator(p, s, true);
			act.Should().Throw<ArgumentException>($"precision is {p} and scale is {s}");
		}

		[Test]
		public void
			NumberValidation_DoesNotThrowArgumentException_IfPrecisionIsPositiveAndScaleIsNotNegativeAndLessThanPrecision()
		{
			Action act = () => new NumberValidator(1, 0, true);
			act.Should().NotThrow<ArgumentException>("precision is 1 and scale is 0");
		}

		[Test]
		public void IsValidNumber_False_ValueIsNull()
		{
			var res = new NumberValidator(1, 0, true).IsValidNumber(null);
			res.Should().BeFalse("value is null");
		}

		[Test]
		public void IsValidNumber_False_ValueIsEmpty()
		{
			var res = new NumberValidator(1, 0, true).IsValidNumber("");
			res.Should().BeFalse("value is \"\"");
		}

		[TestCase("a")]
		[TestCase(".")]
		[TestCase("a.sd")]
		[TestCase("++1")]
		[TestCase("1+.3")]
		public void IsValidNumber_False_ValueIsNan(string value)
		{
			var res = new NumberValidator(3, 2, true).IsValidNumber(value);
			res.Should().BeFalse($"value is \"{value}\"");
		}

		[TestCase("00.00")]
		[TestCase("-0.00")]
		[TestCase("+0.00")]
		public void IsValidNumber_False_SumOfIntAndFracPartsIsBiggerThanPrecision(string value)
		{
			var res = new NumberValidator(3, 2, true).IsValidNumber(value);
			res.Should().BeFalse($"value is \"{value}\"");
		}

		[Test]
		public void IsValidNumber_False_FracPartIsLongerThanScale()
		{
			var res = new NumberValidator(17, 2, true).IsValidNumber("0.000");
			res.Should().BeFalse("value is 0.000");
		}

		[Test]
		public void IsValidNumber_False_ValueIsNegativeAndNumberValidatorIsOnlyPositive()
		{
			var res = new NumberValidator(17, 2, true).IsValidNumber("-1");
			res.Should().BeFalse("value is -1 and onlyPositive=true");
		}

		[TestCase("0")]
		[TestCase("+0")]
		public void IsValidNumber_True_ValueIsNotNegativeAndNumberValidatorIsOnlyPositive(string value)
		{
			var res = new NumberValidator(17, 2, true).IsValidNumber(value);
			res.Should().BeTrue($"value is \"{value}\"");
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