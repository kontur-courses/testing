using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[TestCase(-2, 1, TestName = "Precision is negative")]
		[TestCase(2, -1, TestName = "Scale is negative")]
		[TestCase(1, 2, TestName = "Scale more then precision")]
        public void ThrowArgumentException_When(int precision, int scale)
		{
			// ReSharper disable once ObjectCreationAsStatement
			Assert.Throws<ArgumentException>(() => new NumberValidator(precision, scale, true));
			// ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentException>(() => new NumberValidator(precision, scale, false));
        }

        [TestCase(1, 0)]
        [TestCase(2, 1)]
        public void CreateValidator_WhenArgumentsIsCorrect(int precision, int scale)
		{
			// ReSharper disable once ObjectCreationAsStatement
			Assert.DoesNotThrow(() => new NumberValidator(precision, scale, true));
			// ReSharper disable once ObjectCreationAsStatement
			Assert.DoesNotThrow(() => new NumberValidator(precision, scale, false));
        }

        [Test]
		public void ReturnFalse_WhenNumbersMoreThenPrecision()
		{
			Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("00.00"));
		}

		[TestCase("-0.00",false)]
		[TestCase("+0.00", true)]
		[TestCase("0,00", true)]
		[TestCase("0.00", true)]
		[TestCase("00.00", true)]
        public void ReturnTrue_When(string number, bool onlyPositive)
		{
			Assert.IsTrue(new NumberValidator(17, 2, onlyPositive).IsValidNumber(number));
		}

		[TestCase("0", true)]
		[TestCase("+0", true)]
		[TestCase("-0", false)]
        public void ReturnTrue_When1(string number, bool onlyPositive)
        {
	        Assert.IsTrue(new NumberValidator(17, 0, onlyPositive).IsValidNumber(number));
        }

        [TestCase("0.000")]
        [TestCase("000.0")]
        public void DoSomething_When(string number)
		{
			Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber(number));
		}

		[TestCase(null)]
		[TestCase("")]
		[TestCase(" ")]
		[TestCase("\n\r")]
		[TestCase("a.sd")]
        public void ReturnFalse_When(string number)
		{
			Assert.IsFalse(new NumberValidator(17, 2, true).IsValidNumber(number));
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