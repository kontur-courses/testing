using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[Test]
		public void NegativePrecision()
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(-1, 2, true));
		}
		[Test]
		public void CorrectInitial()
		{			
			Assert.DoesNotThrow(() => new NumberValidator(1, 0, true));
		}
		[Test]
		public void ScaleBiggerThanPrecision()
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(1, 2, false));
		}
		[Test]
		public void NegativeScale()
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(1, -1, false));
		}
		[Test]
		public void ZeroValue()
		{			
			Assert.IsTrue(new NumberValidator(17, 2, true).IsValidNumber("0"));
		}
		[Test]
		public void ZeroWithFrac()
		{
			Assert.IsTrue(new NumberValidator(17, 2, true).IsValidNumber("0.0"));
		}
		[Test]
		public void IntAndFracPartBiggerThenPrecision()
		{
			Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("00.00"));
		}
		[Test]
		public void NegativeIntAndFracPathBiggerThanPrecision()
		{
			Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("-0.00"));
		}
		[Test]
		public void CorrectNegative()
		{
			Assert.IsTrue(new NumberValidator(4, 2).IsValidNumber("-1.23"));
		}
		[Test]
		public void CorrectValue()
		{
			Assert.IsTrue(new NumberValidator(4, 2).IsValidNumber("1.23"));
		}
		[Test]
		public void CorrectNegativeWithoutFrac()
		{
			Assert.IsTrue(new NumberValidator(4, 2).IsValidNumber("-1"));
		}
		[Test]
		public void UncorrectValue()
		{
			Assert.IsFalse(new NumberValidator(4, 2).IsValidNumber("-+1"));
		}
		[Test]
		public void CorrectValueWithPlus()
		{
			Assert.IsTrue(new NumberValidator(4, 2, true).IsValidNumber("+1.23"));
		}
		[Test]
		public void FracPathBiggerThanScale()
		{
			Assert.IsFalse(new NumberValidator(17, 2, true).IsValidNumber("0.000"));
		}
		[Test]
		public void UncorrectValueWithLetters()
		{
			Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("a.sd"));
		}
		[Test]
		public void WithoutFracPathAfterDot()
		{
			Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("1."));
		}
		[Test]
		public void WithFracPathAfterTwoDot()
		{
			Assert.IsFalse(new NumberValidator(4, 2, true).IsValidNumber("1..1"));
		}
		[Test]
		public void OnlyDot()
		{
			Assert.IsFalse(new NumberValidator(4, 2, true).IsValidNumber("."));
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