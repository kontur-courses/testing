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
			Action act3 = () => new NumberValidator(0, -1, true);
			Action act4 = () => new NumberValidator(1, 2, true);
			Action act5 = () => new NumberValidator(1, 1, false);

			act1.Should().ThrowExactly<ArgumentException>().WithMessage("precision must be a positive number");
			act2.Should().NotThrow();
			act3.Should().ThrowExactly<ArgumentException>().WithMessage("precision must be a positive number");
			act4.Should().ThrowExactly<ArgumentException>().WithMessage("precision must be a non-negative number less or equal than precision");
			act5.Should().ThrowExactly<ArgumentException>().WithMessage("precision must be a non-negative number less or equal than precision");
		}

		// IsValidForPrecision
		[TestCase("1.23", 4, 2, true, true)]
		[TestCase("+1.23", 4, 2, true, true)]
		[TestCase("-1.23", 4, 2, false, true)]
		[TestCase("1.00", 3, 2, true, true)]
		[TestCase("+1.23", 3, 2, true, false)]
		[TestCase("-1.23", 3, 2, false, false)]
		[TestCase("00.00", 3, 2, true, false)]
		// IsValidNullOrEmpty
		[TestCase("", 17, 4, true, false)]
		[TestCase(null, 17, 4, true, false)]
		// IsValidForScale
		[TestCase("0.000", 17, 2, true, false)]
		[TestCase("+1.23", 17, 2, true, true)]
		[TestCase("0.1", 17, 2, true, true)]
		// IsValidWithMinus
		[TestCase("-1.23", 17, 2, true, false)]
		[TestCase("-1.23", 17, 2, false, true)]
		// IsValidForFormat
		[TestCase("+", 17, 4, true, false)]
		[TestCase("-0.", 17, 4, false, false)]
		[TestCase("+0.", 17, 4, true, false)]
		[TestCase(".1", 17, 4, true, false)]
		[TestCase("-.0", 17, 4, true, false)]
		[TestCase("+1;7", 17, 4, true, false)]
		[TestCase("a.sd", 6, 4, true, false)]
		[TestCase("+1,7", 17, 4, true, true)]
		// IsValidForAdditionalTests
		// начальные тесты, не попавшие в другие подборки
		[TestCase("+0.00", 3, 2, true, false)]
		[TestCase("-0.00", 3, 2, true, false)]
		[TestCase("0.0", 17, 2, true, true)]
		[TestCase("0", 17, 2, true, true)]
		public void IsValidFor(string strNum, int precision, int scale, bool onlyPositive, bool trueOrFalse)
		{
			var isValidNumber = new NumberValidator(precision, scale, onlyPositive).IsValidNumber(strNum);
			if (trueOrFalse == true)
				isValidNumber.Should().BeTrue($"{strNum}{valid}({precision}, {scale}, {onlyPositive})");
			else
				isValidNumber.Should().BeFalse($"{strNum}{notValid}({precision}, {scale}, {onlyPositive})");
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