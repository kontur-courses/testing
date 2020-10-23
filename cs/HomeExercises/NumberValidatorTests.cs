using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		public class ConstuctorTest
		{
			[TestCase(-1, 3)]
			[TestCase(-1, 0, true)]
			[TestCase(-1, -7)]
			[TestCase(-6, 5)]
			[TestCase(0, 0, true)]
			[TestCase(0, 4)]
			[TestCase(0, -1, true)]
			public void Test_PrecisionMustBePositive(int precesion, int scale, bool onlyPositive = false)
			{
				Assert.Throws<ArgumentException>(() => new NumberValidator(precesion, scale, onlyPositive));
			}

			[TestCase(3, -1)]
			[TestCase(3, -7, true)]
			[TestCase(17, -16)]
			public void Test_ScaleMustBeNonNegative(int precesion, int scale, bool onlyPositive = false)
			{
				Assert.Throws<ArgumentException>(() => new NumberValidator(precesion, scale, onlyPositive));
			}

			[TestCase(3, 4)]
			[TestCase(1, 3)]
			[TestCase(17, 18)]
			[TestCase(9, 9)]
			public void Test_ScaleMustBeLessThanPrecision(int precesion, int scale)
			{
				Assert.Throws<ArgumentException>(() => new NumberValidator(precesion, scale));
			}

			[TestCase(1, 0, true)]
			[TestCase(3, 1, false)]
			[TestCase(15, 14, false)]
			[TestCase(15, 7, true)]
			[TestCase(4, 2, true)]
			public void Test_PrecisionIsPositiv_ScaleBetweenZeroAndPrecision_IsValid(int precision, int scale, bool isOnlyPositive)
			{
				Assert.DoesNotThrow(() => new NumberValidator(precision, scale, isOnlyPositive));
			}
		}

		public class CorrectNumberTest
		{
			private readonly NumberValidator standartValidator = new NumberValidator(10, 9);

			[TestCase("a.bc")]
			[TestCase("12a.bc")]
			[TestCase("12.c")]
			[TestCase("43.b43c")]
			[TestCase("a")]
			[TestCase("a.104")]
			[TestCase("-2#")]
			public void Test_StringCanContainsOnlyNumbersSignAndSeparator(string str)
			{
				Assert.IsFalse(standartValidator.IsValidNumber(str));
			}

			[Test]
			public void Test_NullString_NotValid()
			{
				Assert.IsFalse(standartValidator.IsValidNumber(null));	
			}

			[Test]
			public void Test_EmptyString_NotValid()
			{
				Assert.IsFalse(standartValidator.IsValidNumber(""));	
			}
		
			[TestCase("1.0", true)]
			[TestCase("1,0", true)]
			[TestCase("-7.15", true)]
			[TestCase("-0,15", true)]
			[TestCase("1:0", false)]
			[TestCase("1'1", false)]
			[TestCase("8;15", false)]
			[TestCase("9-3", false)]
			public void Test_OnlyDotOrCommaMustBeSeparator(string str, bool expectedValid)
			{
				Assert.AreEqual(standartValidator.IsValidNumber(str), expectedValid);
			}

			[TestCase("1.0")]
			[TestCase("15,15")]
			[TestCase("6")]
			[TestCase("-8")]
			[TestCase("-3.2")]
			[TestCase("-7,16")]
			public void Test_OneOrNothingSeparator_IsValid(string str)
			{
				Assert.IsTrue(standartValidator.IsValidNumber(str));
			}

			[TestCase("1.0.0")]
			[TestCase("15,,15")]
			[TestCase("6,0.5")]
			[TestCase("-8.5,6")]
			[TestCase("-3.,2")]
			[TestCase("-7,.16")]
			[TestCase("1.1.1.1")]
			public void Test_MoreThanOneSeparator_IsNotValid(string str)
			{
				Assert.IsFalse(standartValidator.IsValidNumber(str));
			}

			[TestCase("15.")]
			[TestCase("15,")]
			[TestCase(".50")]
			[TestCase(",6")]
			[TestCase("+7.")]
			[TestCase("+.15")]
			[TestCase("-,0")]
			[TestCase("+.")]
			[TestCase("-,")]
			[TestCase(".")]
			[TestCase(",")]
			public void Test_SeparatorCanBeBetweenNumbers(string str)
			{
				Assert.IsFalse(standartValidator.IsValidNumber(str));
			}

			[TestCase("+")]
			[TestCase("-")]
			[TestCase("+.")]
			[TestCase("-,")]
			[TestCase(".")]
			[TestCase(",")]
			public void Test_StringWithoutNumber_IsNotValid(string str)
			{
				Assert.IsFalse(standartValidator.IsValidNumber(str));
			}

			[TestCase("+9.5")]
			[TestCase("-8.6")]
			[TestCase("+9,0")]
			[TestCase("-7,3")]
			[TestCase("+913")]
			[TestCase("-54")]
			public void Test_StringStartesWithSign_IsValid(string str)
			{
				Assert.IsTrue(standartValidator.IsValidNumber(str));
			}

			[TestCase("0.0")]
			[TestCase("1,1")]
			[TestCase("12")]
			public void Test_StringWithoutSign_IsValid(string str)
			{
				Assert.IsTrue(standartValidator.IsValidNumber(str));
			}

			[TestCase("++0")]
			[TestCase("+-1")]
			[TestCase("--0.0")]
			[TestCase("-+1,1")]
			[TestCase("++++12")]
			[TestCase("-+-+-+-6")]
			public void Test_StringWithMoreThanOneSign_IsNotValid(string str)
			{
				Assert.IsFalse(standartValidator.IsValidNumber(str));
			}

		}

		public class ValidatorTeste
		{
			[TestCase(13, 1, "1.0")]
			[TestCase(1, 0, "7")]
			[TestCase(2, 1, "3.9")]
			[TestCase(3, 2, "-3.0")]
			[TestCase(2, 0, "-9")]
			public void Test_LengthNumberLessOrEqualThanProcision_IsValid(int procising, int scale, string str)
			{
				Assert.IsTrue(new NumberValidator(procising, scale).IsValidNumber(str));
			}

			[TestCase(2, 1, "4.45")]
			[TestCase(2, 1, "+5.0")]
			[TestCase(1, 0, "+7")]
			public void Test_LengthNumberMoreThanProcision_IsNotValid(int procising, int scale, string str)
			{
				Assert.IsFalse(new NumberValidator(procising, scale).IsValidNumber(str));
			}

			[TestCase(1, "0.0")]
			[TestCase(2, "1.0")]
			[TestCase(1, "0")]
			[TestCase(1, "+5.6")]
			[TestCase(3, "-7")]
			[TestCase(3, "13.666")]
			public void Test_FractionalPartLessOrEqualThanScale_IsValid(int scale, string str)
			{
				Assert.IsTrue(new NumberValidator(10, scale).IsValidNumber(str));
			}

			[TestCase(1, "0.13")]
			[TestCase(2, "0.137")]
			[TestCase(0, "0.0")]
			public void Test_FractionalPartMoreThanScale_IsNotValid(int scale, string str)
			{
				Assert.IsFalse(new NumberValidator(10, scale).IsValidNumber(str));
			}

			[TestCase("0,15")]
			[TestCase("+0,15")]
			[TestCase("+0")]
			[TestCase("+0.0")]
			[TestCase("16")]
			public void Test_PositiveValidatorProcessesPositiveNumbers(string str)
			{
				Assert.IsTrue(new NumberValidator(10, 9, true).IsValidNumber(str));
			}

			[TestCase("-0,15")]
			[TestCase("-0")]
			[TestCase("-0.0")]
			[TestCase("-16")]
			public void Test_PositiveValidatorDoNotProcessesNotPositiveNumbers(string str)
			{
				Assert.IsFalse(new NumberValidator(10, 9, true).IsValidNumber(str));
			}

			[TestCase("0,15")]
			[TestCase("-0,15")]
			[TestCase("0")]
			[TestCase("+0.0")]
			[TestCase("-16")]
			public void Test_NotOnlyPositiveValidatorProcessNumbersWithAnySign(string str)
			{
				Assert.IsTrue(new NumberValidator(10, 9, false).IsValidNumber(str));
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