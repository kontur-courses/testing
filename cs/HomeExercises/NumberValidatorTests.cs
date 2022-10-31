using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using FluentAssertions.Execution;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[TestCase(1, 0, TestName = "Minimum value of precision and scale")]
		[TestCase(20, 10, TestName = "Simple value of precision and scale")]
		[TestCase(int.MaxValue, int.MaxValue - 1, TestName = "Maximum value of precision and scale")]
		public void NumberValidator_CorrectCreation_DoesNotThrowException(int precision, int scale)
		{
			Action validatorCreation = () => new NumberValidator(precision, scale, false);
			Action positiveNumberValidatorCreation = () => new NumberValidator(precision, scale, true);
			
			using (new AssertionScope())
			{
				var message = $"Precision = {precision} or scale = {scale} is correct";
				validatorCreation.Should().NotThrow(message);
				positiveNumberValidatorCreation.Should().NotThrow(message);
			}
		}

		[TestCase(-1, 0, TestName = "NegativePrecision")]
		[TestCase(0, 0, TestName = "ZeroPrecision")]
		[TestCase(2, -1, TestName = "NegativeScale")]
		[TestCase(10, 10, TestName = "ScaleEqualPrecision")]
		[TestCase(15, 16, TestName = "ScaleGreaterThanPrecision")]
		public void NumberValidator_IncorrectCreation_ShouldThrowException(int precision, int scale)
		{
			Action validatorCreation = () => new NumberValidator(precision, scale, false);
			Action positiveNumberValidatorCreation = () => new NumberValidator(precision, scale, true);

			using (new AssertionScope())
			{
				var message = $"Precision = {precision} or scale = {scale} is incorrect";
				validatorCreation.Should().Throw<ArgumentException>(message);
				positiveNumberValidatorCreation.Should().Throw<ArgumentException>(message);
			}
		}

		[TestCase(null, TestName = "Null")]
		[TestCase("", TestName = "EmptyString")]
		[TestCase("       ", TestName = "Spaces")]
		[TestCase("\r\n", TestName = "NewLineWindows")]
		[TestCase("\n", TestName = "NewLineLinux")]
		[TestCase("qwerty", TestName = "Text")]
		[TestCase("FF.AA", TestName = "HexNumber")]
		public void IsValidNumber_IncorrectValueFormat_ShouldBeFalse(string value)
		{
			var validator = new NumberValidator(2, 1, true);

			validator.IsValidNumber(value).Should().BeFalse($"Value = '{value}' has wrong format ");
		}

		[TestCase("-0.00", TestName = "NegativeSign")]
		[TestCase("+0.00", TestName = "PositiveSign")]
		public void IsValidNumber_NumberWithSignGreaterThanPrecision_ShouldBeFalse(string value)
		{
			const int precision = 3;
			const int scale = 2;
			var validator = new NumberValidator(precision, scale, true);
			var onlyPositiveValidator = new NumberValidator(precision, scale, false);

			using (new AssertionScope())
			{
				var message = $"Precision = {precision} was less than count of digits + sign in string = {value}";

				validator.IsValidNumber(value).Should().BeFalse(message);
				onlyPositiveValidator.IsValidNumber(value).Should().BeFalse(message);
			}
		}

		[TestCase(2, 1, "1.1", TestName = "PointSeparator")]
		[TestCase(2, 1, "1,1", TestName = "CommaSeparator")]
		public void IsValidNumber_CorrectSeparator_ShouldBeTrue(int precision, int scale, string value)
		{
			var validator = new NumberValidator(precision, scale, true);
			var onlyPositiveValidator = new NumberValidator(precision, scale, false);

			using (new AssertionScope())
			{
				var message = $"Value = '{value}' satisfies a separator rules";

				validator.IsValidNumber(value).Should().BeTrue(message);
				onlyPositiveValidator.IsValidNumber(value).Should().BeTrue(message);
			}
		}

		[TestCase(4, 2, "0.", TestName = "WithoutFracPartButContainsPoint")]
		[TestCase(4, 2, "0,", TestName = "WithoutFracPartButContainsComma")]
		[TestCase(20, 10, "1,1.1", TestName = "TwoDifferentSeparators")]
		[TestCase(20, 10, "1.1.1", TestName = "TwoSameSeparators")]
		public void IsValidNumber_IncorrectSeparator_ShouldBeFalse(int precision, int scale, string value)
		{
			var validator = new NumberValidator(precision, scale, true);
			var onlyPositiveValidator = new NumberValidator(precision, scale, false);

			using (new AssertionScope())
			{
				var message = $"Value = '{value}' doesn't satisfy a separator rules";

				validator.IsValidNumber(value).Should().BeFalse(message);
				onlyPositiveValidator.IsValidNumber(value).Should().BeFalse(message);
			}
		}

		[TestCase(4, 2, true, "0", TestName = "WithoutFracPart")]
		[TestCase(4, 2, true, "000.0", TestName = "FracPartLessThanScale")]
		[TestCase(40, 1, true, "111.1", TestName = "NumberLessThanPrecision")]
		[TestCase(4, 2, false, "+1.23", TestName = "PositiveNumberWithoutOnlyPositiveFlag")]
		[TestCase(5, 2, true, "12.34", TestName = "FracPartEqualsScale")]
		[TestCase(4, 2, true, "123.4", TestName = "NumberEqualsPrecision")]
		public void IsValidNumber_CorrectValues_ShouldBeTrue(int precision, int scale, bool onlyPositive, string value)
		{
			var validator = new NumberValidator(precision, scale, onlyPositive);

			validator.IsValidNumber(value)
				.Should().BeTrue($"Value = '{value}' satisfies precision = {precision} and scale = {scale}");
		}

		[TestCase(4, 2, true, ".0", TestName = "WithoutIntPart")]
		[TestCase(4, 2, true, "0.000", TestName = "FracPartGreaterThanScale")]
		[TestCase(3, 0, true, "1111", TestName = "NumberLengthGreaterThanPrecision")]
		[TestCase(4, 2, true, "-1.23", TestName = "NegativeNumberWithOnlyPositiveFlag")]
		public void IsValidNumber_IncorrectValues_ShouldBeFalse(int precision, int scale, bool onlyPositive, string value)
		{
			var validator = new NumberValidator(precision, scale, onlyPositive);

			validator.IsValidNumber(value)
				.Should().BeFalse($"Value = '{value}' doesn't satisfy precision = {precision} or scale = {scale}");
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
				throw new ArgumentException("scale must be a non-negative number less than precision");
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