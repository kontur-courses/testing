using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		readonly string valid = " is valid number for NumberValidator";
		readonly string notValid = " is not valid number for NumberValidator";

		[Test]
		public void TestConstructor()
		{
			Action act1 = () => new NumberValidator(-1, 2, true);
			Action act2 = () => new NumberValidator(1, 0, true);
			Action act5 = () => new NumberValidator(0, -1, true);
			Action act6 = () => new NumberValidator(1, 2, true);
			Action act7 = () => new NumberValidator(1, 1, false);

			act1.Should().ThrowExactly<ArgumentException>().WithMessage("precision must be a positive number");
			act2.Should().NotThrow();
			act5.Should().ThrowExactly<ArgumentException>().WithMessage("precision must be a positive number");
			act6.Should().ThrowExactly<ArgumentException>().WithMessage("precision must be a non-negative number less or equal than precision");
			act7.Should().ThrowExactly<ArgumentException>().WithMessage("precision must be a non-negative number less or equal than precision");
		}

		[Test]
		public void IsValidForPrecision()
		{
			new NumberValidator(4, 2, true).IsValidNumber("1.23").Should().BeTrue($"1.23{valid}(4, 2, true)");
			new NumberValidator(4, 2, true).IsValidNumber("+1.23").Should().BeTrue($"+1.23{valid}(4, 2, true)");
			new NumberValidator(4, 2, false).IsValidNumber("-1.23").Should().BeTrue($"-1.23{valid}(4, 2, false)");
			new NumberValidator(3, 2, true).IsValidNumber("1.00").Should().BeTrue($"1.00{valid}(3, 2, true)");
			new NumberValidator(3, 2, true).IsValidNumber("+1.23").Should().BeFalse($"+1.23{notValid}(3, 2, true)");
			new NumberValidator(3, 2, false).IsValidNumber("-1.23").Should().BeFalse($"-1.23{notValid}(3, 2, false)");
			new NumberValidator(3, 2, true).IsValidNumber("00.00").Should().BeFalse($"00.00{notValid}(3, 2, true)");
		}

		[Test]
		public void IsValidNullOrEmpty()
		{
			new NumberValidator(17, 4, true).IsValidNumber("").Should().BeFalse($"String.Empty{notValid}(17, 4, true)");
			new NumberValidator(17, 4, true).IsValidNumber(null).Should().BeFalse($"null{notValid}(17, 4, true)");
		}

		[Test]
		public void IsValidForScale()
		{
			new NumberValidator(17, 2, true).IsValidNumber("0.000").Should().BeFalse($"0.000{notValid}(17, 2, true)");
			new NumberValidator(17, 2, true).IsValidNumber("+1.23").Should().BeTrue($"+1.23{valid}(17, 2, true)");
			new NumberValidator(17, 2, true).IsValidNumber("0.1").Should().BeTrue($"0.1{valid}(17, 2, true)");
		}

		[Test]
		public void IsValidWithMinus()
		{
			new NumberValidator(17, 2, true).IsValidNumber("-1.23").Should().BeFalse($"-1.23{notValid}(17, 2, true)");
			new NumberValidator(17, 2, false).IsValidNumber("-1.23").Should().BeTrue($"-1.23{valid}(17, 2, false)");
		}

		[Test]
		public void IsValidForFormat()
		{
			new NumberValidator(17, 4, true).IsValidNumber("+").Should().BeFalse($"+{notValid}(17, 4, true)");
			new NumberValidator(17, 4, false).IsValidNumber("-0.").Should().BeFalse($"{notValid}(17, 4, false)");
			new NumberValidator(17, 4, true).IsValidNumber("+0.").Should().BeFalse($"+0.{notValid}(17, 4, true)");
			new NumberValidator(17, 4, true).IsValidNumber(".1").Should().BeFalse($".1{notValid}(17, 4, true)");
			new NumberValidator(17, 4, true).IsValidNumber("-.0").Should().BeFalse($"-.0{notValid}(17, 4, true)");
			new NumberValidator(17, 4, true).IsValidNumber("+1;7").Should().BeFalse($"+1;7{notValid}(17, 4, true)");
			new NumberValidator(6, 4, true).IsValidNumber("a.sd").Should().BeFalse($"a.sd{notValid}(6, 4, true)");
			new NumberValidator(17, 4, true).IsValidNumber("+1,7").Should().BeTrue($"+1,7{valid}(17, 4, true)");
		}

		[Test]
		public void IsValidForAdditionalTests1() //начальные тесты, не попавшие в другие подборки
		{
			new NumberValidator(3, 2, true).IsValidNumber("+0.00").Should().BeFalse($"+0.00{notValid}(3, 2, true)");
			new NumberValidator(3, 2, true).IsValidNumber("-0.00").Should().BeFalse($"-0.00{notValid}(3, 2, true)");
			new NumberValidator(17, 2, true).IsValidNumber("0.0").Should().BeTrue($"0.0{valid}(17, 2, true)");
			new NumberValidator(17, 2, true).IsValidNumber("0").Should().BeTrue($"0{valid}(17, 2, true)");

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