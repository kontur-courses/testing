using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[TestCase(-1, 2, "Отрицательное количество цифр быть не может")]
		[TestCase(0, 2, "Нулевое количество цифр быть не может")]
		[TestCase(10, -1, "Отрицательное количество цифр после запятой быть не может")]
		[TestCase(5, 5, "Количество цифр после запятой должно быть меньше общего количества цифр")]
		[TestCase(5, 6, "Количество цифр после запятой должно быть меньше общего количества цифр")]
		public void Constructor_ShouldThrowArgumentException_WhenIncorrectParams(
			int precision, int scale, string description = "")
		{
			Action createNewNumberValidator = () =>
			{
				var _ = new NumberValidator(precision, scale);
			};
			createNewNumberValidator.Should().Throw<ArgumentException>(description);
		}
		
		[TestCase(1, 0)]
		[TestCase(8, 1)]
		[TestCase(8, 7)]
		public void Constructor_ShouldNotThrowArgumentException_WhenCorrectParams(
			int precision, int scale)
		{
			Action createNewNumberValidator = () =>
			{
				var _ = new NumberValidator(precision, scale);
			};
			createNewNumberValidator.Should().NotThrow<ArgumentException>("Все передаваемые параметры верны");
		}
		
		
		[TestCase(1, 0, true, "0", "целое число, когда нет десятичныйх знаков")]
		[TestCase(4, 2, true, "135.0", "входит максимум цифр")]
		[TestCase(17, 2, true, "1.35", "входит максимум цифр в дробную часть")]
		[TestCase(17, 8, false, "-1.35", "проходит отрицательное значение")]
		[TestCase(17, 8, true, "1,35", "подходит запятая как разделитель")]
		public void IsValidNumber_ShouldBeTrue_WhenInputIsCorrect(
			int precision, int scale, bool onlyPositive, string numberToCheck, string description = "")
		{
			IsValidNumber_Helper(precision, scale, onlyPositive, numberToCheck, true, description);
		}
		
		
		[TestCase(3, 0, true, "1234", "переполнение цифр")]
		[TestCase(17, 2, true, "1.009", "переполнение дробной части")]
		[TestCase(3, 2, false, "-80.9", "переполнение цифр со знаком")]
		public void IsValidNumber_ShouldBeFalse_WhenDigitsCountMoreThenExpected(
			int precision, int scale, bool onlyPositive, string numberToCheck, string description = "")
		{
			IsValidNumber_Helper(precision, scale, onlyPositive, numberToCheck, false, description);
		}
		
		
		[Test]
		public void IsValidNumber_ShouldBeFalse_WhenGetNegativeValue_ButMustBeOnlyPositive()
		{
			IsValidNumber_Helper(17, 2, true, "-7", false);
		}
		
		
		[TestCase("not.number", "не числовое значение")]
		[TestCase("4:78", "неправильный разделитель")]
		[TestCase("*4.78", "неправильный знак перед числом")]
		public void IsValidNumber_ShouldBeFalse_WhenNotNumberValue(
			string numberToCheck, string description = "")
		{
			IsValidNumber_Helper(17, 10, true, numberToCheck, false, description);
		}
		
		
		[TestCase("")]
		[TestCase(" ")]
		[TestCase(null)]
		public void IsValidNumber_ShouldBeFalse_WhenNullOrEmptyValue(string numberToCheck)
		{
			IsValidNumber_Helper(17, 10, true, numberToCheck, false);
		}
		
		
		private static void IsValidNumber_Helper(
			int precision, int scale, bool onlyPositive, string numberToCheck, bool shouldBe, string description = "")
		{
			var numberValidator = new NumberValidator(precision, scale, onlyPositive);
			numberValidator.IsValidNumber(numberToCheck).Should().Be(shouldBe, description);
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
				throw new ArgumentException("scale must be a non-negative number less than precision");
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