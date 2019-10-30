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
		[TestCase(-2, -1, TestName = "Дробная часть больше чем длина числа при отрицательных значениях")]
		public void Constructor_WithArgumentException(int precision, int scale = 0, bool onlyPositive = false)
		{
			Action action = () => new NumberValidator(precision, scale, onlyPositive);
			action.ShouldThrow<ArgumentException>();
		}
		
		[TestCase("0.00", true, TestName = "Ноль с нулевой дробной частью при только положительных")]
		[TestCase("0.00", false, TestName = "Ноль с нулевой дробной частью")]
		[TestCase("0", true, TestName = "Ноль положительных")]
		[TestCase("0", false, TestName = "Ноль")]
		[TestCase("+0", true, TestName = "Ноль с плюсом при только положительных")]
		[TestCase("-0", false, TestName = "Ноль с минусом")]
		[TestCase("1111", false, TestName = "Число без дробной части")]
		[TestCase("1111.12", false, TestName = "Число с дробной частью")]
		[TestCase("+1111.12", true, TestName = "Положительное число со знаком плюс")]
		[TestCase("-1111", false, TestName = "Отрицательное число с дробно частью")]
		public void IsValidNumber_ReturnTrue(string value, bool onlyPositive)
		{
			var numberValidator = new NumberValidator(17, 4, onlyPositive);
			numberValidator.IsValidNumber(value).Should().BeTrue();
		}
		
		
		[TestCase(null, false, TestName = "Значение null")]
		[TestCase("", false, TestName = "Значение - пустая строка")]
		[TestCase("+ad", false, TestName = "Значение - это строка с буквами")]
		[TestCase(" ", false, TestName = "Значение пробел")]
		[TestCase("11111", false, TestName = "Длина числа больше установлена")]
		[TestCase("+1111", false, TestName = "Длина числа плюс знак \"плюс\" больше установленной")]
		[TestCase("-11", true, TestName = "Отрицательное число при только положительных")]
		[TestCase("-1.1", false, TestName = "Отрицательное число с дробной частью при только положительных")]
		[TestCase("-0", true, TestName = "Отрицательный ноль при только положительных")]
		[TestCase("1.111", false, TestName = "Длинна дробной части больше чем установлена")]
		
		public void IsValidNumber_ReturnFalse(string value, bool onlyPositive)
		{
			var numberValidator = new NumberValidator(4, 2, onlyPositive);
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