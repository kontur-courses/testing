using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[TestCase(-1, 2, true, "Отрицательное количество цифр быть не может")]
		[TestCase(0, 2, true, "Нулевое количество цифр быть не может")]
		[TestCase(10, -1, true, "Отрицательное количество цифр после запятой быть не может")]
		[TestCase(5, 5, true, "Количество цифр после запятой должно быть меньше всего количества цифр")]
		[TestCase(5, 6, true, "Количество цифр после запятой должно быть меньше всего количества цифр")]
		public void NumberValidator_ThrowArgumentException_WhenIncorrectParams(
			int precision, int scale, bool onlyPositive, string description = "")
		{
			Action createNewNumberValidator = () =>
			{
				var numberValidator = new NumberValidator(precision, scale, onlyPositive);
			};
			createNewNumberValidator.Should().Throw<ArgumentException>(description);
		}
		
		[TestCase(3, 1, true)]
		[TestCase(3, 1, false)]
		[TestCase(3, 0, true)]
		[TestCase(3, 0, false)]
		public void NumberValidator_DoesNotThrowArgumentException_WhenCorrectParams(
			int precision, int scale, bool onlyPositive)
		{
			Action createNewNumberValidator = () =>
			{
				var numberValidator = new NumberValidator(precision, scale, onlyPositive);
			};
			createNewNumberValidator.Should().NotThrow<ArgumentException>("Все передаваемые параметры верны");
		}
		
		
		[TestCase(17, 2, true, "0", "целое число")]
		[TestCase(17, 0, true, "0", "целое число, когда нет десятичныйх знаков")]
		[TestCase(17, 2, true, "0.0", "десятичное число")]
		[TestCase(4, 2, true, "+10.3", "входит максимум элементов всего")]
		[TestCase(4, 2, true, "1.35", "входит максимум элементов в дробную часть")]
		[TestCase(4, 2, false, "-1.23", "проходит отрицательное значение")]
		[TestCase(4, 2, true, "1,23", "подходит запятая как разделитель")]
		public void IsValidNumber_ReturnTrue_WhenInputIsCorrect(
			int precision, int scale, bool onlyPositive, string numberToCheck, string description = "")
		{
			var numberValidator = new NumberValidator(precision, scale, onlyPositive);
			numberValidator.IsValidNumber(numberToCheck).Should().Be(true, description);
		}

		[TestCase(3, 2, true, "123.0", "переполнение цифр")]
		[TestCase(3, 2, false, "-80.9", "переполнение цифр со знаком")]
		[TestCase(17, 2, true, "1.009", "переполнение дробной части")]
		[TestCase(17, 2, true, "-7", "отрицательное число в положительное")]
		[TestCase(17, 9, true, "not.number", "не числовое значение")]
		[TestCase(17, 9, true, "15.number", "не числовое значение")]
		[TestCase(17, 5, true, "4:78", "неправильный разделитель")]
		[TestCase(17, 5, true, "*4.78", "неправильный знак перед числом")]
		[TestCase(17, 9, true, "", "пустая строка -> неверно")]
		[TestCase(17, 9, true, " ", "пробельная строка -> неверно")]
		public void IsValidNumber_ReturnTrue_WhenInputIsIncorrect(
			int precision, int scale, bool onlyPositive, string numberToCheck, string description = "")
		{
			var numberValidator = new NumberValidator(precision, scale, onlyPositive);
			numberValidator.IsValidNumber(numberToCheck).Should().Be(false, description);
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