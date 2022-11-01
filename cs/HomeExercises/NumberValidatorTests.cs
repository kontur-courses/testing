using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[TestCase(-1, 2)]
		[TestCase(1, 2)]
		[TestCase(1, -1)]
		public void UnCorrectInitial_Exception(int precision, int scale)
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(precision, scale, true));
		}
		[Test]
		public void CorrectInitial()
		{			
			Assert.DoesNotThrow(() => new NumberValidator(1, 0, true));
		}
		
		[TestCase("0", true)]
		[TestCase("0.0", true)]
		[TestCase("-1.23", false)]
		[TestCase("1.23", false)]
		[TestCase("-1", false)]
		[TestCase("+1.23", true)]
		public void CorrectStringValue_True(string value, bool onlyPositive)
		{			
			var numberValidator = new NumberValidator(17, 4, onlyPositive);
			var validatingResult = numberValidator.IsValidNumber(value);
			Assert.IsTrue(validatingResult);
		}
		[TestCase("-+1", false)]
		[TestCase("a.sd", true)]
		[TestCase("1.", true)]
		[TestCase("1..1", true)]
		[TestCase(".", true)]
		public void UncorrectValueTests_False(string value, bool onlyPositive)
		{
			Assert.IsFalse(new NumberValidator(4, 2, onlyPositive).IsValidNumber(value));
		}
		[Test]
		public void IntAndFracPartBiggerThenPrecision_False()
		{
			Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("00.00"));
		}
		[Test]
		public void NegativeIntAndFracPathBiggerThanPrecision_False()
		{
			Assert.IsFalse(new NumberValidator(3, 2, false).IsValidNumber("-0.00"));
		}
		[Test]
		public void NegativeValueWithOnlyPositive_False()
		{
			Assert.IsFalse(new NumberValidator(17, 2, true).IsValidNumber("-0.00"));
		}
		[Test]
		public void FracPathBiggerThanScale_False()
		{
			Assert.IsFalse(new NumberValidator(17, 2, true).IsValidNumber("0.000"));
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