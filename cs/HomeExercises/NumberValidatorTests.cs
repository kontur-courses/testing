using System;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[TestCase(-1, 2, true, TestName = "precision < 0")]
		[TestCase(0, 2, true, TestName = "precision = 0")]
		[TestCase(1, -2, false, TestName = "scale < 0")]
		[TestCase(1, 2, false, TestName = "precision < scale")]
		[TestCase(1, 1, false, TestName = "precision = scale")]
		public void Constructor_fail_when(int precision, int scale, bool onlyPositive)
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(precision, scale, onlyPositive));
        }

		[TestCase(1, 0, true, TestName = "precision > 0 and scale = 0")]
		[TestCase(2, 1, true, TestName = "precision > scale")]
        public void Constructor_success_when(int precision, int scale, bool onlyPositive)
		{
			Assert.DoesNotThrow(() => new NumberValidator(precision, scale, onlyPositive));
        }

        [TestCase(17, 2, true, "0.0", ExpectedResult = true, TestName = "Should pass when value = 0.0 with precision and scale > 1")]
        [TestCase(17, 2, true, "0", ExpectedResult = true, TestName = "Should pass when value = 0 with precision and scale > 1")]
        [TestCase(3, 2, true, "00.00", ExpectedResult = false, TestName = "Should fail when value without point length > precision")]
        [TestCase(3, 2, true, "-0.00", ExpectedResult = false, TestName = "Should fail when value with minus and length more then precision")]
        [TestCase(3, 2, true, "+0.00", ExpectedResult = false, TestName = "Should fail when value with plus and length more then precision")]
        [TestCase(3, 2, true, "+1.23", ExpectedResult = false, TestName = "Should fail when value start with + with positive - scale = 1")]
        [TestCase(17, 2, true, "0.000", ExpectedResult = false, TestName = "Should fail when value after point > scale")]
        [TestCase(3, 2, true, "-1.23", ExpectedResult = false, TestName = "Should fail when value with - without point length > precision")]
        [TestCase(3, 2, true, "a.sd", ExpectedResult = false, TestName = "Should fail when value contains letters instead of digits")]
        [TestCase(3, 2, true, null, ExpectedResult = false, TestName = "Should fail when value = null")]
        [TestCase(3, 2, true, "", ExpectedResult = false, TestName = "Should fail when value is empty")]
        [TestCase(4, 2, true, "+1,23", ExpectedResult = true, TestName = "Should pass when value start with +")]
        [TestCase(17, 9, true, "0.0.0", ExpectedResult = false, TestName = "Should fail when value have 2 points")]
        [TestCase(17, 9, true, "0.", ExpectedResult = false, TestName = "Should fail when 0 chars after point")]
        [TestCase(17, 9, true, ".0", ExpectedResult = false, TestName = "Should fail when 0 chars before point")]
        [TestCase(17, 9, true, "-0", ExpectedResult = false, TestName = "Should fail when value -0 with only positive without scale")]
        [TestCase(17, 9, false, "-0", ExpectedResult = true, TestName = "Should pass when value -0 without only positive and scale")]
        [TestCase(17, 9, true, "+-0", ExpectedResult = false, TestName = "Should fail when value with +-")]
        [TestCase(17, 9, true, " -0.0", ExpectedResult = false, TestName = "Should fail when value starts with space")]
        [TestCase(17, 9, true, "0.0 ", ExpectedResult = false, TestName = "Should fail when value ends with space")]
        [TestCase(17, 9, true, "000.0000", ExpectedResult = true, TestName = "Should pass when value starts with extra zeros")]
        [TestCase(17, 9, true, "000.0", ExpectedResult = true, TestName = "Should pass when value starts with 3 zero")]
        [TestCase(17, 9, true, "0٤٥٦٧٨.0", ExpectedResult = true, TestName = "Should pass when value contains eastern arabic numerals")]
        public bool ValidNumber(int precision, int scale, bool onlyPositive, string value)
		{
			return new NumberValidator(precision, scale, onlyPositive).IsValidNumber(value);
		}

		[Test]
		public void Test()
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(-1, 2, true));
			Assert.DoesNotThrow(() => new NumberValidator(1, 0, true));
			Assert.Throws<ArgumentException>(() => new NumberValidator(-1, 2, false));
			Assert.DoesNotThrow(() => new NumberValidator(1, 0, true));

			Assert.IsTrue(new NumberValidator(17, 2, true).IsValidNumber("0.0"));
			Assert.IsTrue(new NumberValidator(17, 2, true).IsValidNumber("0"));
			Assert.IsTrue(new NumberValidator(17, 2, true).IsValidNumber("0.0"));
			Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("00.00"));
			Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("-0.00"));
			Assert.IsTrue(new NumberValidator(17, 2, true).IsValidNumber("0.0"));
			Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("+0.00"));
			Assert.IsTrue(new NumberValidator(4, 2, true).IsValidNumber("+1.23"));
			Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("+1.23"));
			Assert.IsFalse(new NumberValidator(17, 2, true).IsValidNumber("0.000"));
			Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("-1.23"));
			Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("a.sd"));
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