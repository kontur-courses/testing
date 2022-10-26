using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
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