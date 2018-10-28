using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[TestFixture]
		public class NumberValidator_Should
		{
			[Test]
			[TestCase(-1, 2, TestName = "Precision is negative")]
			[TestCase(2, -2, TestName = "Scale is negative")]
			[TestCase(2, 3, TestName = "Scale is more than precision")]
			[TestCase(2, 2, TestName = "Scale equals precision")]
			public void ThrowArgumentException_When(int precision, int scale)
			{
				Action action = () => new NumberValidator(precision, scale);
				action.Should().Throw<ArgumentException>();
            }

			[Test]
			[TestCase(17, 2, null, TestName = "Input is null")]
			[TestCase(17, 2, "", TestName = "Input is Empty")]
            public void NotThrowArgumentException_When(int precision, int scale, string input)
			{
				Action action = () => new NumberValidator(precision, scale).IsValidNumber(input);
				action.Should().NotThrow<ArgumentException>();
            }
		}

        [TestFixture]
		public class IsValidNumber_Should
        {
			[Test]
			[TestCase("0.0", TestName = "Precision is more than digits")]
			[TestCase("0", TestName = "Scale has no digits")]
			[TestCase("+1.23", TestName = "Has plus sign")]
			[TestCase("-1.23", TestName = "Has minus sign")]
	        [TestCase("0,12", TestName = "Has only fractal part")]
			[TestCase("0,00", TestName = "Has comma as delimiter")]
			public void BeTrue_WhenInput(string input)
			{
				new NumberValidator(4, 2).IsValidNumber(input).Should().BeTrue();
            }

	        [Test]
	        [TestCase("00.00", TestName = "Precision is less than digits")]
	        [TestCase("0.000", TestName = "Scale is less than digits")]
	        [TestCase("a.sd", TestName = "Non numeric input")]
	        [TestCase("", TestName = "Is empty string")]
	        [TestCase(null, TestName = "Is null")]
	        [TestCase("-5", true, TestName = "Has minus with onlypositive flag being set")]
	        [TestCase("+-0,00", TestName = "Starts with several signs")]
            public void BeFalse_WhenInput(string input, bool onlyPositive = false)
	        {
		        new NumberValidator(3, 2, onlyPositive).IsValidNumber(input).Should().BeFalse();
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
}