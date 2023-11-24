using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[Test]
		public void Test()
		{
			Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("-0.00"));
			Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("+0.00"));
			Assert.IsTrue(new NumberValidator(4, 2, true).IsValidNumber("+1.23"));
			Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("+1.23"));
			Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("-1.23"));
			Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("a.sd"));
		}

		[Test]
		public void CreatesWithNoExceptions()
		{
			var creationOfValidator = new Action(() => { new NumberValidator(1, 0, true); });
			creationOfValidator.Should().NotThrow();
		}

		[Test]
		public void ShouldThrowException_WhenNegativePrecision()
		{
			const int precision = -1;
			var creationOfValidatorOnlyPositive = new Action(() => { new NumberValidator(precision, -1, true); });
			creationOfValidatorOnlyPositive.Should().Throw<ArgumentException>($"precision = {precision} is less than 0");

			var creationOfValidator = new Action(() => { new NumberValidator(precision, 2, false); });
			creationOfValidator.Should().Throw<ArgumentException>($"precision = {precision} is less than 0");
		}

		[Test]
		public void ShouldThrowException_WhenScaleIsNegative()
		{
			const int scale = -1;
			var creationOfValidatorOnlyPositive = new Action(() => { new NumberValidator(5, scale, true); });
			creationOfValidatorOnlyPositive.Should().Throw<ArgumentException>($"scale = {scale} is less than 0");

			var creationOfValidator = new Action(() => { new NumberValidator(5, scale, false); });
			creationOfValidator.Should().Throw<ArgumentException>($"scale = {scale} is less than 0");
		}

		[TestCase(5, 6)]
		[TestCase(5, 5)]
		public void ShouldThrowException_WhenScaleIsNotLessThanPrecision(int precision, int scale)
		{
			var creationOfValidatorOnlyPositive = new Action(() => { new NumberValidator(precision, scale, true); });
			creationOfValidatorOnlyPositive.Should().Throw<ArgumentException>($"scale = {scale} is bigger or equal to precision = {precision}");

			var creationOfValidator = new Action(() => { new NumberValidator(precision, scale, false); });
			creationOfValidator.Should().Throw<ArgumentException>($"scale = {scale} is bigger or equal to precision = {precision}");
		}

		[TestCase("12.34", true)]
		[TestCase("12", true)]
		[TestCase("1.000", false)]
		[TestCase("123.000", false)]
		public void ShouldValidateNumbersWithoutSign(string inputValue, bool expected)
		{
			var validator = new NumberValidator(5, 2, true);
			validator.IsValidNumber(inputValue).Should().Be(expected, $"number {inputValue} is {(expected ? "correct" : "incorrect")}");
		}

		[Test]
		public void ShouldNotValidateNull()
		{
			var validator = new NumberValidator(5, 2, true);
			validator.IsValidNumber(null!).Should().Be(false, "null should be false");
		}		
		
		[Test]
		public void ShouldNotValidateEmpty()
		{
			var validator = new NumberValidator(5, 2, true);
			validator.IsValidNumber("").Should().Be(false, "empty should be false");
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