using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{

		[TestCase(-1, 1, TestName = "less than zero")]
		[TestCase(0, 1, TestName = "equal to zero")]
		public void Constructor_ShouldThrowArgumentException_WhenPrecisionNotPositive
			(int precision, int scale)
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(precision, scale),
				"precision must be a positive number");
		}

		[TestCase(1, -1, TestName = "less than zero")]
		[TestCase(1, 1, TestName = "equal to presicion")]
		[TestCase(1, 2, TestName = "grater than presicion")]
		public void Constructor_ShouldThrowArgumentException_WhenScaleIncorrect
			(int precision, int scale)
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(precision, scale),
				"precision must be a non-negative number less or equal than precision");
		}

		[TestCase(1, 0, TestName = "when scale equal to 0")]
		[TestCase(10, 5, false, TestName = "when passed onlyPositive argument")]
		public void Constructor_ShouldWorkCorrectly_When(int precision, int scale, 
				bool onlyPositive = false) =>
			// ReSharper disable once ObjectCreationAsStatement
			new NumberValidator(precision, scale, onlyPositive);


		[TestCase(3, 2, true, "00.00", TestName = "scale exceeded")]
		[TestCase(4, 2, true, "-1.23", TestName = "onlyPositive set as true, but not positive number")]
		[TestCase(17, 2, true,"0.000", TestName = "precision exceeded")]
		[TestCase(3, 2, true, "a.sd", TestName = "not a number")]
		[TestCase(7, 5, true, "", TestName = "value argument is empty")]
		[TestCase(10, 6, true, null, TestName = "value argument is null")]
		public void IsValidNumber_ShouldBeFalse_When(int presicion, int scale, bool onlyPositive,
			string value) =>
			new NumberValidator(presicion, scale, onlyPositive)
				.IsValidNumber(value).Should().BeFalse();

		[TestCase(17, 2, true, "0.0", TestName = "number is null")]
		[TestCase(4, 2, true, "+1.23", TestName = "onlyPositive set as true and number positive")]
		public void IsValidNumber_ShouldBeTrue_When(int presicion, int scale, bool onlyPositive,
			string value) =>
			new NumberValidator(presicion, scale, onlyPositive)
				.IsValidNumber(value).Should().BeTrue();
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