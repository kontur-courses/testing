using System;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[Test]
		public void NumberValidator_WithPrecisionLessThanZero_ShouldThrowException()
		{
			var ex = Assert.Throws<ArgumentException>(() => new NumberValidator(-1, 2, true));

			Assert.AreEqual(ex.Message, "precision must be a positive number");
		}
		
		[Test]
		public void NumberValidator_WithScaleLessThanZero_ShouldThrowException()
		{
			var ex = Assert.Throws<ArgumentException>(() => new NumberValidator(1, -2, true));

			Assert.AreEqual(ex.Message, "scale must be a non-negative number less or equal than precision");
		}
		
		[Test]
		public void NumberValidator_WithScaleGreaterThanPrecision_ShouldThrowException()
		{
			var ex = Assert.Throws<ArgumentException>(() => new NumberValidator(1, 2, true));

			Assert.AreEqual(ex.Message, "scale must be a non-negative number less or equal than precision");
		}
		
		[TestCase(17, 2, true, "0.0")]
		[TestCase(17, 2, true, "0")]
		[TestCase(3, 2, true, "+0.0")]
		[TestCase(4, 2, false, "+1.23")]
		public void IsValidNumber_SuccessPath_ShouldReturnTrue(int precision, int scale, bool onlyPositive,
			string value)
		{
			var numberValidator = new NumberValidator(precision, scale, onlyPositive);

			Assert.IsTrue(numberValidator.IsValidNumber(value));
		}
		
		[TestCase("")]
		[TestCase(null)]
		public void IsValidNumber_WithNullOrEmptyValue_ShouldReturnFalse(string value)
		{
			var numberValidator = new NumberValidator(1, 0, true);

			Assert.IsFalse(numberValidator.IsValidNumber(value));
		}
		
		[TestCase("-12zxc.v32bn")]
		[TestCase("-12.00.12")]
		[TestCase("-+1.0")]
		public void IsValidNumber_WithIncorrectValue_ShouldReturnFalse(string value)
		{
			var numberValidator = new NumberValidator(1, 0, true);

			Assert.IsFalse(numberValidator.IsValidNumber(value));
		}
		
		[TestCase("-1.23")]
		[TestCase("+1.23")]
		[TestCase("00.00")]
		public void IsValidNumber_WithNumberLengthGreaterThanPrecision_ShouldReturnFalse(string value)
		{
			var numberValidator = new NumberValidator(3, 2, false);

			Assert.IsFalse(numberValidator.IsValidNumber(value));
		}
		
		[TestCase(2, true, "0.000")]
		[TestCase(0, false, "-0.0")]
		public void IsValidNumber_WithFracPartGreaterThanScale_ShouldReturnFalse(int scale, bool onlyPositive,
			string value)
		{
			var numberValidator = new NumberValidator(3, scale, onlyPositive);

			Assert.IsFalse(numberValidator.IsValidNumber(value));
		}
		
		[Test]
		public void IsValidNumber_OnlyPositiveWithNegativeNumber_ShouldReturnFalse()
		{
			var numberValidator = new NumberValidator(3, 2, true);

			Assert.IsFalse(numberValidator.IsValidNumber("-0.0"));
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