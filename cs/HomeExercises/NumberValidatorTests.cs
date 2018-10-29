using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace HomeExercises
{
	[SuppressMessage("ReSharper", "ObjectCreationAsStatement")]
	public class NumberValidatorTests
	{
		[TestCase(6, 5, false)]
		[TestCase(5, 1, false)]
		[TestCase(5, 0, true)]
		public void Constructor_DoesNot_Throw_WithValidArguments(int precision, int scale, bool onlyPositive)
		{
			Assert.DoesNotThrow(() => new NumberValidator(precision, scale, onlyPositive),
				"Expected constructor not to throw with validator settings:" +
				$"\n\tPrecision: {precision};" +
				$"\n\tScale: {scale};" +
				$"\n\t{(onlyPositive ? "Only positive" : "All numbers")}.");
		}

		[TestCase(-1, 2, false, TestName = "WhenPrecision_IsNegative")]
		[TestCase(0, -2, false, TestName = "WhenPrecision_IsZero")]
		[TestCase(1, -2, false, TestName = "WhenScale_IsNegative")]
		[TestCase(5, 6, false, TestName = "WhenScale_IsGreaterThanPrecision")]
		[TestCase(5, 5, false, TestName = "WhenScale_IsEqualToPrecision")]
		public void Constructor_Throws_ArgumentException(int precision, int scale, bool onlyPositive)
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(precision, scale, onlyPositive),
				"Expected constructor to throw an ArgumentException with validator settings:" +
				$"\n\tPrecision: {precision};" +
				$"\n\tScale: {scale};" +
				$"\n\t{(onlyPositive ? "Only positive" : "All numbers")}.");
		}

		[Test]
		public void Validates_Numbers_WithLessSymbols()
		{
			AssertValid("0.0", 17, 2, false);
		}

		[Test]
		public void Validates_Integer_Numbers()
		{
			AssertValid("0", 4, 2, false);
		}

		[TestCase("000.00")]
		[TestCase("0.000")]
		[TestCase("+00.00")]
		[TestCase("-00.00")]
		public void DoesNot_Validate_Numbers_WithTooManySymbols(string number)
		{
			AssertInvalid(number, 4, 2, false);
		}

		[TestCase("-0.0")]
		[TestCase("-1.0")]
		[TestCase("-1.23")]
		public void PosOnly_DoesNot_Validate_Negative_Numbers(string number)
		{
			AssertInvalid(number, 4, 2, true);
		}

		[TestCase("0.0")]
		[TestCase("+0.0")]
		[TestCase("1.0")]
		[TestCase("+1.23")]
		[TestCase("11.23")]
		public void PosOnly_Validates_NonNegative_Numbers(string number)
		{
			AssertValid(number, 4, 2, true);
		}

		[Test]
		public void DoesNot_Validate_NonNumber()
		{
			AssertInvalid("a.sd", 4, 2, false);
		}

		[Test]
		public void DoesNot_Validate_EmptyString()
		{
			AssertInvalid("", 4, 2, false);
		}

        private static void AssertInvalid(string number, int precision, int scale, bool onlyPositive)
		{
			Assert.IsFalse(new NumberValidator(precision, scale, onlyPositive).IsValidNumber(number),
				$"Expected \"{number}\" to be invalid (actual: valid) with validator settings:" +
				$"\n\tPrecision: {precision};" +
				$"\n\tScale: {scale};" +
				$"\n\t{(onlyPositive ? "Only positive" : "All numbers")}.");
		}

		private static void AssertValid(string number, int precision, int scale, bool onlyPositive)
		{
			Assert.IsTrue(new NumberValidator(precision, scale, onlyPositive).IsValidNumber(number),
				$"Expected \"{number}\" to be valid (actual: invalid) with validator settings:" +
				$"\n\tPrecision: {precision};" +
				$"\n\tScale: {scale};" +
				$"\n\t{(onlyPositive ? "Only positive" : "All numbers")}.");
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
				throw new ArgumentException("Precision must be a positive number");
			if (scale < 0 || scale >= precision)
				//Вы тут намеренно ошиблись?
				throw new ArgumentException("Scale must be a non-negative number and less than precision");
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
