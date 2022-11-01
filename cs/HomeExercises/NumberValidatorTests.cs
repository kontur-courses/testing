using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		// Проверяем все возможные ошибки, которые вызывает конструктор при вводе неверных данных
		[TestCase(-1, 2, "precision must be a positive number")]
		[TestCase(0, 2, "precision must be a positive number")]
		[TestCase(2, -2, "precision must be a non-negative number less or equal than precision")]
		[TestCase(2, 4, "precision must be a non-negative number less or equal than precision")]
		public void NumberRegistry_ConstructorWithIncorrectInput_ShouldThrowArgumentException(int precision, int scale, string exeptionMessage)
		{
			Action act = () => NumberRegistry.GetCurrentNumber(precision, scale);

			act.Should().Throw<ArgumentException>(exeptionMessage);
		}
		
		// Проверяем нормальную работу конструктора без ошибок
		[Test]
		public void NumberRegistry_NormalInput_ShouldNotThrowExceptions()
		{
			Action act = () => NumberRegistry.GetCurrentNumber(1, 0);

			act.Should().NotThrow();
		}

		// Проверяем метод на числах, соответствующих шаблону
		[TestCase(17, 2, true, "0.0")]
		[TestCase(17, 2, true, "0")]
		public void IsValidNumber_ValidValue_ShouldReturnTrue(int precision, int scale, bool onlyPositive, string value)
		{
			var numberValidator = NumberRegistry.GetCurrentNumber(precision, scale, onlyPositive);

			var validFlag = numberValidator.IsValidNumber(value);

			validFlag.Should().BeTrue();
		}

		// Проверяем метод на ввод отрицательного числа валидатору, у которого стоит флаг "только положительные числа"
		[Test]
		public void IsValidNumber_NegativeInputWithOnlyPositiveValidator_ShouldReturnFalse()
		{
			var numberValidator = NumberRegistry.GetCurrentNumber(3, 2, true);

			var validFlag = numberValidator.IsValidNumber("-0.00");

			validFlag.Should().BeFalse();
		}
		
		// Проверяем метод на вход строки, не соответствующей шаблону
		[Test]
		public void IsValidNumber_InputWithIncorrectPattern_ShouldReturnFalse()
		{
			var numberValidator = NumberRegistry.GetCurrentNumber(3, 2, true);

			var validFlag = numberValidator.IsValidNumber("a.sd");

			validFlag.Should().BeFalse();
		}

		// Проеряем метод на исключение числа, у которого дробная часть не соответствует предельному scale
		[Test]
		public void IsValidNumber_FractionalPartLongerThanScale_ShouldReturnFalse()
		{
			var numberValidator = NumberRegistry.GetCurrentNumber(17, 2, true);

			var validFlag = numberValidator.IsValidNumber("0.000");

			validFlag.Should().BeFalse();
		}

		// Проеряем метод на исключение числа, у которого общая длина не соответствует предельному precision
		[TestCase(3, 2, true, "+0.00")]
		[TestCase(3, 2, true, "00.00")]
		public void IsValidNumber_NumLengthLongerThanPrecision_ShouldReturnFalse(int precision, int scale, bool onlyPositive, string value)
		{
			var numberValidator = NumberRegistry.GetCurrentNumber(precision, scale, onlyPositive);

			var validFlag = numberValidator.IsValidNumber(value);

			validFlag.Should().BeFalse();
		}

		// Проеряем метод на исключение пустой строки или нулевой ссылки
		[TestCase(3, 2, true, null)]
		[TestCase(3, 2, true, "")]
		public void IsValidNumber_NullOrEmptyValue_ShouldReturnFalse(int precision, int scale, bool onlyPositive, string value)
		{
			var numberValidator = NumberRegistry.GetCurrentNumber(precision, scale, onlyPositive);

			var validFlag = numberValidator.IsValidNumber(value);

			validFlag.Should().BeFalse();
		}

		// Проверка метода на то, что при отсутствии знака перед числом мы его считаем положительным
		[Test]
		public void IsValidNumber_NumWithoutSign_ShouldReturnTrue()
		{
			var numberValidator = NumberRegistry.GetCurrentNumber(17, 2, true);

			var validFlag = numberValidator.IsValidNumber("0.00");

			validFlag.Should().BeTrue();
		}

		// Проверка корректной работы метода при верном аргументе
		[Test]
		public void IsValidNumber_CommonInput_ShouldReturnTrue()
		{
			var numberValidator = NumberRegistry.GetCurrentNumber(4, 2, true);

			var validFlag = numberValidator.IsValidNumber("+1.23");

			validFlag.Should().BeTrue();
		}
	}
}

	public class NumberRegistry
	{
		public static NumberValidator GetCurrentNumber(int precision, int scale = 0, bool onlyPositive = false)
		{
			return new NumberValidator(precision, scale, onlyPositive);
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
