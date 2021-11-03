using System;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace HomeExercises
{
	[TestFixture]
	public class NumberValidatorTests
	{
		[Test]
		public void Constructor_Should_ThrowArgumentException_When_GivenNegativePrecision()
			=> Assert.Throws<ArgumentException>(() => new NumberValidator(-1, 2));

		[Test]
		public void Constructor_Should_ThrowArgumentException_When_GivenZeroPrecision()
			=> Assert.Throws<ArgumentException>(() => new NumberValidator(0, 2));

		[TestCase(1, 0, true, TestName = "onlyPositive is true and scale is zero")]
		[TestCase(5, 2, false, TestName = "onlyPositive is false and scale is less then precision")]
		[TestCase(10, 9, TestName = "Scale is one less than precision")]
		public void Constructor_Should_NotThrow_When_GivenCorrectPrecisionAndScale(int precision, int scale, bool onlyPositive = false)
			=> Assert.DoesNotThrow(() => new NumberValidator(precision, scale, onlyPositive));

		[Test]
		public void Constructor_Should_ThrowArgumentException_When_GivenNegativeScale()
			=> Assert.Throws<ArgumentException>(() => new NumberValidator(1, -1));
		
		[Test]
		public void Constructor_Should_ThrowArgumentException_When_GivenScaleBiggerThanPrecision()
			=> Assert.Throws<ArgumentException>(() => new NumberValidator(1, 2));

		[Test]
		public void Constructor_Should_ThrowArgumentException_When_GivenScaleEqualToPrecision()
			=> Assert.Throws<ArgumentException>(() => new NumberValidator(1, 1));
	
		[TestCase("", TestName = "Empty string")]
		[TestCase(null, TestName = "Null check")]
		[TestCase(" ", TestName = "1 Space")]
		[TestCase("\t", TestName = "1 Tab")]
		[TestCase("\n", TestName = "New Line")]
		[TestCase("\r\n", TestName = "Windows New Line")]
		[TestCase("a.bd", TestName = "Not numbers separated with a dot")]
		[TestCase("2.a", TestName = "Mixing symbols with numbers, symbol after the dot")]
		[TestCase("a.2", TestName = "Mixing symbols with numbers, symbol before the dot")]
		[TestCase("0-1", TestName = "Negative number after zero")]
		[TestCase(".0", TestName = "No digits before the dot")]
		[TestCase("0.", TestName = "No digits after the dot")]
		[TestCase(" 0.1", TestName = "Space before correct number")]
		public void IsValidNumber_Should_Be_False_When_GivenStringInIncorrectFormat(string value)
			=> Assert.IsFalse(new NumberValidator(int.MaxValue, int.MaxValue - 1).IsValidNumber(value));
		

		[TestCase(3, "10.02", TestName = "More digits than precision")]
		[TestCase(3, "+3.01", TestName = "Is + affecting precision check")]
		[TestCase(3, "-0.05", TestName = "Is - affecting precision check")]
		[TestCase(2, "0.00", TestName = "Zeros are not ignored")]
		public void IsValidNumber_Should_Be_False_When_GivenNumberWithIncorrectPrecision(int precision, string value)
			=> Assert.IsFalse(new NumberValidator(precision, precision - 1).IsValidNumber(value));
		
		
		[TestCase("-1.23", TestName = "Negative number with the dot")]
		[TestCase("-0.00", TestName = "Negative zero")]
		[TestCase("-60", TestName = "Negative number without the dot")]
		public void IsValidNumber_Should_Be_False_When_GivenNegativeNumber_And_onlyPositive_Is_True(string value)
			=> Assert.IsFalse(new NumberValidator(4, 2, true).IsValidNumber(value));
		
		
		[TestCase(2, "0.002", TestName = "Number of digits after dot is more than scale")]
		[TestCase(1, "10.10", TestName = "Zeros at the end count towards scale")]
		[TestCase(0, "1.1", TestName = "With scale=0 anything after dot")]
		public void IsValidNumber_Should_Be_False_When_GivenNumberWithIncorrectScale(int scale, string value)
			=> Assert.IsFalse(new NumberValidator(int.MaxValue, scale).IsValidNumber(value));
		
		
		[TestCase("0.1", TestName = "Number with decimal point")]
		[TestCase("2", TestName = "Whole number")]
		[TestCase("+0", TestName = "Positive zero")]
		[TestCase("-0", TestName = "Negative zero")]
		[TestCase("+1.23", TestName = "Positive number with decimal point")]
		[TestCase("-2.4444", TestName = "Negative number with decimal point")]
		[TestCase("00002.20000", TestName = "Leading and following zero for 2.2")]
		[TestCase("0.0", TestName = "Zero with decimal point")]
		[TestCase("1,4", TestName = "Number with decimal comma")]
		public void IsValidNumber_Should_Be_True_When_GivenCorrectNumber(string value)
			=> Assert.IsTrue(new NumberValidator(int.MaxValue, int.MaxValue - 1).IsValidNumber(value));
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
			numberRegex = new Regex(@"^(?<sign>[+-]?)(?<intPart>\d+)([.,](?<fracPart>\d+))?$", RegexOptions.IgnoreCase);
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
			var intPart = match.Groups["sign"].Value.Length + match.Groups["intPart"].Value.Length;
			// Дробная часть
			var fracPart = match.Groups["fracPart"].Value.Length;

			if (intPart + fracPart > precision || fracPart > scale)
				return false;

			if (onlyPositive && match.Groups["sign"].Value == "-")
				return false;
			return true;
		}
	}
}