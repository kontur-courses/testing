using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	[TestFixture]
	public class NumberValidatorTests
	{
		[TestCase(-1, 2, TestName = "Кол-во знаков отрицательное")]
		[TestCase(0, 2, TestName = "Кол-во знаков равно нулю")]
		[TestCase(1, -1, TestName = "Кол-во знаков дробной части числа отрицательно")]
		[TestCase(1, 2, TestName = "Дробная часть больше чем длина числа")]
		[TestCase(2, 2, TestName = "Длина дробной части равна длине числа")]
		[TestCase(-2, -1, TestName = "Дробная часть больше чем длина числа при отрицательных значениях")]
		public void Constructor_WithArgumentException(int precision, int scale = 0, bool onlyPositive = false)
		{
			Action action = () => new NumberValidator(precision, scale, onlyPositive);
			action.ShouldThrow<ArgumentException>();
		}
		
		[TestCase(2, 0, TestName = "Длина дробной части равна нулю")]
		[TestCase(2, 1, TestName = "Длины числа и дробной части положительные")]
		public void Constructor_DoesNotThrow(int precision, int scale = 0, bool onlyPositive = false)
		{
			Action action = () => new NumberValidator(precision, scale, onlyPositive);
			action.ShouldNotThrow();
		}
		
		
		[TestCase("1111", false, TestName = "Число без дробной части")]
		[TestCase("1111.12", false, TestName = "Число с дробной частью")]
		[TestCase("+1111", false, TestName = "Число c  плюсом")]
		[TestCase("+1111.12", true, TestName = "Положительное число со знаком плюс")]
		[TestCase("-1111.12", false, TestName = "Отрицательное число с дробной частью")]
		[TestCase("-12345678.90", false, TestName = "Отрицательное число из всех цифр")]
		[TestCase("+23,90", false, TestName = "Число с раздилительной запятой")]
		public void IsValidNumber_ReturnTrue_WhenNumberHasStandardForm(string value, bool onlyPositive)
		{
			var numberValidator = new NumberValidator(17, 4, onlyPositive);
			numberValidator.IsValidNumber(value).Should().BeTrue();
		}
		
		[TestCase("0.00", true, TestName = "Ноль с нулевой дробной частью при только положительных")]
		[TestCase("0.00", false, TestName = "Ноль с нулевой дробной частью")]
		[TestCase("0", true, TestName = "Ноль при только положительных")]
		[TestCase("0", false, TestName = "Ноль")]
		[TestCase("+0", true, TestName = "Ноль с плюсом при только положительных")]
		[TestCase("-0", false, TestName = "Ноль с минусом")]
		public void IsValidNumber_ReturnTrue_WhenNumberIsZero(string value, bool onlyPositive)
		{
			var numberValidator = new NumberValidator(17, 4, onlyPositive);
			numberValidator.IsValidNumber(value).Should().BeTrue();
		}
		
		
		[TestCase(null, TestName = "Значение null")]
		[TestCase("", TestName = "Значение - пустая строка")]
		[TestCase("+ad", TestName = "Значение - это строка с буквами")]
		[TestCase(" ", TestName = "Значение - пробел")]
		[TestCase("O.1", TestName = "Буква О вместо нуля")]
		public void IsValidNumber_ReturnFalse_WhenInputIsNotNumber(string value, bool onlyPositive = false)
		{
			var numberValidator = new NumberValidator(4, 2, onlyPositive);
			numberValidator.IsValidNumber(value).Should().BeFalse();
		}
		
		[TestCase("-11", TestName = "Отрицательное число при только положительных")]
		[TestCase("-1.1", TestName = "Отрицательное число с дробной частью при только положительных")]
		[TestCase("-0", TestName = "Отрицательный ноль при только положительных")]
		public void IsValidNumber_ReturnFalse_WhenNumberOnlyPositive(string value, bool onlyPositive = true)
		{
			var numberValidator = new NumberValidator(17, 4, onlyPositive);
			numberValidator.IsValidNumber(value).Should().BeFalse();
		}
		
		[TestCase("11111", TestName = "Число без целой части")]
		[TestCase("+1111", TestName = "Число с плюсом")]
		[TestCase("1.111", TestName = "Дробная части больше чем установлена")]
		[TestCase("+11.11", TestName = "Число с дробной частью")]
		public void IsValidNumber_ReturnFalse_WhenLengthNumberGreaterThanSet(string value, bool onlyPositive = false)
		{
			var numberValidator = new NumberValidator(4, 2, onlyPositive);
			numberValidator.IsValidNumber(value).Should().BeFalse();
		}
		
		
		[TestCase("+-1", TestName = "Плюс минус число")]
		[TestCase("1E2", TestName = "Число с экспонентой")]
		[TestCase(".1", TestName = "Без целой части")]
		[TestCase("1.", TestName = "Без дробной части, но с разделяющей точкой")]
		[TestCase("pi", TestName = "Число пи(pi)")]
		public void IsValidNumber_ReturnFalse_WhenNumberHasNonStandardForm(string value, bool onlyPositive = false)
		{
			var numberValidator = new NumberValidator(10, 5, onlyPositive);
			numberValidator.IsValidNumber(value).Should().BeFalse();
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
				throw new ArgumentException("scale must be a non-negative number less or equal than precision");
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