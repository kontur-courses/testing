using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[TestCase(-1, 2, false, TestName = "precision is negative")]
		[TestCase(0, 2, true, TestName = "precision is zero")]
		[TestCase(2, -2, true, TestName = "scale is negative")]
		[TestCase(1, 2, true, TestName = "scale is greater than precision")]
		[TestCase(5, 5, true, TestName = "precision and scale are same")]
		public void ShouldThrowException_WithWrongParametersInConstructor(int precision, int scale, bool onlyPositive)
		{
			Action nv = () => new NumberValidator(precision, scale, onlyPositive);
			nv.Should().Throw<ArgumentException>();
		}

		[TestCase(1, 0, true, TestName = "precision is positive, scale is zero")]
		[TestCase(5, 3, true, TestName = "precision and scale is positive")]

		public void ShouldNotThrowException_WithCorrectParametersInConstructor(int precision, int scale, bool onlyPositive)
		{
			Action nv = () => new NumberValidator(precision, scale, onlyPositive);
			nv.Should().NotThrow<ArgumentException>();
		}

		[TestCase(17, 2, true, "0.0", TestName = "0.0 dot separated with precision=17 and scale=2")]
		[TestCase(17, 2, true, "0,0", TestName = "0,0 comma separated with precision=17 and scale=2")]
		[TestCase(17, 2, true, "0", TestName = "0 without fracPart with precision=17 and scale=2")]
		[TestCase(4, 2, true, "+1.23", TestName = "+1.23 dot separated with precision=4 and scale=2")]
		[TestCase(4, 2, false, "-1.23", TestName = "-1.23 with not only positive param")]
		public void ShouldBeValid(int precision, int scale, bool onlyPositive, string number)
		{
			var nv = new NumberValidator(precision, scale, onlyPositive);
			nv.IsValidNumber(number).Should().BeTrue();
		}

		[TestCase(3, 2, true, "00.00", TestName = "00.00 dot separated with precision=3 and scale=2")]
		[TestCase(3, 2, true, "-0.00", TestName = "-0.00 dot separated with precision=3 and scale=2")]
		[TestCase(3, 2, true, "+1.23", TestName = "+1.23 dot separated with precision=3 and scale=2")]
		[TestCase(17, 2, true, "0.000", TestName = "0.000 dot separated with precision=17 and scale=2")]
		[TestCase(3, 2, true, "-1.23", TestName = "-1.23 dot separated with precision=3 and scale=2")]
		[TestCase(3, 2, true, "a.sd", TestName = "string with letters")]
		[TestCase(3, 2, true, "", TestName = "empty string")]
		[TestCase(3, 2, true, null, TestName = "null")]
		public void ShouldBeInvalid(int precision, int scale, bool onlyPositive, string number)
		{
			var nv = new NumberValidator(precision, scale, onlyPositive);
			nv.IsValidNumber(number).Should().BeFalse();
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
				throw new ArgumentException("scale must be a non-negative number less or equal than precision");
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
