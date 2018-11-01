using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	[TestFixture]
	public class NumberValidatorTests
	{
        [Test]
		[TestCase(17, 2, true, "0.1", ExpectedResult = true, TestName = "Number with dot is valid")]
		[TestCase(17, 2, true, "0,1", ExpectedResult = true, TestName = "Number with comma is valid")]
		[TestCase(17, 2, true, "0", ExpectedResult = true, TestName = "Number without fract part is valid")]
        [TestCase(3, 2, true, "+0.3", ExpectedResult = true, TestName = "Number with '+' in int part is valid")]
        [TestCase(3, 2, false, "-0.3", ExpectedResult = true, TestName = "Negative is valid on disabled positive flag")]
        [TestCase(3, 2, true, "00.00", ExpectedResult = false, TestName = "Number is longer than precision isnt valid ")]
        [TestCase(3, 2, true, "-0.3", ExpectedResult = false, TestName = "Negative isnt valid on enabled positive flag")]
		[TestCase(17, 2, true, "0.000", ExpectedResult = false, TestName = "Fractional part is longer than scale should not be validated")]
		[TestCase(3, 2, true, "a.sd", ExpectedResult = false, TestName = "Not number isnt valid")]
        [TestCase(4, 1, true, "", ExpectedResult = false, TestName = "Empty string isnt valid")]
        [TestCase(4, 1, true, null, ExpectedResult = false, TestName = "Null isnt valid")]
        public bool IsValidNumber_True_WhenArgsIsCorrect(int precision, int scale, bool onlyPositive, string value)
		{
			return new NumberValidator(precision, scale, onlyPositive).IsValidNumber(value);
        }

		[Test]
        [TestCase(-1, 2, true, TestName = "Negative precision throws ArgumentException")]
		[TestCase(1, 3, true, TestName = "Scale greater than presicion throws ArgumentException")]
		[TestCase(0, -1, false, TestName = "Negative scale throws ArgumentException")]
		[TestCase(1, 1, false, TestName = "Scale equal presicion throws ArgumentException")]
        public void Constructor_ThrowArgumentException_OnIncorrectArgs(int precision, int scale, bool onlyPositive)
		{
			Assert.That(() => new NumberValidator(precision, scale, onlyPositive),
				Throws.TypeOf<ArgumentException>());
		}

		[Test]
        [TestCase(1, 0, true)]
        public void Constructor_DoesNotThrowException_WhenArgsIsCorrect(int precision, int scale, bool onlyPositive)
		{
			Assert.That(() => new NumberValidator(precision, scale, onlyPositive),
				Throws.Nothing);
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
