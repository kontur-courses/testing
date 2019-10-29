using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	[TestFixture]
	public class NumberValidatorTests
	{
		[TestCase(0)]
		[TestCase(-10)]
        public void Constructor_ThrowsArgumentException_IfPrecisionIsNotPositive(int precision)
		{
			Action act = () => new NumberValidator(precision);
			act.ShouldThrow<ArgumentException>("precision is not positive number");
		}

        [TestCase(3)]
        [TestCase(4)]
        public void Constructor_ThrowsArgumentException_IfScaleEqualsOrBiggerThanPrecision(int scale)
		{
			Action act = () => new NumberValidator(3, scale);
			act.ShouldThrow<ArgumentException>("precision should be bigger than scale");
		}


		[Test]
		public void Constructor_ThrowsArgumentException_IfScaleIsNegative()
        {
	        Action act = () => new NumberValidator(3, -10);
	        act.ShouldThrow<ArgumentException>("scale is not positive number");
        }

		[Test]
		public void Constructor_NoExceptions_IfPrecisionIsPositive()
		{
			Action act = () => new NumberValidator(3);
			act.ShouldNotThrow();
        }

		[TestCase(9, true)]
		[TestCase(0, false)]
		public void Constructor_NoException_OnPassingNonDefaultParameters(int scale, bool onlyPositive)
		{
			Action act = () => new NumberValidator(10, scale, onlyPositive);
			act.ShouldNotThrow();
        }

		[TestCase(0)]
		[TestCase(1)]
		public void Constructor_NoException_IfScaleIsNonNegative(int scale)
		{
			Action act = () => new NumberValidator(3, scale);
			act.ShouldNotThrow();
		}

		[Test]
		public void IsValidNumber_ReturnsFalse_IfNumberIsNull()
		{
			var nv = new NumberValidator(3);
			nv.IsValidNumber(null).Should().BeFalse();
        }

		[Test]
		public void IsValidNumber_ReturnsFalse_IfNumberIsEmpty()
		{
			var nv = new NumberValidator(3);
			nv.IsValidNumber("").Should().BeFalse();
		}

        [TestCase("abc")]
		[TestCase("12a45")]
		[TestCase("123.ab6")]
		[TestCase("12%45")]
		[TestCase("11  ")]
        public void IsValidNumber_ReturnsFalse_IfNumberHasNonNumericChars(string number)
		{
			var nv = new NumberValidator(10,5);
			nv.IsValidNumber(number).Should().BeFalse();
		}

		[TestCase("௧௨௩", "Tamil")]
		[TestCase("๐၁၂", "Burmese")]
        public void IsValidNumber_ReturnsTrue_OnGivenNonArabicNumbers(string number, string language)
        {
	        var nv = new NumberValidator(10);
	        nv.IsValidNumber(number).Should().BeTrue($"{language} numerals should be supported");
        }

        [TestCase("1")]
		[TestCase("12345")]
        public void IsValidNumber_ReturnsTrue_IfNumberLenghtIsEqualOrLessThanPercision(string number)
        {
	        var nv = new NumberValidator(5);
	        nv.IsValidNumber(number).Should().BeTrue();
        }

        [TestCase("0.1")]
        [TestCase("0.123")]
        public void IsValidNumber_ReturnsTrue_IfNumberFracPartLengthEqualOrLessThanScale(string number)
        {
	        var nv = new NumberValidator(5,3);
	        nv.IsValidNumber(number).Should().BeTrue();
        }

        [Test]
        public void IsValidNumber_ReturnsFalse_IfNumberIsLongerThanPrecision()
		{
			var nv = new NumberValidator(3);
			nv.IsValidNumber("1234").Should().BeFalse();
		}

		[TestCase("+1234")]
		[TestCase("-1234")]
		public void IsValidNumber_ReturnsFalse_IfLengthOfNumberAndSignGreaterThanPrecision(string number)
		{
			var nv = new NumberValidator(3);
			nv.IsValidNumber(number).Should().BeFalse();
		}

		[TestCase("1.234")]
		[TestCase("1,234")]
		public void IsValidNumber_ReturnsTrue_IfDotAndCommaUsedAsSeparators(string number)
		{
			var nv = new NumberValidator(4, 3);
			nv.IsValidNumber(number).Should().BeTrue();
		}

		[Test]
		public void IsValidNumber_ReturnsFalse_IfNoValueAfterSeparator()
		{
			var nv = new NumberValidator(4, 3);
			nv.IsValidNumber("123.").Should().BeFalse();
		}

		[Test]
        public void IsValidNumber_ReturnsFalse_IfNumberScaleGreaterThanValidatorScale()
		{
			var nv = new NumberValidator(4, 3);
			nv.IsValidNumber("0.1234").Should().BeFalse();
		}

		[Test]
		public void IsValidNumber_ReturnsFalse_IfNegativeNumberPassedToPositiveNumberValidator()
		{
			var nv = new NumberValidator(3, onlyPositive: true);
			nv.IsValidNumber("-12").Should().BeFalse();
		}

		[TestCase("+")]
		[TestCase("-")]
        public void IsValidNumber_ReturnsFalse_IfInputHasOnlyOneSign(string input)
		{
			var nv = new NumberValidator(3);
			nv.IsValidNumber(input).Should().BeFalse();
		}

		[TestCase("--1")]
		[TestCase("++1")]
		public void IsValidNumber_ReturnsFalse_IfNumberHasMoreThanOneSign(string number)
		{
			var nv = new NumberValidator(3);
			nv.IsValidNumber(number).Should().BeFalse();
		}

		[Test]
		public void IsValidNumber_ReturnsFalse_IfNumberLengthWithZeroPrefixIsGreaterThanPrecision()
		{
			var nv = new NumberValidator(3);
			nv.IsValidNumber("0123").Should().BeFalse();
		}

        [TestCase("12..3")]
		[TestCase("12.,3")]
		[TestCase("12.3.4")]
        public void IsValidNumber_ReturnsFalse_IfInputHasMoreThanOneSeparator(string number)
		{
			var nv = new NumberValidator(10, 5);
			nv.IsValidNumber(number).Should().BeFalse();
		}

		[TestCase(100)]
		[TestCase(1000)]
		[TestCase(10000)]
		public void IsValidNumber_IsTimePermissible_OnBigNumberLength(int numberPrecision)
		{
			var nv = new NumberValidator(numberPrecision);
			Action action = () => nv.IsValidNumber(new string('0', numberPrecision));
			action.ExecutionTime().ShouldNotExceed(100.Milliseconds());
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