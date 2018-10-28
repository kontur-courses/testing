using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[Test]
		public void NumberValidator_NegativePrecision_Fail() =>
			Assert.Throws<ArgumentException>(() => new NumberValidator(-1, 2, true));
		[Test]
		public void NumberValidator_NegativeScale_Fail() =>
			Assert.Throws<ArgumentException>(() => new NumberValidator(3, -1, true));
		[Test]
		public void NumberValidator_ScaleMoreThenPrecision_Fail() =>
			Assert.Throws<ArgumentException>(() => new NumberValidator(4, 5, true));
		[Test]
		public void NumberValidator_Initialization_Success() =>
			Assert.DoesNotThrow(() => new NumberValidator(1, 0, true));

        [TestCase(10,2, true, null, ExpectedResult = false,
	        TestName = "Null string in IsValidNumber")]
        [TestCase(10, 2, true, "", ExpectedResult = false, 
	        TestName = "Empty string in IsValidNumber")]
        [TestCase(10, 2, true, "0zrt0", ExpectedResult = false, 
	        TestName = "Invalid string format in IsValidNumber")]
        [TestCase(4, 3, true, "123.33", ExpectedResult = false,
	        TestName = "Simbols in number more than precision in IsValidNumber")]
        [TestCase(11, 3, true, "123.33240", ExpectedResult = false,
	        TestName = "Simbols in fraction part more than scale in IsValidNumber")]
        [TestCase(11, 3, true, "-2.44", ExpectedResult = false,
	        TestName = "Incorrect only positive namber in IsValidNumber")]
        [TestCase(4, 3, true, "3.141", ExpectedResult = true,
	        TestName = "Corect only positive namber in IsValidNumber")]
        [TestCase(11, 3, false, "-2.44", ExpectedResult = true,
	        TestName = "Negative namber in IsValidNumber")]
        [TestCase(11, 3, false, "+2.44", ExpectedResult = true,
	        TestName = "Positive namber in IsValidNumber")]
        [TestCase(4, 0, true, "3", ExpectedResult = true, 
	        TestName = "Integer number in IsValidNumber")]
        [TestCase(4, 3, true, "3.2.2", ExpectedResult = false,
	        TestName = "More one fraction part in IsValidNumber")]
        [TestCase(4, 2, false, "+2.44", ExpectedResult = true,
	        TestName = "Consider the sign in front of the number in IsValidNumber")]
        [TestCase(3, 2, false, "2,44", ExpectedResult = true,
	        TestName = "Consider a comma in IsValidNumber")]
        [TestCase(3, 2, false, ".44", ExpectedResult = false,
	        TestName = "No integer part in IsValidNumber")]
        public bool Test_IsValidNumber(int precision, int scale, bool onlyPositive,  string input)
		{
			return new NumberValidator(precision, scale,onlyPositive ).IsValidNumber(input);
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