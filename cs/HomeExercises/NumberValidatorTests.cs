using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	[TestFixture]
	public class NumberValidator_Constructor_Tests
	{
		[Test]
		public void Test_Throws_OnNegativePrecision()
		{
			Action action = () => new NumberValidator(-1, 2, true);
			action
				.ShouldThrow<ArgumentException>()
				.WithMessage("precision must be a positive number");
		}

		[Test]
        public void Test_Throw_WhenPrecisionLessThanScale()
		{
			Action action = () => new NumberValidator(1, 2, true);
			action
				.ShouldThrow<ArgumentException>()
				.WithMessage("precision must be a non-negative number less or equal than precision");
		}

        [Test]
        public void Test_Throws_WhenPrecisionIsEqualScale()
		{
			Action action = () => new NumberValidator(1, 1, true);
			action
				.ShouldThrow<ArgumentException>()
				.WithMessage("precision must be a non-negative number less or equal than precision");
		}

        [Test]
        public void Test_Throws_OnNegativeScale()
		{
			Action action = () => new NumberValidator(5, -1, true);
			action
				.ShouldThrow<ArgumentException>()
				.WithMessage("precision must be a non-negative number less or equal than precision"); ;
		}

        [Test]
        public void Test_NotThrow_WhenOnlyPositiveFlagIsTrue()
		{
			Action action = () => new NumberValidator(5, 2, true);
			action.ShouldNotThrow();

		}

        [Test]
        public void Test_NotThrow_WhenOnlyPositiveFlagIfFalse()
		{
			Action action = () => new NumberValidator(5, 2, true);
			action.ShouldNotThrow();
		}
    }

	[TestFixture]
	public class NumberValidator_WithOnlyPositiveFlag_Tests
	{
		private NumberValidator numberValidator;

		[SetUp]
		public void SetUp()
		{
			numberValidator = new NumberValidator(5, 2, true);
		}


		[Test]
		public void Test()
		{
			Assert.IsTrue(new NumberValidator(4, 2, true).IsValidNumber("+1.23"));
			Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("-1.23"));
		}

		[TestCase("-1.23")]
		[TestCase("-1,23")]
		public void Test_IsNotValid_On_NegativeNumber(string number)
		{
			numberValidator.IsValidNumber(number).Should().BeFalse();
		}

		[TestCase("0")]
		[TestCase("+0.00")]
		[TestCase("0.00")]
		[TestCase("00000")]
		public void Test_IsValid_OnValidZero(string number)
		{
			numberValidator.IsValidNumber(number).Should().BeTrue();
		}

		[TestCase("000000", "because precision of this number is greater than the precision of Validator")]
		[TestCase("-0.00" , "because it is negative zero")]
		[TestCase("0.000", "because scale is less than length this number")]
		[TestCase("+000.00", "because precision of this number is greater than the precision of Validator")]
		public void Test_IsNotValid_OnWrongZero(string zeroForm, string becauseMessage)
		{
			numberValidator.IsValidNumber(zeroForm).Should().BeFalse(becauseMessage);
		}

		[TestCase("a.sd", "because it's not a digits")]
		[TestCase("1a.b2", "because there are letters in a number")]
		public void Test_IsNotValid_OnLettersString(string testInput, string becauseMessage)
		{
			numberValidator.IsValidNumber(testInput).Should().BeFalse(becauseMessage);
		}

		[Test]
		public void Test_IsNotValid_OnEmptyString()
		{
			numberValidator.IsValidNumber("").Should().BeFalse();
		}

		[Test]
		public void Test_IsNotValid_OnNullString()
		{
			numberValidator.IsValidNumber(null).Should().BeFalse();
		}
		
	}

	[TestFixture]
	public class NumberValidator_WithoutOnlyPositiveFlag_Tests
	{
		private NumberValidator numberValidator;

        [SetUp]
		public void SetUp()
		{
			numberValidator = new NumberValidator(5, 2);
		}

		[TestCase("+1.23")]
		[TestCase("+2,34")]
		public void Test_IsValid_OnPositiveNumbers(string number)
		{
			numberValidator.IsValidNumber(number).Should().BeTrue();
		}

		[Test]
		public void Test_IsValid_OnUnsignedNumber()
		{
			numberValidator.IsValidNumber("1.23");
		}

		[Test]
		public void Test_IsValid_OnNegativeZero()
		{
			numberValidator.IsValidNumber("-0.00").Should().BeTrue();
		}

		[TestCase("-1.23")]
		[TestCase("-1,23")]
		public void Test_IsValid_OnNegativeNumbers(string number)
		{
			numberValidator.IsValidNumber(number).Should().BeTrue();
		}

		[Test]
		public void Test_IsNotValid_OnNumberWithoutIntPart()
		{
			numberValidator.IsValidNumber(".25").Should().BeFalse();
		}

		[Test]
		public void Test_IsNotValid_OnNumberWithCommaWithoutFractionalPart()
		{
			numberValidator.IsValidNumber("2,").Should().BeFalse();
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