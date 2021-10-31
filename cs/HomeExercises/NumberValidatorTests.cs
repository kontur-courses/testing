using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using FluentAssertions.Execution;
using NUnit.Framework;

namespace HomeExercises
{
	public class PlugException : Exception{}
	
	public class NumberValidatorTests
	{
		// 1 - пустая строка или null
		[TestCase(null, false, TestName = "validation test with null")]
		[TestCase("", false, TestName = "validation test with empty string")]
		// [TestCase(" ", false)]
		
		// 2 - проверка на регур
		// [TestCase("a.sd", false)]
		[TestCase("1.1d", false, TestName = "validation test with non-number in fractional part")]
		[TestCase("d.1", false, TestName = "validation test with non-number in precision")]
		[TestCase("d1", false, TestName = "validation test with non-number")]
		
		[TestCase("1.1.1", false, TestName = "validation test with three dots")]
		[TestCase(".1", false, TestName = "validation test with dot but without precision")]
		[TestCase("1.", false, TestName = "validation test with dot but without fractional part")]
		[TestCase(".", false, TestName = "validation test with dot only")]
		
		[TestCase("--1.0", false, 17, 2, false, TestName = "validation test double sign")]
		[TestCase("1+1.0", false, 17, 2, false, TestName = "validation test with digit before sign")]
		[TestCase("1.0+", false, TestName = "validation test with sign in the end of number")]
		
		// 3 
		// проварка на общую длину
		[TestCase("-12.23", true, 5, 2, false, TestName = "number lenght validation test")] 
		[TestCase("-321.23", false, 5, 2, false, TestName = "number lenght validation test")] 
		[TestCase("21.23", true, 5, 2, false, TestName = "number lenght validation test")] 
		
		// проверка на длину дробной части
		[TestCase("+1.234", false, 55, 2, false, TestName = "fractional part lenght validation test")]
		[TestCase("+1.234", true, 55, 3, false, TestName = "fractional part lenght validation test")]
		[TestCase("+1.23", true, 55, 2, false, TestName = "fractional part lenght validation test")]
		
		// 4 - проверка на знак
		[TestCase("-2.1", false, 10, 5, true, TestName = "sign validation test")]
		[TestCase("+2.1", true, 10, 5, true, TestName = "sign validation test")]
		[TestCase("-2.1", true, 10, 5, false, TestName = "sign validation test")]
		[TestCase("+2.1", true, 10, 5, false, TestName = "sign validation test")]
		
		[TestCase("2,1", true, TestName = "comma validation test")]
		public void Test_IsValidNumber(
			string numberForCheck,
			bool expected, 
			int precision = 17, 
			int scale = 2, 
			bool onlyPositive = true)
		{
			var message = $"Number {numberForCheck} with parameters: " +
			              $"\n\tprecision = {precision}, " +
			              $"\n\tscale = {scale}, " +
			              $"\n\tonlyPositive = {onlyPositive}" +
			              $"\n\n  IsValidNumber result";

			var builder = TestNumberValidatorBuilder.AValidator()
				.WithPrecision(precision)
				.WithScale(scale)
				.WithSignMode(onlyPositive);
			var validator = builder.Build();
			
			var firstCallActual = validator.IsValidNumber(numberForCheck);
			var secondCallActual = validator.IsValidNumber(numberForCheck);
			var secondConstructorCallActual = builder.Build().IsValidNumber(numberForCheck);
			
			if (expected)
			{
				Assert.That(firstCallActual, Is.True, message);
				Assert.That(secondCallActual, Is.True, $"Exception on double call\n{message}");
				Assert.That(secondConstructorCallActual, Is.True, $"Exception on double NumberValidator constructor call\n{message}");
			}
			else
			{
				Assert.That(firstCallActual, Is.False, message);
				Assert.That(secondCallActual, Is.False, $"Exception on double call\n{message}");
				Assert.That(secondConstructorCallActual, Is.False, $"Exception on double NumberValidator constructor call\n{message}");
			}
		}

		[TestCase(-1, 0, 
			"*precision must be a positive*", 
			TestName = "NumberValidator constructor test with precision = -1 and scale = 0")]
		[TestCase(0, 0,
			"*precision must be a positive*", 
			TestName = "NumberValidator constructor test with precision = 0 and scale = 0")]
		[TestCase(2, -1, 
			"*precision must be a non-negative number less or equal than precision*", 
			TestName = "NumberValidator constructor test with precision = 2 and scale = -1")]
		[TestCase(2, 2, 
			"*precision must be a non-negative number less or equal than precision*", 
			TestName = "NumberValidator constructor test with precision = 2 and scale = 2")]
		[TestCase(2, 3, 
			"*precision must be a non-negative number less or equal than precision*", 
			TestName = "NumberValidator constructor test with precision = 2 and scale = 3")]
		public void NumberValidatorConstructor_ThrowArgumentException_IncorrectPrecisionOrScale(
			int precision, 
			int scale, 
			string requiredMessage)
		{
			var builder1 = TestNumberValidatorBuilder
				.AValidator()
				.WithPrecision(precision)
				.WithScale(scale)
				.WithSignMode(false);
			var builder2 = builder1
				.But()
				.WithSignMode(true);
			
			Action action1 = () => builder1.Build();
			Action action2 = () => builder2.Build();

			using (new AssertionScope())
			{
				action1.Should().ThrowExactly<ArgumentException>().WithMessage(requiredMessage);
				action1.Should().ThrowExactly<ArgumentException>().WithMessage(requiredMessage);
				action2.Should().ThrowExactly<ArgumentException>().WithMessage(requiredMessage);
				action2.Should().ThrowExactly<ArgumentException>().WithMessage(requiredMessage);
			}
		}
		
		[TestCase(1, 0, TestName = "NumberValidator constructor test with precision = 1 and scale = 0")]
		[TestCase(2, 0, TestName = "NumberValidator constructor test with precision = 2 and scale = 0")]
		[TestCase(2, 1, TestName = "NumberValidator constructor test with precision = 2 and scale = 1")]
		public void NumberValidatorConstructor_DoesNotThrow_CorrectPrecisionAndScale(int precision, int scale)
		{
			var builder1 = TestNumberValidatorBuilder
				.AValidator()
				.WithPrecision(precision)
				.WithScale(scale)
				.WithSignMode(false);
			var builder2 = builder1
				.But()
				.WithSignMode(true);
			
			Action action1 = () => builder1.Build();
			Action action2 = () => builder2.Build();
			
			action1.Should().NotThrow();
			action1.Should().NotThrow();
			action2.Should().NotThrow();
			action2.Should().NotThrow();
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
		private  Regex numberRegex;
		private  bool onlyPositive;
		private  int precision;
		private  int scale;

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