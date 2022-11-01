using System;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[Test]
		public void IsValidNumber_CorrectValue_True()
		{
			Assert.IsTrue(new NumberValidator(17, 2, true).IsValidNumber("0.0"));
		}
		
		[Test]
		public void IsValidNumber_NegativeValueWithOnlyPositiveFlag_False()
		{
			Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("-11.1"));
		}
		
		[Test]
		public void IsValidNumber_SumOfPartsBiggerThenPrecision_False()
		{
			Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("1111.11"));
			Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("+1111.1"));
		}
		
		[Test]
		public void IsValidNumber_FracPartBiggerThenScale_False()
		{
			Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("1.1231"));
		}

		[Test]
		public void IsValidNumber_DoesNotMuchValue_False()
		{
			Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("a.sd"));
		}

		[Test]
		public void IsValidNumber_NullOrEmptyValue_False()
		{
			Assert.IsFalse(new NumberValidator(17, 2, true).IsValidNumber(""));
			Assert.IsFalse(new NumberValidator(17, 2, true).IsValidNumber(null));
		}

		[Test]
		public void NumberValidator_CorrectValues_DoesNotThrowException()
		{
			Assert.DoesNotThrow(() => new NumberValidator(1, 0, true));
		}

		[Test]
		public void NumberValidator_ScaleBiggerThenPrecision_ThrowException()
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(3, 4));
		}
		
		[Test]
		public void NumberValidator_ScaleEqualPrecision_ThrowException()
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(3, 3));
		}
		
		[Test]
		public void NumberValidator_ScaleIsNegative_ThrowException()
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(3, -1));
		}
		
		[Test]
		public void NumberValidator_PrecisionIsNegative_ThrowException()
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(-1, 2));
		}
		
		[Test]
		public void NumberValidator_PrecisionIsZero_ThrowException()
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(0, 2));
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
