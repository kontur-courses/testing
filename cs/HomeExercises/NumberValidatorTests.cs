using System;
using System.Collections;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[Test, Explicit]
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

		[TestCase(-1, TestName = "precision is negative")]
		[TestCase(1, -1, TestName = "scale is negative")]
		[TestCase(2, 2, TestName = "scale >= precision")]
		public void Constructor_ThrowException_When(int precision, int scale = 0)
		{
			Action act = () => new NumberValidator(precision, scale);
			act.Should().Throw<ArgumentException>();
		}

		[TestCase("")]
		[TestCase(null)]
		[TestCase("a")]
		[TestCase("1.1.1")]
		[TestCase(" 1 ")]
		[TestCase("1.")]
		[TestCase(".1")]
		[TestCase("+")]
		[TestCase("-")]
		public void IsValidNumber_False_WithIncorrectValue(string value)
		{
			new NumberValidator(int.MaxValue, int.MaxValue - 1).IsValidNumber(value).Should().BeFalse();
		}

		[TestCase("12", 1, TestName = "precision less than value length")]
		[TestCase("1.2", 2, 0, TestName = "scale less than fraction length")]
		[TestCase("1.2", 1, 0, TestName = "precision less than fraction+int")]
		[TestCase("-1.2", 2, 1, TestName = "precision consider signs")]
		[TestCase("-1", 2, 0, true, TestName = "onlyPositive and negative int")]
		[TestCase("-1.2", 3, 1, true, TestName = "onlyPositive and negative float")]
		[TestCase("00001", 1, TestName = "unaccounted leading zeros in precision")]
		[TestCase("1.100", 2, 1, TestName = "unaccounted following zeros")]
		public void IsValidNumber_False_With(string value, int precision = 1, int scale = 0, bool onlyPositive = false)
		{
			var validator = new NumberValidator(precision, scale, onlyPositive);
			validator.IsValidNumber(value).Should().BeFalse();
		}

		[TestCase("1.1", 2)]
		[TestCase("1,1", 2)]
		[TestCase("-1,1", 3)]
		[TestCase("+1,1", 3)]
		[TestCase("1.12345", 6, 5)]
		public void IsValidNumber_True_WithFraction(string value, int precision = 1, int scale = 1)
		{
			var validator = new NumberValidator(precision, scale);
			validator.IsValidNumber(value).Should().BeTrue();
		}

		[TestCase("1")]
		[TestCase("-1", 2)]
		[TestCase("+1", 2)]
		[TestCase("+1", 2, true)]
		public void IsValidNumber_True_WithInteger(string value, int precision = 1, bool onlyPositive = false)
		{
			var validator = new NumberValidator(precision, 0, onlyPositive);
			validator.IsValidNumber(value).Should().BeTrue();
		}

		private static IEnumerable IsValidNumberTrueWithBigNumberCases
		{
			get
			{
				yield return new TestCaseData(new string('1', 1000)) {TestName = "Integer"};
				yield return new TestCaseData(
					$"{new string('1', 1000)}.{new string('2', 1000)}") {TestName = "Float"};
			}
		}

		[TestCaseSource(nameof(IsValidNumberTrueWithBigNumberCases))]
		public void IsValidNumber_True_WithBigNumber(string value)
		{
			var validator = new NumberValidator(int.MaxValue, int.MaxValue - 1);
			validator.IsValidNumber(value).Should().BeTrue();
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

			return !onlyPositive || match.Groups[1].Value != "-";
		}
	}
}