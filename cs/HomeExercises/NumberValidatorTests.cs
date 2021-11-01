using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[Test]
		public void OriginalTests()
        {
			Assert.Throws<ArgumentException>(() => new NumberValidator(-1, 2, true));
			Assert.DoesNotThrow(() => new NumberValidator(1, 0, true));
			Assert.Throws<ArgumentException>(() => new NumberValidator(-1, 2, false));
			//Assert.DoesNotThrow(() => new NumberValidator(1, 0, true)); - repeats 13 line

			Assert.IsTrue(new NumberValidator(17, 2, true).IsValidNumber("0.0"));
			Assert.IsTrue(new NumberValidator(17, 2, true).IsValidNumber("0"));
			//Assert.IsTrue(new NumberValidator(17, 2, true).IsValidNumber("0.0")); - repeats 18 line
			Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("00.00"));
			Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("-0.00"));
			//Assert.IsTrue(new NumberValidator(17, 2, true).IsValidNumber("0.0")); -repeats 18 line
			//Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("+0.00")); - almost repeats 22 line
			Assert.IsTrue(new NumberValidator(4, 2, true).IsValidNumber("+1.23"));
			//Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("+1.23")); - quiet similiar to 22 line
			Assert.IsFalse(new NumberValidator(17, 2, true).IsValidNumber("0.000"));
			//Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("-1.23")); -almost repeats 25 line
			Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("a.sd"));

			/*
			 *Отсутствует тест конструктора, проверяющий поведение при отрицательном scale,
			 *Скудность проверки выражений, отличающихся от заданного формата,
			 *отсутствие проверки на null и пустую строку
			 *Слишком мало тестовых примеров, возникает ситуация, что часто один случай проверяет сразу несколько важных особеннностей,
			 *может быть непонятно, на какой именнно мы допустили ошибку.
			*/


		}
		[Test]
		public void ShouldThrowExceptionWhenPrecisionIsNotPositive()
		{
			Action act1 = () => new NumberValidator(-1, 2, false);
			Action act2 = () => new NumberValidator(0, -1, true);

			act1.Should().ThrowExactly<ArgumentException>().WithMessage("precision must be a positive number");
			act2.Should().ThrowExactly<ArgumentException>().WithMessage("precision must be a positive number");
		}


		[Test]
		public void ShouldThrowExceptionWhenScaleIsNegative()
		{
			Action validator = () => new NumberValidator(1, -1, false);
			validator.Should().ThrowExactly<ArgumentException>().WithMessage("scale must be a non-negative number less than precision");
		}


		[Test]
		public void ShouldThrowExceptionWhenScaleIsGreaterThenPrecision()
		{
			Action validator = () => new NumberValidator(1, 2, false);
			validator.Should().ThrowExactly<ArgumentException>().WithMessage("scale must be a non-negative number less than precision");
		}


		[Test]
		public void ShouldNotThrowExceptionForValidArgs()
		{
			Action act1 = () => new NumberValidator(1, 0, true);
			Action act2 = () => new NumberValidator(2, 1, true);
			act1.Should().NotThrow();
			act2.Should().NotThrow();
		}


		[TestCase("1.23", 4, 2, true, true)]
		[TestCase("+1.23", 4, 2, true, true)]
		[TestCase("-1.23", 4, 2, false, true)]
		[TestCase("1.00", 3, 2, true, true)]
		[TestCase("+1.23", 3, 2, true, false)]
		[TestCase("-1.23", 3, 2, false, false)]
		[TestCase("00.00", 3, 2, true, false)]
		public void IsValidForPrecision
			(string valueForCheck, int precision, int scale, bool onlyPositive, bool expectedResultOfChecking)
			=> IsValidFor(valueForCheck, precision, scale, onlyPositive, expectedResultOfChecking,
				" and\n the maximum number of characters in a valid number, including a sign, must not exceed the precision");


		[TestCase("", 17, 4, true, false)]
		[TestCase(null, 17, 4, true, false)]
		public void IsValidNullOrEmpty
			(string valueForCheck, int precision, int scale, bool onlyPositive, bool expectedResultOfChecking)
			=> IsValidFor(valueForCheck, precision, scale, onlyPositive, expectedResultOfChecking,
				" and\n empty string or null is not a valid number");


		[TestCase("0.000", 17, 2, true, false)]
		[TestCase("+1.23", 17, 2, true, true)]
		[TestCase("0.1", 17, 2, true, true)]
		public void IsValidForScale
			(string valueForCheck, int precision, int scale, bool onlyPositive, bool expectedResultOfChecking)
			=> IsValidFor(valueForCheck, precision, scale, onlyPositive, expectedResultOfChecking,
				" and\n the maximum number of decimal digits of a valid number must not exceed the size of the \"scale\" parameter");


		[TestCase("-1.23", 17, 2, true, false)]
		[TestCase("-1.23", 17, 2, false, true)]
		public void IsValidWithMinus
			(string valueForCheck, int precision, int scale, bool onlyPositive, bool expectedResultOfChecking)
			=> IsValidFor(valueForCheck, precision, scale, onlyPositive, expectedResultOfChecking,
				" and\n if there is a minus in the number, we must specify it in the constructor");


		[TestCase("+", 17, 4, true, false)]
		[TestCase("-0.", 17, 4, false, false)]
		[TestCase("+0.", 17, 4, true, false)]
		[TestCase(".1", 17, 4, true, false)]
		[TestCase("-.0", 17, 4, true, false)]
		[TestCase("+1;7", 17, 4, true, false)]
		[TestCase("a.sd", 6, 4, true, false)]
		[TestCase("+1,7", 17, 4, true, true)]
		public void IsValidForFormat
			(string valueForCheck, int precision, int scale, bool onlyPositive, bool expectedResultOfChecking)
			=> IsValidFor(valueForCheck, precision, scale, onlyPositive, expectedResultOfChecking,
				" and\n because the format of a numeric value " +
				"assumes two sets of digits separated by a dot or comma,\n " +
				"and each set must have at least one digit, the first set\n " +
				"can also be preceded by a plus or minus sign");


		[TestCase("+0.00", 3, 2, true, false)]
		[TestCase("-0.00", 3, 2, true, false)]
		[TestCase("0.0", 17, 2, true, true)]
		[TestCase("0", 17, 2, true, true)]
		public void IsValidForAdditionalTests
			(string valueForCheck, int precision, int scale, bool onlyPositive, bool expectedResultOfChecking)
			=> IsValidFor(valueForCheck, precision, scale, onlyPositive, expectedResultOfChecking,
				"");


		public void IsValidFor(string valueForCheck, int precision, int scale, bool onlyPositive, bool expectedResultOfChecking,string whatWrong)
		{
			var valid = " is valid number for NumberValidator";
			var notValid = " is not valid number for NumberValidator";
			var isValidNumber = new NumberValidator(precision, scale, onlyPositive).IsValidNumber(valueForCheck);
			if (expectedResultOfChecking)
				isValidNumber.Should().BeTrue($"{valueForCheck}{valid}({precision}, {scale}, {onlyPositive})" +
					$"{whatWrong}");
			else
				isValidNumber.Should().BeFalse($"{valueForCheck}{notValid}({precision}, {scale}, {onlyPositive})" +
					$"{whatWrong}");
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
				throw new ArgumentException("scale must be a non-negative number less than precision");
				//throw new ArgumentException("precision must be a non-negative number less or equal than precision");
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