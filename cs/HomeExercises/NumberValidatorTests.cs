using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		private static readonly NumberValidator standardNonNegativeValidator = new NumberValidator(10, 5, true);

		[TestCase(-10, TestName = "On negative precision")]
		[TestCase(0, TestName = "On zero precision")]
		[TestCase(10, -1, TestName = "On negative scale and correct precision")]
		[TestCase(10, 20, TestName = "When scale more then precision")]
		[TestCase(10, 10, TestName = "When scale equals precision")]
		public void Constructor_ShouldRaiseArgumentException(int precision, int scale = 0, bool onlyPositive = false)
		{
			Action constructorCall = () => new NumberValidator(precision, scale, onlyPositive);
			constructorCall.Should().Throw<ArgumentException>();
		}

		[TestCase(4, 0, TestName = "When precision is more then 0 and scale equals zero")]
		[TestCase(4, 2, TestName = "When precision is more then 0 and scale between zero and precision")]
		[TestCase(3, 2, false, TestName = "When precision equals scale+1 on validator allowing negative numbers")]
		public void Constructor_ShouldWorkProperly(int precision, int scale = 0, bool onlyPositive = false)
		{
			Action constructorCall = () => new NumberValidator(precision, scale, onlyPositive);
			constructorCall.Should().NotThrow();
		}

		[Test]
		public void IsValidNumber_FractionalAndIntPartEqualsPrecision_False()
		{
			standardNonNegativeValidator.IsValidNumber("12345.12345").Should().BeTrue();
		}

        [Test]
		public void IsValidNumber_FractionalAndIntPartsAreMoreThenPrecision_True()
		{
			standardNonNegativeValidator.IsValidNumber("12345.123456").Should().BeFalse();
		}

		[Test]
		public void IsValidNumber_NumberEndsWithZeros_True()
		{
			standardNonNegativeValidator.IsValidNumber("0.000").Should().BeTrue();
		}

		[Test]
		public void IsValidNumber_FractionalPartLengthEqualsScale_True()
		{
			standardNonNegativeValidator.IsValidNumber("12345.12345").Should().BeTrue();
		}

		[Test]
		public void IsValidNumber_FractionalPartLengthMoreThenScale_False()
		{
			standardNonNegativeValidator.IsValidNumber("12345.123456").Should().BeFalse();
		}

        [Test]
		public void IsValidNumber_NumberWithoutFractionalPart_True()
		{
			standardNonNegativeValidator.IsValidNumber("123").Should().BeTrue();
		}

		[Test]
		public void IsValidNumber_FractionalAndIntPartsConsistOnlyOfZeros_True()
		{
			standardNonNegativeValidator.IsValidNumber("000.000").Should().BeTrue();
		}

		[Test]
		public void IsValidNumber_NumberStartsWithZeros_True()
		{
			standardNonNegativeValidator.IsValidNumber("000.123").Should().BeTrue();
		}

		[Test]
		public void IsValidNumber_NegativeNumber_False_OnNonNegativeValidator()
		{
			standardNonNegativeValidator.IsValidNumber("-1.1").Should().BeFalse();
		}

		[Test]
		public void IsValidNumber_NullInput_False()
		{
			standardNonNegativeValidator.IsValidNumber(null).Should().BeFalse();
        }

		[Test]
		public void IsValidNumber_EmptyInput_False()
		{
			standardNonNegativeValidator.IsValidNumber("").Should().BeFalse();
        }

		[Test]
		public void IsValidNumber_IntNumberLengthEqualsPrecision_True()
		{
			standardNonNegativeValidator.IsValidNumber("1234567890").Should().BeTrue();
		}

		[Test]
		public void IsValidNumber_NumberLengthIncludingSignIsMoreThenPrecision_False()
		{
			const string number = "234567890.12";
			var currentValidator = new NumberValidator(11, 5);
			currentValidator.IsValidNumber('-' + number).Should().BeFalse();
			currentValidator.IsValidNumber('+' + number).Should().BeFalse();
		}

		[Test]
		public void IsValidNumber_NumberLengthIncludingSignEqualsPrecision_True()
		{
			const string number = "234567890.1";
			var currentValidator = new NumberValidator(11, 5);
			currentValidator.IsValidNumber('-' + number).Should().BeTrue();
			currentValidator.IsValidNumber('+' + number).Should().BeTrue();
		}

		[Test]
		public void IsValidNumber_NaN_False()
		{
			standardNonNegativeValidator.IsValidNumber("abc.def").Should().BeFalse();
		}

		[Test]
		public void IsValidNumber_SignedZero_True()
		{
			const string zero = "0.0";
			var currentValidator = new NumberValidator(11, 5);
			currentValidator.IsValidNumber('-' + zero).Should().BeTrue();
			currentValidator.IsValidNumber('+' + zero).Should().BeTrue();
		}

		[Test]
		public void IsValidNumber_NumberDividedByComma_True()
		{
			standardNonNegativeValidator.IsValidNumber("123,4").Should().BeTrue();
		}

		[Test]
		public void IsValidNumber_NumberDividedByDot_True()
		{
			standardNonNegativeValidator.IsValidNumber("123.4").Should().BeTrue();
		}

		[Test]
		public void IsValidNumber_NumberStartsWithDivider_False()
		{
			standardNonNegativeValidator.IsValidNumber(".123").Should().BeFalse();
		}

		[Test]
		public void IsValidNumber_NumberEndsWithDivider_False()
		{
			standardNonNegativeValidator.IsValidNumber("123.").Should().BeFalse();
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
				// Могу предположить, что в исходном варианте были опечатки:
	            // throw new ArgumentException("precision must be a non-negative number less or equal than precision");
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