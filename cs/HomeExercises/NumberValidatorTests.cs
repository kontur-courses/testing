using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[TestCase(0, 2, TestName = "Precision zero")]
		[TestCase(-1, 2, TestName = "Precision negative")]
		[TestCase(1, -2, TestName = "Scale negative")]
		[TestCase(1, 1, TestName = "Scale equals to precision")]
		[TestCase(1, 2, TestName = "Scale is greater than precision")]
		public void Constructor_ThrowExceptions_OnIncorrectArguments(int precision, int scale)
		{
			Action onlyPositive = () => new NumberValidator(precision, scale, true);
			Action allRange = () => new NumberValidator(precision, scale, false);

			onlyPositive.Should().Throw<ArgumentException>();
			allRange.Should().Throw<ArgumentException>();
		}

		[TestCase(1, 0, TestName = "Small values")]
		[TestCase(int.MaxValue, 0, TestName = "Big precision value")]
		[TestCase(int.MaxValue, int.MaxValue - 1, TestName = "Big precision and scale values")]
		public void Constructor_ThrowNoExceptions_OnCorrectArguments(int precision, int scale)
		{
			Action onlyPositive = () => new NumberValidator(precision, scale, true);
			Action allRange = () => new NumberValidator(precision, scale, false);

			onlyPositive.Should().NotThrow();
			allRange.Should().NotThrow();
		}

		[Test]
		public void IsValidNumber_ReturnFalse_OnNullString()
		{
			NumberValidatorBuilder.Default.IsValidNumber(null).Should().BeFalse();
		}

		[TestCase("", TestName = "Empty")]
		[TestCase("abc", TestName = "Letters")]
		[TestCase("a.bc", TestName = "Letters with dot")]
		[TestCase("%&#", TestName = "Non digits")]
		public void IsValidNumber_ReturnFalse_OnNonNumberFormattedString(string value)
		{
			NumberValidatorBuilder.Default.IsValidNumber(value).Should().BeFalse();
		}


		[TestCase(".", TestName = "Only dot")]
		[TestCase(".56", TestName = "Without integer part")]
		[TestCase("2.", TestName = "Dot without fractional part")]
		[TestCase("2..2", TestName = "More than one dot")]
		[TestCase("2.,2", TestName = "Dot comma mixed")]
		[TestCase("++5", TestName = "More than one plus sign")]
		[TestCase("+-5", TestName = "Plus minus signs mixed")]
		public void IsValidNumber_ReturnFalse_OnWrongNumberFormattedString(string value)
		{
			NumberValidatorBuilder.Default.IsValidNumber(value).Should().BeFalse();
		}

		[TestCase("1234", TestName = "Integer number")]
		[TestCase("12.34", TestName = "Float number")]
		[TestCase("0000", TestName = "Only zeros")]
		[TestCase("0.000", TestName = "Zeros with dot")]
		public void IsValidNumber_ReturnFalse_OnTooLongNumber(string value)
		{
			new NumberValidatorBuilder().SetPrecision(3).SetScale(2).Build().IsValidNumber(value).Should().BeFalse();
		}

		[Test]
		public void IsValidNumber_ReturnFalse_OnTooLongFractionalPart()
		{
			new NumberValidatorBuilder().SetPrecision(1000).Build().IsValidNumber("1.234").Should().BeFalse();
		}

		[Test]
		public void IsValidNumber_ReturnFalse_OnNegativeNumberForOnlyPositive()
		{
			new NumberValidatorBuilder().SetOnlyPositive(true).Build().IsValidNumber("-5").Should().BeFalse();
		}

		[TestCase("12", TestName = "Integer")]
		[TestCase("12345", TestName = "Max length integer")]
		[TestCase("0.1", TestName = "Float")]
		[TestCase("123.45", TestName = "Max length float")]
		[TestCase("+1234", TestName = "Max length integer with plus")]
		[TestCase("-1234", TestName = "Max length integer with minus")]
		[TestCase("-12.45", TestName = "Max length float with minus")]
		public void IsValidNumber_ReturnTrue_OnCorrectValueForAllRange(string value)
		{
			NumberValidatorBuilder.Default.IsValidNumber(value).Should().BeTrue();
		}

		[TestCase("12", TestName = "Integer")]
		[TestCase("12345", TestName = "Max length integer")]
		[TestCase("0.1", TestName = "Float")]
		[TestCase("123.45", TestName = "Max length float")]
		[TestCase("+1234", TestName = "Max length integer with plus")]
		[TestCase("+12.45", TestName = "Max length float with plus")]
		public void IsValidNumber_ReturnTrue_OnCorrectValueForOnlyPositive(string value)
		{
			new NumberValidatorBuilder().SetOnlyPositive(true).Build().IsValidNumber(value).Should().BeTrue();
		}

		[Test]
		public void IsValidNumber_ReturnTrue_WithCorrectBigNumbers()
		{
			var bigInteger = new string('5', 10000);
			var bigFloat = new string('1', 5000) + "." + new string('2', 5000);
			var validator = new NumberValidatorBuilder().SetPrecision(int.MaxValue).SetScale(int.MaxValue - 1).Build();

			validator.IsValidNumber(bigInteger).Should().BeTrue();
			validator.IsValidNumber(bigFloat).Should().BeTrue();
		}


		[Test]
		[Explicit]
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

		private class NumberValidatorBuilder
		{
			private const int DefaultPrecision = 5;
			private const int DefaultScale = 2;
			private const bool DefaultOnlyPositive = false;

			public static readonly NumberValidator Default =
				new(DefaultPrecision, DefaultScale, DefaultOnlyPositive);

			private int _precision = DefaultPrecision;
			private int _scale = DefaultScale;
			private bool _onlyPositive = DefaultOnlyPositive;

			public NumberValidatorBuilder SetPrecision(int value)
			{
				_precision = value;
				return this;
			}

			public NumberValidatorBuilder SetScale(int value)
			{
				_scale = value;
				return this;
			}

			public NumberValidatorBuilder SetOnlyPositive(bool value)
			{
				_onlyPositive = value;
				return this;
			}

			public NumberValidator Build() => new(_precision, _scale, _onlyPositive);
		}
	}

	public class NumberValidator
	{
		private const string NumberRegexPattern = @"^([+-]?)(\d+)([.,](\d+))?$";
		private static readonly Regex NumberRegex = new(NumberRegexPattern, RegexOptions.IgnoreCase);

		private readonly int _precision;
		private readonly int _scale;
		private readonly bool _onlyPositive;

		public NumberValidator(int precision, int scale = 0, bool onlyPositive = false)
		{
			if (precision <= 0)
				throw new ArgumentException($"{nameof(precision)} must be a positive number");
			_precision = precision;

			if (scale < 0 || scale >= precision)
				throw new ArgumentException(
					$"{nameof(scale)} must be a non-negative number less or equal than {nameof(precision)}");
			_scale = scale;

			_onlyPositive = onlyPositive;
		}

		public bool IsValidNumber(string value)
		{
			// Проверяем соответствие входного значения формату N(m,k), в соответствии с правилом, 
			// описанным в Формате описи документов, направляемых в налоговый орган в электронном
			// виде по телекоммуникационным каналам связи:
			// Формат числового значения указывается в виде N(m.к), где m – максимальное количество знаков в числе,
			// включая знак (для отрицательного числа), целую и дробную часть числа без разделяющей десятичной
			// точки, k – максимальное число знаков дробной части числа. Если число знаков дробной части
			// числа равно 0 (т.е. число целое), то формат числового значения имеет вид N(m).

			if (string.IsNullOrEmpty(value))
				return false;

			var match = NumberRegex.Match(value);
			if (!match.Success)
				return false;

			var intPartLength = match.Groups[1].Value.Length + match.Groups[2].Value.Length;
			var fracPartLength = match.Groups[4].Value.Length;

			return intPartLength + fracPartLength <= _precision &&
			       fracPartLength <= _scale &&
			       (!_onlyPositive || match.Groups[1].Value != "-");
		}
	}
}