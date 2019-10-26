using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		private NumberValidator positiveNumberValidator;
		private NumberValidator numberValidator;
		
		[SetUp]
		public void SetUp()
		{
			positiveNumberValidator = new NumberValidator(6, 2, true);
			numberValidator = new NumberValidator(6, 2);
		}

		[TestCase("", false, Description = "Пустая строка не проходит проверку")]
		[TestCase(null, false, Description = "Null не проходит проверку ")]
		[TestCase("abs", false, Description = "Строка не из цифр не проходит проверку")]
		[TestCase("a.sd", false, Description = "Строка не из цифр не проходит проверку")]
		[TestCase("-12345.6", false, Description = "Число, которое длинне максимального размера из-за знака," +
		                                           " не должно проходить по критериям")]
		[TestCase("+12345.6", false,  Description = "Число, которое длинне максимального размера из-за знака," +
		                                            "не должно проходить по критериям")]
		[TestCase("123456.7", false,  Description = "Число, которое длинне максимального размера," +
		                                            "не должно проходить по критериям")]
		[TestCase("000000.00", false, Description = "Число, которое длинне максимального размера," +
		                                            "не должно проходить по критериям")]
		[TestCase("-3.2", true, Description = "Правильное число, которое проходит по всем условиям")]
		[TestCase("-0.0", true, Description = "Правильное число, которое проходит по всем условиям")]
		[TestCase("-0", true, Description = "Правильное число, которое проходит по всем условиям")]
		[TestCase("22.1", true, Description = "Правильное число, которое проходит по всем условиям")]
		
		public void IsValidNumberChecker(string input, bool exceptedAnswer)
		{
			numberValidator.IsValidNumber(input).Should().Be(exceptedAnswer);
		}
		
		[TestCase("", false, Description = "Пустая строка не проходит проверку")]
		[TestCase(null, false, Description = "Null не проходит проверку ")]
		[TestCase("abs", false, Description = "Строка не из цифр не проходит проверку")]
		[TestCase("a.sd", false, Description = "Строка не из цифр не проходит проверку")]
		[TestCase("-1.1", false, Description = "Отрицательное число в OnlyPositiveValidator не проходит проверку")]
		[TestCase("-1123", false, Description = "Отрицательное число в OnlyPositiveValidator не проходит проверку")]
		[TestCase("+3.2", true, Description = "Хорошое число, которое должно проходить проверку")]
		[TestCase("22.1", true, Description =  "Хорошое число, которое должно проходить проверку")]
		[TestCase("+0.0", true, Description =  "Хорошое число, которое должно проходить проверку")]
		[TestCase("0", true, Description =  "Хорошое число, которое должно проходить проверку")]
		public void IsValidNumberCheckerOnlyPositive(string input, bool exceptedAnswer)
		{
			positiveNumberValidator.IsValidNumber(input).Should().Be(exceptedAnswer);
		}

		[TestCase(-1, 1, Description = "Вызов конструктора с отрицательным precision должен выбрасывать ArgumentException")]
		[TestCase(0, 1, Description = "Вызов конструктора с precision = 0 должен выбрасывать ArgumentException")]
		[TestCase(-222222, 1, Description = "Вызов конструктора с отрицательным precision должен выбрасывать ArgumentException")]
		[TestCase(1, -1, Description = "Вызов конструктора с отрицательным scale должен выбрасывать ArgumentException")]
		[TestCase(1, -128, Description = "Вызов конструктора с отрицательным scale должен выбрасывать ArgumentException")]
		[TestCase(1, 2, Description = "Вызов конструктора с scale большим precision должен выбрасывать ArgumentException")]
		[TestCase(1, 1, Description = "Вызов конструктора с scale == precision должен выбрасывать ArgumentException")]
		public void TestNumberValidatorConstructor(int precision, int scale)
		{
			Action action = () => new NumberValidator(precision, scale);
			
			action.ShouldThrow<ArgumentException>();
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