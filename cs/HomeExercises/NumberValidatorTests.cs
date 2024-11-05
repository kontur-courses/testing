using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[TestCaseSource(nameof(ValidationExceptionCases))]
		public void ValidationExceptionTest(int precision, int scale, bool onlyPositive, bool shouldThrow)
		{
			// Action action = () => new NumberValidator(precision, scale, onlyPositive);
			// action.Should().Throw<ArgumentException>().When(shouldThrow); When не доступно в старой версии либы(
			Action action = () => new NumberValidator(precision, scale, onlyPositive);
			if (shouldThrow)
			{
				action.Should().Throw<ArgumentException>();
			}
			else
			{
				action.Should().NotThrow();
			}
			
		}
		public static object[] ValidationExceptionCases =
		{
			new object[] { 1, 0, true, false },
			new object[] { -1, 2, false, true },
			new object[] { 2, 3, true, true }
		};

		[TestCaseSource(nameof(CorrectValidationCases))]
		public void CorrectValidationTest(int precision, int scale, bool onlyPositive, string inputCase, bool expected)
		{
			var validator = new NumberValidator(precision, scale, onlyPositive);
			var isValid = validator.IsValidNumber(inputCase);
			isValid.Should().Be(expected);
		}

		public static object[] CorrectValidationCases =
		{
			new object[] { 17, 2, true, "0", true },
			new object[] { 3, 2, true, "00.00", false },
			new object[] { 3, 2, true, "-0.00", false },
			new object[] { 17, 2, true, "0.0", true },
			new object[] { 3, 2, true, "+0.00", false },
			new object[] { 4, 2, true, "+1.23", true },
			new object[] { 3, 2, true, "+1.23", false },
			new object[] { 17, 2, true, "0.000", false },
			new object[] { 3, 2, true, "-1.23", false },
			new object[] { 3, 2, true, "a.sd", false },
			new object[] { 1, 0, true, "0", true },
			new object[] { 1, 0, false, "-0", false },
		};
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