using System;
using System.Runtime.InteropServices.ComTypes;
using System.Text.RegularExpressions;
using FluentAssertions;
using Microsoft.VisualStudio.TestPlatform.Common.Interfaces;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[TestCase(-1, 2, TestName = "Throw Exception on negative Precision")]
		[TestCase(2, -1, TestName = "Throw Exception on negative Scale ")]
		[TestCase(2, 3, TestName = "Throw Exception if Scale is greater than Precision ")]
		[TestCase(1, 1, TestName = "Throw Exception if Scale is equal Precision ")]
		[TestCase(0, 1, TestName = "Throw Exception on zero Precision ")]
		public void NumberValidator_IncorrectValues_ThrowException(int precision, int scale)
		{
			Action a = () => new NumberValidator(precision, scale);

			a.Should().Throw<ArgumentException>();
		}

		[TestCase(1, 0, TestName = "Don't throw Exception on correct values")]
		public void NumberValidator_CorrectValue_NotThrowException(int precision, int scale)
		{
			Action a = () => new NumberValidator(precision, scale);

			a.Should().NotThrow<ArgumentException>();
		}

		[TestCase(17, 2, true, "0.0", TestName = "Return True on correct values")]
		[TestCase(5, 2, true, "+12.34", TestName = "Positive float wit sign")]
		[TestCase(5, 2, false, "-12.34", TestName = "Negative float wit sign")]
		[TestCase(5, 0, true, "12345", TestName = "Integer")]
		[TestCase(5, 2, true, "123,45", TestName = "Comma separated float")]
		public void NumberValidator_CorrectValue_ShouldReturnTrue(int precision, int scale, bool onlyPositive,
			string number)
		{
			new NumberValidator(precision, scale, onlyPositive).IsValidNumber(number).Should().Be(true);
		}

		[TestCase(3, 2, "a.sd", TestName = "Return False on incorrect number")]
		[TestCase(3, 2, "", TestName = "Return False on empty string")]
		[TestCase(3, 2, null, TestName = "Return False on null string")]
		[TestCase(3, 2, "-1.23", TestName = "Return False on negative number if only positive allowed")]
		[TestCase(4, 2, "1.111", TestName = "Return False if fraction is longer than Scale")]
		[TestCase(4, 2, "111.11", TestName = "Return False if number length is greater than Precision")]
		[TestCase(3, 2, "+1.23", TestName = "Return False if number length with sign is greater than Precision")]

		public void NumberValidator_IncorrectValue_ShouldReturnFalse(int precision, int scale, string number)
		{
			new NumberValidator(precision, scale, true).IsValidNumber(number).Should().Be(false);
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