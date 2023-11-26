using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[Test]
		public void Constructor_ThrowsArgumentException_WhenPrecisionIsNegative()
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(-1, 0, true));
		}

		[Test]
		public void Constructor_ThrowsArgumentException_WhenScaleIsNegative()
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(1, -1, true));
		}

		[Test]
		public void Constructor_ThrowsArgumentException_WhenScaleIsGreaterThanOrEqualToPrecision()
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(1, 2, true));
		}
		
		[Test]
		public void Constructor_SuccessfullyCreatesObject_OnCorrectInput()
		{
			Assert.DoesNotThrow(() => new NumberValidator(1, 0, true));
		}
		
		[TestCase(null)]
		[TestCase(" ")]
		[TestCase("                   ")]
		[TestCase("")]
		[TestCase("\n")]
		public void IsValidNumber_ReturnsFalse_WhenValueIsNullOrEmpty(string value)
		{
			var numberValidator = new NumberValidator(1, 0, true);
			
			numberValidator.IsValidNumber(value).Should().BeFalse();
		}

		[TestCase(".")]
		[TestCase("a.sd")]
		[TestCase("+-5")]
		[TestCase("1..3")]
		[TestCase("1.")]
		[TestCase(".3")]
		public void IsValidNumber_ReturnsFalse_WhenValueIsNaN(string value)
		{
			var numberValidator = new NumberValidator(2, 1, true);

			numberValidator.IsValidNumber(value).Should().BeFalse();
		}

		[TestCase("00.00", 3, 2)]
		[TestCase("+1.00", 3, 2)]
		[TestCase("-1.00", 3, 2)]
		[TestCase("-1", 1, 0)]
		public void IsValidNumber_ReturnsFalse_WhenIntAndFracPartsAreGreaterThanPrecision(string value, int precision, int scale)
		{
			var numberValidator = new NumberValidator(precision, scale);

			numberValidator.IsValidNumber(value).Should().BeFalse();
		}

		[TestCase("0.00", 3, 1)]
		[TestCase("-123.12", 6, 0)]
		public void IsValidNumber_ReturnsFalse_WhenFracPartIsGreaterThanScale(string value, int precision, int scale)
		{
			var numberValidator = new NumberValidator(precision, scale);

			numberValidator.IsValidNumber(value).Should().BeFalse();
		}

		[TestCase("-1.2", 3, 1)]
		[TestCase("-0.00", 4, 2)]
		public void IsValidNumber_ReturnsFalse_WhenValueIsNegativeButOnlyPositiveIsTrue(string value, int precision, int scale)
		{
			var numberValidator = new NumberValidator(precision, scale, true);

			numberValidator.IsValidNumber(value).Should().BeFalse();
		}

		[TestCase("0.0", 17, 2, true)]
		[TestCase("0.0", 2, 1, false)]
		[TestCase("+1.23", 4, 2, true)]
		[TestCase("-1.23", 4, 2, false)]
		[TestCase("0,1", 2, 1, true)]
		[TestCase("0", 1, 0, true)]
		public void IsValidNumber_ReturnsTrue_OnCorrectInputData(string value, int precision, int scale, bool onlyPositive)
		{
			var numberValidator = new NumberValidator(precision, scale, onlyPositive);

			numberValidator.IsValidNumber(value).Should().BeTrue();
		}
	}

	public class NumberValidator
	{
		private readonly Regex _numberRegex;
		private readonly bool _onlyPositive;
		private readonly int _precision;
		private readonly int _scale;

		public NumberValidator(int precision, int scale = 0, bool onlyPositive = false)
		{
			_precision = precision;
			_scale = scale;
			_onlyPositive = onlyPositive;
			if (precision <= 0)
				throw new ArgumentException("precision must be a positive number");
			if (scale < 0 || scale >= precision)
				throw new ArgumentException("precision must be a non-negative number less or equal than precision");
			_numberRegex = new Regex(@"^([+-]?)(\d+)([.,](\d+))?$", RegexOptions.IgnoreCase);
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

			var match = _numberRegex.Match(value);
			if (!match.Success)
				return false;

			// Знак и целая часть
			var intPart = match.Groups[1].Value.Length + match.Groups[2].Value.Length;
			// Дробная часть
			var fracPart = match.Groups[4].Value.Length;

			if (intPart + fracPart > _precision || fracPart > _scale)
				return false;

			if (_onlyPositive && match.Groups[1].Value == "-")
				return false;
			return true;
		}
	}
}