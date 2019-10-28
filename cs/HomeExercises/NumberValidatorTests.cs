using System;
using System.Collections;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;
using NUnit.Framework.Internal;


namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[Test, TestCaseSource(nameof(IsValidNumber_CheckErrors_TestCases))]
		public void IsValidNumber_IncorrectInput_ThrowsException(int precision, int scale, bool isOnlyPositive)
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(precision, scale, isOnlyPositive));
        }

		public static IEnumerable IsValidNumber_CheckErrors_TestCases
		{
			get
			{
				yield return new TestCaseData(-1, 2, true);
				yield return new TestCaseData(1, -1, true);
                yield return new TestCaseData(1, 2, true);
				yield return new TestCaseData(-1, 2, false);
				yield return new TestCaseData(1, 2, false);
            }
		}


        [Test, TestCaseSource(nameof(IsValidNumber_CheckFunctionality_TestCases))]
        public bool IsValidNumber(int precision, int scale, bool isOnlyPositive, string inp)
		{
			if (scale != 0)
				return new NumberValidator(precision, scale, isOnlyPositive).IsValidNumber(inp);
			else
				return new NumberValidator(precision, onlyPositive:isOnlyPositive).IsValidNumber(inp);
        }

        public static IEnumerable IsValidNumber_CheckFunctionality_TestCases
        {
	        get
	        {
		        yield return new TestCaseData(17, 2, true, "0.0").Returns(true);
                yield return new TestCaseData(2, 1, true, "0.0").Returns(true);
				yield return new TestCaseData(2, 1, true, "00.0").Returns(false);
				yield return new TestCaseData(4, 1, true, "0.00").Returns(false);
				yield return new TestCaseData(4, 1, true, "000.00").Returns(false);
				yield return new TestCaseData(17, 4, true, "-0.0").Returns(false);
				yield return new TestCaseData(17, 4, false, "-0.0").Returns(true);
				yield return new TestCaseData(17, 4, false, "+0.0").Returns(true);
                yield return new TestCaseData(4, 2, false, "-00.00").Returns(false);
				yield return new TestCaseData(17, 5, true, "123.ac").Returns(false);
				yield return new TestCaseData(17, 5, true, "ac").Returns(false);
                yield return new TestCaseData(17, 5, true, "").Returns(false);
                yield return new TestCaseData(17, 5, true, null).Returns(false);
                yield return new TestCaseData(5, 0, true, "12345").Returns(true);
				yield return new TestCaseData(5, 0, false, "-12345").Returns(false);
				yield return new TestCaseData(5, 2, true, "+123.12").Returns(true); //Почему на этот тест выдает false?
																				  //Знак же учитывается только для отрицательных чисел
				yield return new TestCaseData(6, 3, false, "1.1.1").Returns(false);
	        }
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