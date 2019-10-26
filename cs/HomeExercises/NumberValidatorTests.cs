using FluentAssertions;
using NUnit.Framework;
using System;
using System.Text.RegularExpressions;

namespace HomeExercises
{
    public class NumberValidatorTests
    {
        [TestCase(-1, 2, true, Description = "if Precision < 0")]
        [TestCase(0, 2, true, Description = "if Precision == 0")]
        [TestCase(1, -1, false, Description = "if Scale < 0")]
        [TestCase(2, 2, false, Description = "if Scale == Precision")]
        [TestCase(2, 3, false, Description = "if Scale > Precision")]
        public void NumberValidator_ShouldThrow(int precision, int scale, bool onlyPositive)
        {
            Assert.Throws<ArgumentException>(() => new NumberValidator(precision, scale, onlyPositive));
        }

        [TestCase(1, 0, true)]
        [TestCase(1, 0, false)]
        public void NumberValidator_ShouldntThrow(int precision, int scale, bool onlyPositive)
        {
            Assert.DoesNotThrow(() => new NumberValidator(precision, scale, onlyPositive));
        }

        [TestCase(2, 1, false, null, Description = "if value IsNullOrEmpty")]
        [TestCase(2, 1, false, "", Description = "if value IsNullOrEmpty")]
        [TestCase(3, 2, false, "00.00", Description = "if m > precision")]
        [TestCase(3, 2, false, "-0.00", Description = "if m > precision")]
        [TestCase(3, 2, false, "+0.00", Description = "if m > precision")]
        [TestCase(17, 2, false, "0.000", Description = "if k > scale")]
        [TestCase(17, 2, false, "0.", Description = "if value without frac and has dot")]
        [TestCase(17, 2, false, ".0", Description = "if value without int and has dot")]
        [TestCase(3, 2, false, "a.00", Description = "if value is not number")]
        [TestCase(3, 2, false, "0.aa", Description = "if value is not number")]
        [TestCase(1, 0, false, " ", Description = "if value is whitespace")]
        [TestCase(4, 2, true, "-5.00", Description = "if value is negative but must be positive")]
        public void IsValidNumber_MustReturnFalse(int precision, int scale, bool onlyPositive, string value)
        {
            new NumberValidator(precision, scale, onlyPositive).IsValidNumber(value).Should().BeFalse();
        }

        [TestCase(2, 1, true, "0.0")]
        [TestCase(2, 1, true, "+1", Description = "The value may be with '+' and without frac")]
        [TestCase(4, 2, false, "-5,25", Description = "The value may be with '-' and with ','")]
        public void IsValidNumber_MustReturnTrue(int precision, int scale, bool onlyPositive, string value)
        {
            new NumberValidator(precision, scale, onlyPositive).IsValidNumber(value).Should().BeTrue();
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
            /// ниже, видимо, ошибка, которую можно выявить только при code review:
            /// по идее, scale (k – максимальное число знаков дробной части) 
            /// не может быть равен precision (m – максимальное количество знаков в числе)
            /// потому что кроме дробной части в любом случае будет как минимум одна цифра целой части.
            /// В таком случае выбрасывается исключение. И правильно, что выбрасывается, но
            /// в сообщении этого исключения написано "or equal than precision".
            /// Т.е. при запуске например new NumberValidator(2, 2 ....
            /// выбросится исключение, но в сообщении будет написано, что так можно!..
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