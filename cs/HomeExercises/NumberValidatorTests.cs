using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[TestCase(5, 0, TestName = "Precision is pos, scale = 0")]
		[TestCase(5, 1, TestName = "Precision is pos, scale < precision")]
		public void Constructor_WhenCorrectArgs_ShouldNoThrowEx(int precision, int scale)
		{
			Action action = () => new NumberValidator(precision, scale);
			action.Should().NotThrow<ArgumentException>();
		}

		[TestCase(0, 0, TestName = "Precision = 0, Scale = 0")]
		[TestCase(-5, 0, TestName = "Precision is neg, Scale = 0")]
		[TestCase(-5, 5, TestName = "Precision is neg, Scale is pos")]
        [TestCase(5, 5, TestName = "Precision = Scale")]
		[TestCase(5, 6, TestName = "Precision < Scale")]
		[TestCase(5, -5, TestName = "Precision is pos, Scale < 0")]
		public void Constructor_WhenIncorrectArgs_ShouldThrowEx(int precision, int scale)
		{
			Action action = () => new NumberValidator(precision, scale);
			action.Should().Throw<ArgumentException>();
		}

        [TestCase("5.5", TestName = "When point separator")]
		[TestCase("5,5", TestName = "When comma separator")]
		public void IsValidNumber_WhenAnySeparators_ShouldBeTrue(string value)
		{
			new NumberValidator(8, 2)
				.IsValidNumber(value).Should().BeTrue();
		}

		[TestCase("555555.88", TestName = "When INT and DEC parts is corresponds")]
		[TestCase("5555555.8", TestName = "When INT is correspond, DEC is less")]
		[TestCase("88888888", TestName = "When only INT")]
		[TestCase("55.88", TestName = "When INT less, DEC is correspond")]
		[TestCase("5.5", TestName = "When INT and DEC is less")]
		public void IsValidNumber_WhenCorrectArgs_ShouldBeTrue(string value)
		{
			var validator = new NumberValidator(8, 2);
			validator.IsValidNumber(value).Should().BeTrue();
		}

		[TestCase("1.7777777", TestName = "DEC is greater than Scale")]
		[TestCase("999999999", TestName = "DEC is greater than Precision")]
		[TestCase("88888888.9", TestName = "Length is greater than Precision")]
		[TestCase(".", TestName = "Only separator case")]
		public void IsValidNumber_WhenIncorrectArgs_ShouldBeFalse(string value)
		{
			var validator = new NumberValidator(8, 2);
			validator.IsValidNumber(value).Should().BeFalse();
		}

		[TestCase("-33.3", TestName = "Explicit negative sign")]
		[TestCase("+66.6", TestName = "Explicit positive sign")]
		[TestCase("00.00", TestName = "Explicit double zero prefix")]
        public void IsValidNumber_WhenExplicitSign_ShouldBeTrue(string value)
		{
			var validator = new NumberValidator(8, 2);
			validator.IsValidNumber(value).Should().BeTrue();
		}

		[TestCase("-123456.78", TestName = "Explicit negative sign overflow")]
		[TestCase("+123456.78", TestName = "Explicit positive sign overflow")]
		public void IsValidNumber_WhenExplicitSignOverflow_ShouldBeFalse(string value)
		{
			var validator = new NumberValidator(8, 2);
			validator.IsValidNumber(value).Should().BeFalse();
		}

		[TestCase("-1.11", TestName = "When float is negative")]
		[TestCase("-1", TestName = "When INT is negative")]
		public void IsValidNumber_WhenIsOnlyPositive_ShouldBeFalse(string value)
		{
			var validator = new NumberValidator(8, 2, onlyPositive: true);
			validator.IsValidNumber(value).Should().BeFalse();
		}

		[TestCase("", TestName = "Empty value")]
		[TestCase(" ", TestName = "Spaces string value")]		
		[TestCase(null, TestName = "Null value")]
		[TestCase("77..5", TestName = "Double separator")]
		[TestCase("++55.5", TestName = "Double explicit plus")]
		[TestCase("--66.5", TestName = "Double explicit minus")]
		[TestCase("5555@5", TestName = "Wrong separator")]
		[TestCase(" 333.3", TestName = "Space prefix in value")]
		[TestCase("666.6 ", TestName = "Space postfix in value")]
        [TestCase("qwe77.7c", TestName = "Not only numbers")]
		[TestCase("qwerty.qw", TestName = "Only chars")]
        [TestCase(".2", TestName = "True_WithoutIntegerPart")]
        [TestCase("ccc\t5.6\t6", TestName = "When escapes and chars")]
		public void IsValidNumber_WhenValueIsNaN_ShouldBeFalse(string value)
		{
			var validator = new NumberValidator(8, 4, onlyPositive: false);
			validator.IsValidNumber(value).Should().BeFalse();
		}

		[TestCase("11.111", true, TestName = "Pos value and onlyPositive is true")]
		[TestCase("22.222", false, TestName = "Pos value and onlyPositive is false")]
		[TestCase("+33.33", true, TestName = "Pos value with explicit plus and onlyPositive is true")]
		[TestCase("+44.44", false, TestName = "Pos value ith explicit plus and onlyPositive is false")]
		[TestCase("-55.55", false, TestName = "Neg value and onlyPositive is false")]
		public void IsValidNumber_IsCorrectValuesAndOnlyPosFlag_ShouldBeTrue(string value, bool onlyPositive)
		{
			var validator = new NumberValidator(5, 3, onlyPositive);
			validator.IsValidNumber(value).Should().BeTrue();
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