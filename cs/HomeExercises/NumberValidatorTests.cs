using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	[TestFixture]
	public class NumberValidatorTests
	{
		private static NumberValidator numberValidatorOnlyPositive = null!;
		private static NumberValidator numberValidatorNegative = null!;

		[SetUp]
		public void CreateNumberValidators()
		{
			numberValidatorOnlyPositive = new NumberValidator(3, 2, true);
			numberValidatorNegative = new NumberValidator(3, 2);
		}
		[TestCase(1,0, null)]
		[TestCase(1,0, false)]
		[TestCase(1,0, true)]
		[TestCase(1000,54, null)]
		[TestCase(9999,8768, false)]
		[TestCase(112312,2, true)]
		public void NumberValidatorCreatesCorrectly_WithValidArguments(int precision, int scale, bool? onlyPositive)
		{
			Action action;
			if (onlyPositive is null)
				action = () => new NumberValidator(precision, scale);
			else
				action = () => new NumberValidator(precision, scale, (bool)onlyPositive);
			action.Should().NotThrow();
		}
		
		[TestCase(-1, 2, TestName = "Negative precision")]
		[TestCase(1, -1, TestName = "Negative scale")]
		[TestCase(2, 3, TestName = "Scale grater than precision")]
		public void ThrowException_WithInvalidArguments(int precision, int scale)
		{
			var action = new Action(() => new NumberValidator(precision, scale, true));
			action.Should().Throw<ArgumentException>();
		}
		
		[TestCase("1.0", true)]
		[TestCase("0.15", true)]
		[TestCase("+0.2", true)]
		[TestCase("-0.04", false)]
		[TestCase("+1.2", true)]
		[TestCase("-1.2", false)]
		public void ValidatesCorrectly_WithOnlyPositive(string number, bool expected)
		{
			numberValidatorOnlyPositive.IsValidNumber(number).Should().Be(expected);
		}
		
		[TestCase("1.0", true)]
		[TestCase("-0.1", true)]
		[TestCase("+0.2", true)]
		[TestCase("-0.04", false)]
		[TestCase("+1.2", true)]
		[TestCase("-1.2", true)]
		public void ValidatesCorrectly_WithNegative(string number, bool expected)
		{
			numberValidatorNegative.IsValidNumber(number).Should().Be(expected);
		}
		
		[TestCase("a.sd")]
		[TestCase("")]
		[TestCase(null)]
		[TestCase("?0.1")]
		[TestCase("10.")]
		[TestCase(".12")]
		[TestCase(" 0")]
		[TestCase("0 ")]
		public void ReturnsFalse_WithNotNumbers(string number)
		{
			numberValidatorOnlyPositive.IsValidNumber(number).Should().Be(false);
			numberValidatorNegative.IsValidNumber(number).Should().Be(false);
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
}