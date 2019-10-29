using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[Test]
		public void NumberValidator_ShouldThrowArgumentException_WhenPrecisionNotPositive()
		{
			Action action = () => new NumberValidator(-1, 2);

			action.Should().Throw<ArgumentException>();
		}

		[Test]
		public void NumberValidator_ShouldThrowArgumentException_WhenScaleIsNegative()
		{
			Action action = () => new NumberValidator(1, -2);

			action.Should().Throw<ArgumentException>();
        }

		[Test]
		public void NumberValidator_ShouldThrowArgumentException_WhenScaleMoreOrEqualToPrecision()
		{
			Action actionMore = () => new NumberValidator(1, 2);
			Action actionEqual = () => new NumberValidator(1, 1);

            actionMore.Should().Throw<ArgumentException>();
            actionEqual.Should().Throw<ArgumentException>();
        }

		[Test]
		public void NumberValidator_DoesNotThrowException_WhenPrecisionAndScaleCorrect()
		{
			Action action = () => new NumberValidator(4, 2);

			action.Should().NotThrow();
		}

		[Test]
		public void IsValidNumber_False_WhenNull()
		{
			TestNumberValidators.PositiveValidatorWithSmallPrecisionAndScaleTwo()
				.IsValidNumber(null)
				.Should()
				.BeFalse();
		}

		[Test]
		public void IsValidNumber_False_WhenEmpty()
		{
			TestNumberValidators.PositiveValidatorWithSmallPrecisionAndScaleTwo()
				.IsValidNumber("")
				.Should()
				.BeFalse();
		}

		[Test]
		public void IsValidNumber_False_WhenNotANumber()
		{
			TestNumberValidators.PositiveValidatorWithSmallPrecisionAndScaleTwo()
				.IsValidNumber("a.sd")
				.Should()
				.BeFalse();
		}

		[Test]
		public void IsValidNumber_False_WhenNumberMoreThanPrecision()
		{
			TestNumberValidators.PositiveValidatorWithSmallPrecisionAndScaleTwo()
				.IsValidNumber("00.00")
				.Should()
				.BeFalse();
		}

		[Test]
		public void IsValidNumber_False_WhenNumberWithSignMoreThanPrecision()
		{
			TestNumberValidators.PositiveValidatorWithSmallPrecisionAndScaleTwo()
				.IsValidNumber("+1.23")
				.Should()
				.BeFalse();
		}

        [Test]
		public void IsValidNumber_False_WhenFractionalPartMoreThanScale()
		{
			TestNumberValidators.PositiveValidatorWithBigPrecisionAndScaleTwo()
				.IsValidNumber("0.000");
		}

		[Test]
		public void IsValidNumber_False_WhenNegativeNumberWithOnlyPositiveFlag()
		{
			TestNumberValidators.PositiveValidatorWithBigPrecisionAndScaleTwo()
				.IsValidNumber("-1.23")
				.Should()
				.BeFalse();
		}

        [Test]
		public void IsValidNumber_True_OnFractionThatFitsPrecisionAndScale()
		{
			TestNumberValidators.PositiveValidatorWithBigPrecisionAndScaleTwo()
				.IsValidNumber("0.0")
				.Should()
				.BeTrue();
		}
		
		[Test]
		public void IsValidNumber_True_OnIntegerThatFitsPrecisionAndScale()
		{
			TestNumberValidators.PositiveValidatorWithBigPrecisionAndScaleTwo()
				.IsValidNumber("0")
				.Should()
				.BeTrue();
        }

		[Test]
		public void IsValidNumber_True_OnNumberWithSign()
		{
			TestNumberValidators.PositiveValidatorWithBigPrecisionAndScaleTwo()
				.IsValidNumber("+1.23")
				.Should()
				.BeTrue();
		}

		[Test]
		public void IsValidNumber_True_OnNumberWithNegativeSignWhenNotOnlyPositiveFlag()
		{
			TestNumberValidators.ValidatorWithBigPrecisionAndScaleTwo()
				.IsValidNumber("-1.23")
				.Should()
				.BeTrue();
		}


    }

	public class TestNumberValidators
	{
		public static NumberValidator PositiveValidatorWithBigPrecisionAndScaleTwo()
		{
			return new NumberValidator(17, 2, true);
		}

		public static NumberValidator ValidatorWithBigPrecisionAndScaleTwo()
		{
			return new NumberValidator(17, 2, false);
		}

        public static NumberValidator PositiveValidatorWithSmallPrecisionAndScaleTwo()
		{
			return new NumberValidator(3, 2, true);
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