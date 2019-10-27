using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
    public class NumberValidatorTests
    {
        [Description("Проверка успешного создания экземпляра класса NumberValidator")]
        [TestCase(1, 0, false)]
        [TestCase(1, 0, true)]
        public void Not_Fail_On_NumberValidator_Creation(int precision, int scale, bool onlyPositive)
        {
            Action act = () => new NumberValidator(precision, scale, onlyPositive);
            act
                .Should()
                .NotThrow();
        }

        [Description("Проверка выдачи ошибок при создании экземпляра класса NumberValidator")]
        [TestCase(-1, 2, true, "precision must be a positive number")]
        [TestCase(1, -2, true, "precision must be a non-negative number less or equal than precision")]
        [TestCase(1, 2, true, "precision must be a non-negative number less or equal than precision")]
        public void Fail_On_NumberValidator_Creation(int precision, int scale, bool onlyPositive, string message)
        {
            Action act = () => new NumberValidator(precision, scale, onlyPositive);
            act
                .Should()
                .Throw<ArgumentException>()
                .WithMessage(message);
        }

        [Description("Проверка обработки некорректного ввода")]
        [TestCase(4, 2, true, "a.sd", ExpectedResult = false)]
        [TestCase(4, 2, true, null, ExpectedResult = false)]
        public bool Incorrect_Input(int precision, int scale, bool onlyPositive, string value)
        {
            return new NumberValidator(precision, scale, onlyPositive).IsValidNumber(value);
        }

        [Description("Фэйл, когда intPart + fracPart > precision || fracPart > scale")]
        [TestCase(17, 2, true, "0.000", ExpectedResult = false)]
        [TestCase(3, 2, true, "+1.23", ExpectedResult = false)]
        public bool Incorrect_Input_Lenght(int precision, int scale, bool onlyPositive, string value)
        {
            return new NumberValidator(precision, scale, onlyPositive).IsValidNumber(value);
        }

        [Description("Фэйл, когда разрешены только положительные числа, а передано отрицательное")]
        [TestCase(4, 2, true, "-1.23", ExpectedResult = false)]
        public bool Incorrect_Sign(int precision, int scale, bool onlyPositive, string value)
        {
            return new NumberValidator(precision, scale, onlyPositive).IsValidNumber(value);
        }

        [Description("Проверка корректных значений")]
        [TestCase(17, 2, true, "0.0", ExpectedResult = true)]
        [TestCase(4, 2, false, "-1.23", ExpectedResult = true)]
        [TestCase(4, 2, true, "1", ExpectedResult = true)]
        [TestCase(5, 2, false, "+33.23", ExpectedResult = true)]
        public bool Correct_Input(int precision, int scale, bool onlyPositive, string value)
        {
            return new NumberValidator(precision, scale, onlyPositive).IsValidNumber(value);
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