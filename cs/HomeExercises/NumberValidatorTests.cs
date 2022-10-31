using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Running;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorIsValidNumberBenchmark
	{
		private readonly NumberValidator numberValidator = new NumberValidator(17, 2);
		private readonly string number;

		public NumberValidatorIsValidNumberBenchmark()
		{
			number =
				((new Random(20221031).NextDouble() - 0.5) * 1000000000000000).ToString(CultureInfo.InvariantCulture);
		}


		[Benchmark()]
		public void NumberValidator() => numberValidator.IsValidNumber(number);
	}

	public class NumberValidatorTests
	{
		[Test]
		[Category("Benchmark")]
		public void IsValidNumber_RunsLessThan800Nanoseconds_Benchmark()
		{
			//Arrange & Act 
			var summary = BenchmarkRunner.Run<NumberValidatorIsValidNumberBenchmark>();
			
			//Assert
			var actual = summary.Reports.First().AllMeasurements
				.Where(x => x.IterationMode == IterationMode.Workload && x.IterationStage == IterationStage.Actual)
				.Average(x => x.GetAverageTime().Nanoseconds);
			actual.Should().BeLessThan(800);
		}

		[TestCase(17, 2, true, "1.2.3", Description = "number is three digits separated by dots")]
		[TestCase(17, 2, true, "1,2,3", Description = "number is three digits separated by commas")]
		[TestCase(17, 2, true, "1,", Description = "there aren`t digits after integer part and comma")]
		[TestCase(17, 2, false, "+-1", Description = "number contains two signs")]
		[TestCase(17, 2, true, " 0.00", Description = "number contains valid number and starting space")]
		[TestCase(17, 2, true, "0.00 ", Description = "number contains valid number and ending space")]
		[TestCase(17, 2, false, "- 0.00",
			Description = "number contains valid number and space between minus sign and number")]
		[TestCase(17, 2, true, "0.000", Description = "number scale greater than specified scale")]
		[TestCase(17, 2, true, "0000000000000000000",
			Description = "count of digits in number greater than specified precision")]
		[TestCase(17, 0, true, "00000000000000000.0",
			Description = "specified digit after dot when scale is 0")]
		[TestCase(17, 2, true, "-1", Description = "number is negative when accepted only positive numbers")]
		[TestCase(17, 2, true, "одна целая и пять десятых", Description = "number is not match regular number format")]
		[TestCase(17, 2, true, "", Description = "number is empty string")]
		[TestCase(17, 2, true, "                   ", Description = "number contains only white spaces")]
		[TestCase(17, 2, true, "\0\0\0\0\0\0\0\0\0", Description = "number contains only zero symbols")]
		public void IsValidNumber_ShouldReturnsFalse_WhenCallingWithTheseParameters(int precision, int scale,
			bool onlyPositive, string number)
		{
			//Arrange
			var numberValidator = new NumberValidator(precision, scale, onlyPositive);

			//Act
			var actual = numberValidator.IsValidNumber(number);

			//Assert
			actual.Should().BeFalse();
		}


		[TestCase(17, 2, true, "0.00")]
		[TestCase(17, 2, true, "0")]
		[TestCase(17, 2, true, "00000000000000000")]
		[TestCase(17, 2, true, "0,01")]
		[TestCase(4, 2, true, "+1.23")]
		[TestCase(4, 2, false, "-1.23")]
		public void IsValidNumber_ShouldReturnsTrue_WhenCallingWithTheseParameters(int precision, int scale,
			bool onlyPositive, string number)
		{
			//Arrange
			var numberValidator = new NumberValidator(precision, scale, onlyPositive);

			//Act
			var actual = numberValidator.IsValidNumber(number);

			//Assert
			actual.Should().BeTrue();
		}

		[TestCase(1, 1, false, Description = "scale equal to precision")]
		[TestCase(1, 2, false, Description = "scale greater than precision")]
		[TestCase(1, -1, false, Description = "scale is negative")]
		[TestCase(-1, 1, false, Description = "precision is negative")]
		[TestCase(0, 0, false, Description = "precision is zero")]
		public void Constructor_ShouldThrows_WhenCallingWithTheseParameters(int precision, int scale, bool onlyPositive)
		{
			//Arrange
			Action action = () => _ = new NumberValidator(precision, scale, onlyPositive);

			//Act & Assert
			action.Should().Throw<ArgumentException>();
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