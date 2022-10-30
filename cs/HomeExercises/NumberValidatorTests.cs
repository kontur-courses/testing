using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[Test]
		[Category(nameof(Ctor_ThrowException_WhenArgumentsInclude))]
		[Description("Проверка выдачи исключения при некорректных аргументах конструктора")]
		[TestCase(-1, 2, true, TestName = "negative precision and only positive flag")]
		[TestCase(-1, 2, false, TestName = "negative precision and only negative flag")]
		[TestCase(0, TestName = "zero precision")]
		[TestCase(1, -1, TestName = "scale less than zero")]
		[TestCase(2, 3, TestName = "scale more precision")]
		[TestCase(5, 5, TestName = "scale equals precision")]
		public void Ctor_ThrowException_WhenArgumentsInclude(int precision, int scale = 0, bool onlyPositive = false)
		{
			Action action = () => new NumberValidator(precision, scale, onlyPositive);
			action.Should().Throw<ArgumentException>();
		}

		[Test]
		[Category(nameof(Ctor_NotThrowException_WhenValidArgumentsInclude))]
		[Description("Проверка отсутствия исключений при корректных аргументах конструктора")]
		[TestCase(1, TestName = "construct with valid precision and default arguments")]
		[TestCase(1, 0, true, TestName = "positive precision, zero scale and only positive")]
		[TestCase(2, 1, false, TestName = "positive precision, zero scale and only negative")]
		public void Ctor_NotThrowException_WhenValidArgumentsInclude(int precision, int scale = 0, bool onlyPositive = false)
		{
			Action action = () => new NumberValidator(precision, scale, onlyPositive);
			action.Should().NotThrow();
		}

		[Test]
		[Category(nameof(IsValidNumber_ReturnsTrue_When))]
		[Description("Проверка, что метод IsValidNumber возвращает true при корректных аргументах")]
		[TestCase(2, 1, true, "0.0", TestName = "dot delimiter")]
		[TestCase(1, 0, true, "0", TestName = "zero scale without delimiter")]
		[TestCase(2, 1, true, "0", TestName = "single scale without delimiter")]
		[TestCase(2, 1, true, "0,0", TestName = "comma delimiter")]
		[TestCase(4, 2, true, "+1.23", TestName = "value is positive number with sign")]
		[TestCase(3, 2, true, "1.23", TestName = "value is positive number")]
		[TestCase(4, 2, false, "-1.23", TestName = "value is negative number with sign")]
		[TestCase(4, 1, true, "001.0", TestName = "value starts with multiple zeros")]
		[TestCase(4, 3, true, "0.000", TestName = "fraction consists of many zeros")]
		[TestCase(4, 0, false, "+000", TestName = "value starts with plus sign and multiple zeros")]
		[TestCase(4, 0, false, "-000", TestName = "value starts with minus sign and multiple zeros")]
		public void IsValidNumber_ReturnsTrue_When(int precision, int scale, bool onlyPositive, string value)
		{
			var numberValidator = new NumberValidator(precision, scale, onlyPositive);

			numberValidator.IsValidNumber(value).Should().BeTrue();
		}

		[Test]
		[Description("Проверка, что метод IsValidNumber возвращает true при очень больших числах")]
		public void IsValidNumber_ReturnsTrue_WhenValueIsBig()
		{
			var numberValidator = new NumberValidator(1000, 0);

			numberValidator.IsValidNumber(new string('9',1000)).Should().BeTrue();
		}

		[Test]
		[Description("Проверка, что метод IsValidNumber возвращает true при очень большой дробной части")]
		public void IsValidNumber_ReturnsTrue_WhenFractionIsBig()
		{
			var numberValidator = new NumberValidator(1000, 999);

			numberValidator.IsValidNumber("0." + new string('9', 999)).Should().BeTrue();
		}

		[Test]
		[Category(nameof(IsValidNumber_ReturnsFalse_When))]
		[Description("Проверка, что метод IsValidNumber возвращает true при корректных аргументах")]
		[TestCase(3, 2, true, "90.00", TestName = "small precision")]
		[TestCase(4, 1, true, "90.00", TestName = "small scale")]
		[TestCase(1, 0, false, "-0", TestName = "sign is not taken into account exactly")]
		[TestCase(5, 2, false, "+-0.00", TestName = "value has two signs")]
		[TestCase(3, 2, true, "a.sd", TestName = "value are text instead of numbers")]
		[TestCase(3, 2, true, null, TestName = "null input value")]
		[TestCase(3, 2, true, "", TestName = "empty input value")]
		[TestCase(4, 2, true, "-1.23", TestName = "negative value but expect positive")]
		[TestCase(4, 2, false, "-.23", TestName = "negative value without integer part")]
		public void IsValidNumber_ReturnsFalse_When(int precision, int scale, bool onlyPositive, string value)
		{
			var numberValidator = new NumberValidator(precision, scale, onlyPositive);

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