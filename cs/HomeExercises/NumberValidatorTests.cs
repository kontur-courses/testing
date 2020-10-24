using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[TestCase("0", 4, 2, TestName = "Unsigned number without fractional part")]
		[TestCase("+0", 4, 2, TestName = "Positive number without fractional part")]
		[TestCase("-0", 4, 2, TestName = "Negative number without fractional part")]
		[TestCase("0.0", 4, 2, TestName = "Unsigned number with separator dot and fractional part length 1")]
		[TestCase("0,0", 4, 2, TestName = "Unsigned number with separator comma and fractional part length 1")]
		[TestCase("+0.0", 4, 2, TestName = "Positive number with fractional part length 1")]
		[TestCase("-0.0", 4, 2, TestName = "Negative number with fractional part length 1")]
		[TestCase("010", 4, 2, TestName = "Unsigned number starts with 0 without fractional part")]
		[TestCase("+010", 4, 2, TestName = "Positive number starts with 0 without fractional part")]
		[TestCase("-010", 4, 2, TestName = "Negative number starts with 0 without fractional part")]
		[TestCase("01.0", 4, 2, TestName = "Unsigned number starts with 0 with fractional part")]
		[TestCase("-01.0", 4, 2, TestName = "Negative number starts with 0 with fractional part")]
		[TestCase("+01.0", 4, 2, TestName = "Positive number starts with 0 with fractional part")]
		[TestCase("1.45", 4, 2, TestName = "Unsigned number with fractional part length more 1")]
		[TestCase("1.00000", 10, 9, TestName = "Fractional part consists only of zeros")]
		[TestCase("+1.45", 4, 2, TestName = "Positive number with fractional part length more 1")]
		[TestCase("-1.45", 4, 2, TestName = "Negative number with fractional part length more 1")]
		[TestCase("൨", 4, 2, TestName = "Malayalam numbers")]
		[TestCase("۴", 4, 2, TestName = "Eastern Arabic numerals")]
		[TestCase("٥", 4, 2, TestName = "Arabic numerals")]
		[TestCase("000", 4, 2, TestName = "Number consists of only zeros without fractional part")]
		[TestCase("11", 2, 1, TestName = "Precision of number equal to specified precision/without fractional part")]
		[TestCase("11.11", 5, 2, TestName = "Scale of number equal to specified scale")]
		[TestCase("11.11", 5, 3, TestName = "Scale of number less than specified scale")]
		[TestCase("+11", 5, 3,true,  TestName = "Positive number when only positive")]
		[TestCase("11", 5, 3,true, TestName = "Unsigned number when only positive")]
		[TestCase("0", 5, 3,true, TestName = "Zero when only positive")]
		// Нет поддержки римских цифр.

		public void IsValidNumber_ValidNumber_True(string number, int precision, int scale = 0, bool onlyPositive = false)
		{
			Assert.IsTrue(new NumberValidator(precision, scale, onlyPositive).IsValidNumber(number));
		}
		[TestCase(".0", 4, 2, TestName = "Number starts with dot")]
		[TestCase(",0", 4, 2, TestName = "Number starts with comma")]
		[TestCase("++0", 4, 2, TestName = "Number starts with double sign")]
		[TestCase(null, 4, 2, TestName = "Is null")]
		[TestCase("", 4, 2, TestName = "Is empty string")]
		[TestCase ("Danil", 4, 2, TestName = "Is not digit/english letters")]
		[TestCase ("ć", 4, 2, TestName = "Is not digit/poland letters")]
		[TestCase ("0.", 4, 2, TestName = "Number ends with dot")]
		[TestCase ("0,", 4, 2, TestName = "Number ends with comma")]
		[TestCase ("0!0", 4, 2, TestName = "Another separator")]
		[TestCase ("1..001", 4, 2, TestName = "Contains 2 dots side by side")]
		[TestCase ("1,,001", 4, 2, TestName = "Contains 2 commas side by side")]
		[TestCase ("@1.1", 4, 2, TestName = "Number starts with not digit")]
		[TestCase ("1.@1", 4, 2, TestName = "Number contains not digit, dot or comma")]
		[TestCase ("111", 2, 1, TestName = "Precision of number greater than specified precision")]
		[TestCase ("11.11", 5, 1, TestName = "Scale of number greater than specified scale")]
		[TestCase ("-5", 5, 1, true, TestName = "Negative number when only positive")]

		public void IsValidNumber_InvalidNumber_False(string number, int precision, int scale = 0, bool onlyPositive = false)
		{
			new NumberValidator(precision, scale, onlyPositive).IsValidNumber(number).Should().BeFalse(null, number);
		}

		[TestCase(-1, 5, TestName = "Precision is negative number")]
		[TestCase(0, 5, TestName = "Precision is zero")]
		[TestCase(5, -1, TestName = "Scale is negative number")]
		[TestCase(5, 5, TestName = "Scale equal to precision")]
		[TestCase(5, 6, TestName = "Scale greater than precision")]
		[TestCase(0, 0, TestName = "Scale and precision is zero")]
		public void CreateNumberValidator_IncorrectArguments_Throw(int precision, int scale = 0)
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(precision, scale));
		}

		[TestCase(5, 4,false, TestName = "Precision and scale is positive numbers, onlyPositive = false/precision>scale")]
		[TestCase(5, 4,true, TestName = "Precision and scale is positive numbers, onlyPositive = true/precision>scale")]
		[TestCase(5, 0,false, TestName = "Precision is positive and scale is zero, onlyPositive = false/precision>scale")]
		[TestCase(5, 0,true, TestName = "Precision is positive and scale is zero, onlyPositive = true/precision>scale")]
		public void CreateNumberValidator_CorrectArguments_DoesNotThrow(int precision, int scale, bool onlyPositive)
		{
			Assert.DoesNotThrow(() => new NumberValidator(precision, scale, onlyPositive));
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