using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;
using static FluentAssertions.FluentActions;


namespace HomeExercises
{
	public class NumberValidatorTests
	{
		// 1 - пустая строка или null
		[TestCase(null, false, 1, 0, false, TestName = "validation test with null")]
		[TestCase("", false, 1, 0, false, TestName = "validation test with empty string")]
		
		// 2 - проверка на регур
		[TestCase("1.1d", false, 4, 3, false, TestName = "validation test with non-number in fractional part")]
		[TestCase("d.1", false, 3, 1, false, TestName = "validation test with non-number in precision")]
		[TestCase("d1", false, 3, 1, false, TestName = "validation test with non-number")]
		
		[TestCase("1.1.1", false, 6, 5, false, TestName = "validation test with three dots")]
		[TestCase(".1", false, 3, 1, false, TestName = "validation test with dot but without precision")]
		[TestCase("1.", false, 3, 1, false, TestName = "validation test with dot but without fractional part")]
		[TestCase(".", false, 3, 1, false, TestName = "validation test with dot only")]
		
		[TestCase("--1.0", false, 4, 2, false, TestName = "validation test double sign")]
		[TestCase("1+1.0", false, 4, 2, false, TestName = "validation test with digit before sign")]
		[TestCase("1.0+", false, 3, 2, false, TestName = "validation test with sign in the end of number")]
		
		[TestCase("2,1", true, 2, 1, true, TestName = "comma validation test")]
		
		// 3 
		// проварка на общую длину
		[TestCase("-12.23", true, 5, 2, false, TestName = "number lenght validation test")] 
		[TestCase("-321.23", false, 5, 2, false, TestName = "number lenght validation test")] 
		[TestCase("21.23", true, 5, 2, false, TestName = "number lenght validation test")] 
		
		// проверка на длину дробной части
		[TestCase("+1.234", false, 5, 2, false, TestName = "fractional part lenght validation test")]
		[TestCase("+1.234", true, 5, 3, false, TestName = "fractional part lenght validation test")]
		[TestCase("+1.23", true, 5, 3, false, TestName = "fractional part lenght validation test")]
		
		// 4 - проверка на знак
		[TestCase("-2.1", false, 3, 1, true, TestName = "sign validation test")]
		[TestCase("+2.1", true, 3, 1, true, TestName = "sign validation test")]
		[TestCase("-2.1", true, 3, 1, false, TestName = "sign validation test")]
		[TestCase("+2.1", true, 3, 1, false, TestName = "sign validation test")]
		public void Test_IsValidNumber(
			string numberForCheck,
			bool expected, 
			int precision, 
			int scale, 
			bool onlyPositive)
		{
			var message = $"\nNumber \"{numberForCheck}\" with parameters: " +
			              $"\n\tprecision = {precision}, " +
			              $"\n\tscale = {scale}, " +
			              $"\n\tonlyPositive = {onlyPositive}\n";

			var validator = TestNumberValidatorBuilder.AValidator()
				.WithPrecision(precision)
				.WithScale(scale)
				.WithSignMode(onlyPositive)
				.Build();
			
			var actual = validator.IsValidNumber(numberForCheck);
			
			actual.Should().Be(expected, message);
		}

		[Test]
		public void NumberValidator_ShouldReturnSameResults_WhenTwoCallInARow()
		{
			var validator = TestNumberValidatorBuilder.AValidator()
				.WithPrecision(17)
				.WithScale(2)
				.Build();

			var firstCallResult = validator.IsValidNumber("1.234");
			var secondCallResult = validator.IsValidNumber("1.234");

			firstCallResult.Should().Be(secondCallResult, "method result should return same value with same parameters every time");
		}
		
		[Test]
		public void NumberValidator_ShouldReturnSameResults_AfterCreateAnotherValidator()
		{
			var builder = TestNumberValidatorBuilder.AValidator()
				.WithPrecision(17)
				.WithScale(3);

			var validator = builder.Build();
			var firstCallResult = validator.IsValidNumber("1.234");
			builder
				.But()
				.WithScale(2)
				.Build();
			var secondCallResult = validator.IsValidNumber("1.234");
			
			firstCallResult.Should().Be(secondCallResult, "method result should return same value after another validator creating");
		}

		[TestCase(-1, 0, TestName = "NumberValidator constructor Throw ArgumentException with precision = -1, scale = 0")]
		[TestCase(0, 0, TestName = "NumberValidator constructor Throw ArgumentException with precision = 0, scale = 0")]
		[TestCase(2, -1, TestName = "NumberValidator constructor Throw ArgumentException with precision = 2, scale = -1")]
		[TestCase(2, 2, TestName = "NumberValidator constructor Throw ArgumentException with precision = 2, scale = 2")]
		[TestCase(2, 3, TestName = "NumberValidator constructor Throw ArgumentException with precision = 2, scale = 3")]
		public void NumberValidatorConstructor_ThrowArgumentException_WhenIncorrectArguments(int precision, int scale)
		{
			var builderWithOnlyPositive = TestNumberValidatorBuilder
				.AValidator()
				.WithPrecision(precision)
				.WithScale(scale)
				.WithSignMode(true);

			var builderWithoutOnlyPositive = builderWithOnlyPositive
				.But()
				.WithSignMode(false);

			Invoking(() => builderWithOnlyPositive.Build()).Should().Throw<ArgumentException>();
			Invoking(() => builderWithoutOnlyPositive.Build()).Should().Throw<ArgumentException>();
		}
		
		[TestCase(1, 0, TestName = "NumberValidator constructor NotThrow with precision = 1 and scale = 0")]
		[TestCase(2, 0, TestName = "NumberValidator constructor NotThrow with precision = 2 and scale = 0")]
		[TestCase(2, 1, TestName = "NumberValidator constructor NotThrow with precision = 2 and scale = 1")]
		public void NumberValidatorConstructor_NotThrow_WhenCorrectArguments(int precision, int scale)
		{
			var builderWithOnlyPositive = TestNumberValidatorBuilder
				.AValidator()
				.WithPrecision(precision)
				.WithScale(scale)
				.WithSignMode(true);

			var builderWithoutOnlyPositive = builderWithOnlyPositive
				.But()
				.WithSignMode(false);

			Invoking(() => builderWithOnlyPositive.Build()).Should().NotThrow();
			Invoking(() => builderWithoutOnlyPositive.Build()).Should().NotThrow();
		}
	}

	public class TestNumberValidatorBuilder
	{
		private int precision = 1;
		private int scale = 0;
		private bool onlyPositive;
		
		public static TestNumberValidatorBuilder AValidator()
		{
			return new TestNumberValidatorBuilder();
		}

		public TestNumberValidatorBuilder WithPrecision(int precision)
		{
			this.precision = precision;
			return this;
		}
		
		public TestNumberValidatorBuilder WithScale(int scale)
		{
			this.scale = scale;
			return this;
		}
		
		public TestNumberValidatorBuilder WithSignMode(bool onlyPositive)
		{
			this.onlyPositive = onlyPositive;
			return this;
		}

		public TestNumberValidatorBuilder But() => 
			AValidator()
				.WithSignMode(onlyPositive)
				.WithScale(scale)
				.WithPrecision(precision);
		
		public NumberValidator Build()
		{
			return new NumberValidator(precision, scale, onlyPositive);
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
			if (precision <=  0)
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