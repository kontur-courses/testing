using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[TestCase(-8, 2, true, TestName = "When Precision Is Negative")]
		[TestCase(1, 3, true, TestName = "When Scale Larger Than Precision")]
		public void Constructor_ShouldThrowException(int precision, int scale, bool onlyPositive)
		{
			Action constructor = (() => new NumberValidator(precision, scale, onlyPositive));
			constructor.ShouldThrow<ArgumentException>();
		}

		[Test]
		public void Constructor_ShouldCreateValidator_WhenArgsIsCorrect()
		{
			Action constructor = (() => new NumberValidator(2, 1, false));
			constructor.ShouldNotThrow<ArgumentException>();
		}


		[TestCase(15, 8, true, "4.24242", TestName = "when number is positive")]
		[TestCase(8, 2, true, "+122,2", TestName = "when number is positive with plus")]
		[TestCase(10, 7, false, "-911.32", TestName = "when number is negative with minus")]
		[TestCase(3, 1, false, "27.1", TestName = "when max count of digit")]
		[TestCase(10, 9, false, "0.3101010", TestName = "when number less than 1")]
		[TestCase(10, 9, false, "2048", TestName = "when number without fraction part")]
		[TestCase(10, 7, false, "42,24", TestName = "when separator is coma")]
		public void ShouldBeValid(int precision, int scale, bool onlyPositive, string value)
		{
			(new NumberValidator(precision, scale, onlyPositive)).IsValidNumber(value).Should().BeTrue(
				"[precision: {0}, scale: {1}, onlyPositive: {2}, value: {3}]", precision, scale, onlyPositive, value);
		}

		
		[TestCase(3, 2, true, "a.sd", TestName = "when wrong symbols in value")]
		[TestCase(3, 1, true, ".0", TestName = "when empty int part")]
		[TestCase(3, 1, true, "0.", TestName = "when empty fraction part")]
		[TestCase(17, 2, true, "", TestName = "when value is empty")]
		[TestCase(17, 2, true, null, TestName = "when value is null")]
		[TestCase(17, 2, true, ".", TestName = "when only separator")]
		[TestCase(3, 2, true, "00.00", TestName = "when Precision Overflow")]
		[TestCase(5, 2, true, "9.111",  TestName = "when Scale Overflow")]
		[TestCase(3, 2, true, "-0.2", TestName = "when Negative value and only-positive flag")]
		public void ShouldBeInvalid(int precision, int scale, bool onlyPositive, string value)
		{
			(new NumberValidator(precision, scale, onlyPositive)).IsValidNumber(value).Should().BeFalse( 
				"[precision: {0}, scale: {1}, onlyPositive: {2}, value: {3}]", precision, scale, onlyPositive, value);
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