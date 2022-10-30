using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[Test]
		[TestCase(-1, 2, true)]
		[TestCase(1, -1, true)]
		[TestCase(1, 2, true)]
		public void TestThrows(int precision, int scale, bool onlyPositive)
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(precision, scale, onlyPositive));
			
			// Assert.Throws<ArgumentException>(() => new NumberValidator(-1, 2, true));
			// Assert.DoesNotThrow(() => new NumberValidator(1, 0, true));
			// Assert.Throws<ArgumentException>(() => new NumberValidator(-1, 2, false));
			// Assert.DoesNotThrow(() => new NumberValidator(1, 0, true));
		}
		
		[Test]
		// Значение не пустое и не null
		[TestCase(1, 0, true, "", false)]
		[TestCase(1, 0, true, null, false)]
		// Проверка Regex
		[TestCase(1, 0, true, "-12zxc.v32bn", false)]
		// Проверки с успешным исходом
		[TestCase(17, 2, true, "0.0", true)]
		[TestCase(17, 2, true, "0", true)]
		// Проверки на знак
		[TestCase(3, 2, true, "-0.0", false)]
		[TestCase(3, 2, true, "+0.0", true)]
		// Проверки на длину числа
		[TestCase(4, 2, false, "+1.23", true)]
		[TestCase(3, 2, false, "-1.23", false)]
		[TestCase(3, 2, true, "+1.23", false)]
		[TestCase(3, 2, true, "00.00", false)]
		// Проверки на длину дробной части
		[TestCase(17, 2, true, "0.000", false)]
		public void Test(int precision, int scale, bool onlyPositive, string value, object exceptedValue)
		{
			Assert.AreEqual(exceptedValue, new NumberValidator(precision, scale, onlyPositive).IsValidNumber(value));
			
			// Assert.IsTrue(new NumberValidator(17, 2, true).IsValidNumber("0.0"));
			// Assert.IsTrue(new NumberValidator(17, 2, true).IsValidNumber("0"));
			// Assert.IsTrue(new NumberValidator(17, 2, true).IsValidNumber("0.0"));
			// Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("00.00"));
			// Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("-0.00"));
			// Assert.IsTrue(new NumberValidator(17, 2, true).IsValidNumber("0.0"));
			// Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("+0.00"));
			// Assert.IsTrue(new NumberValidator(4, 2, true).IsValidNumber("+1.23"));
			// Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("+1.23"));
			// Assert.IsFalse(new NumberValidator(17, 2, true).IsValidNumber("0.000"));
			// Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("-1.23"));
			// Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("a.sd"));
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