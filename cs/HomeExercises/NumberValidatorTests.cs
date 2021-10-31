using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	/*
	 * Возможно я переборщил с количеством тестов...
	 * Например, не уверен, что тест
	 * Constructor_Should_ThrowArgumentException_When_GivenNegativePrecision_Ignoring_onlyPositive
	 * проверяет что-то полезное, ведь значение для onlyPositive просто
	 * передаётся полю класса. Но опять же если сделать разные конструкторы для
	 * разных значений onlyPositive, то этот тест должен поймать различные обработки ошибок.
	 * В общем сложно найти грань между нормальным количеством тестов и избыточным.
	 */
	[TestFixture]
	public class NumberValidatorTests
	{
		[Test]
		public void Constructor_Should_ThrowArgumentException_When_GivenNegativePrecision()
			=> Assert.Throws<ArgumentException>(() => new NumberValidator(-1, 2));

		[Test]
		public void Constructor_Should_ThrowArgumentException_When_GivenZeroPrecision()
			=> Assert.Throws<ArgumentException>(() => new NumberValidator(0, 2));

		[Test]
		public void Constructor_Should_ThrowArgumentException_When_GivenNegativePrecision_Ignoring_onlyPositive()
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(-2, 1, true));
			Assert.Throws<ArgumentException>(() => new NumberValidator(-2, 1, false));
		}
		
		[Test]
		public void Constructor_Should_NotThrow_With_CorrectPrecisionAndScale()
			=> Assert.DoesNotThrow(() => new NumberValidator(1, 0, true));

		[Test]
		public void Constructor_Should_ThrowArgumentException_When_GivenNegativeScale()
			=> Assert.Throws<ArgumentException>(() => new NumberValidator(1, -1));
		
		[Test]
		public void Constructor_Should_ThrowArgumentException_When_GivenScaleBiggerThanPrecision()
			=> Assert.Throws<ArgumentException>(() => new NumberValidator(1, 2));

		[Test]
		public void Constructor_Should_ThrowArgumentException_When_GivenScaleEqualToPrecision()
			=> Assert.Throws<ArgumentException>(() => new NumberValidator(1, 1));
	
		[TestCase("")]
		[TestCase(null)]
		[TestCase(" ")]
		[TestCase("\t")]
		[TestCase("\n")]
		[TestCase("\r\n")]
		[TestCase("a.bd")]
		[TestCase("ab.d")]
		[TestCase("2.a")]
		[TestCase("a.2")]
		[TestCase("0-1")]
		[TestCase(".0")]
		[TestCase("0.")]
		[TestCase(" 0.1")]
		public void IsValidNumber_ShouldBe_False_GivenIncorrectString(string value)
			=> Assert.IsFalse(new NumberValidator(int.MaxValue, int.MaxValue - 1).IsValidNumber(value));
		

		[TestCase(3, "10.20")]
		[TestCase(3, "+3.01")]
		[TestCase(3, "-0.05")]
		[TestCase(2, "0.00")]
		public void IsValidNumber_ShouldBe_False_GivenNumber_With_IncorrectPrecision(int precision, string value)
			=> Assert.IsFalse(new NumberValidator(precision, precision - 1).IsValidNumber(value));
		
		
		[TestCase("-1.23")]
		[TestCase("-0.00")]
		[TestCase("-60")]
		public void IsValidNumber_ShouldBe_False_GivenNegativeNumber_When_onlyPositive_Is_True(string value)
			=> Assert.IsFalse(new NumberValidator(4, 2, true).IsValidNumber(value));
		
		
		[TestCase(2, "0.002")]
		[TestCase(1, "10.11")]
		[TestCase(0, "1.1")]
		public void IsValidNumber_ShouldBe_False_GivenNumber_With_IncorrectScale(int scale, string value)
			=> Assert.IsFalse(new NumberValidator(int.MaxValue, scale).IsValidNumber(value));
		
		
		[TestCase("0.1")]
		[TestCase("2")]
		[TestCase("+0")]
		[TestCase("-0")]
		[TestCase("+1.23")]
		[TestCase("-2.4444")]
		[TestCase("00002.20000")]
		[TestCase("0.0")]
		[TestCase("1,4")]
		public void IsValidNumber_ShouldBe_True_GivenCorrectNumber(string value)
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