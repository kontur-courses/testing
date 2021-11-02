using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		/*
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
		*/
		/*
		 *Отсутствует тест конструктора, проверяющий поведение при отрицательном scale,
		 *Скудность проверки выражений, отличающихся от заданного формата,
		 *отсутствие проверки на null и пустую строку
		 *Слишком мало тестовых примеров, возникает ситуация, 
		 *что часто один случай проверяет сразу несколько важных особеннностей,
		 *может быть непонятно, на какой именнно мы допустили ошибку.
		 *Кроме этого, слишком много проверок в одном тесте
		*/


		[TestCase(-1, 2, false)]
		[TestCase(0, -1, true)]
		public void ShouldThrowException_WhenPrecisionIsNotPositive(int precision, int scale, bool onlyPositive)
		{
			Action validator = () => new NumberValidator(precision, scale, onlyPositive);
			validator.Should().ThrowExactly<ArgumentException>()
				.WithMessage("precision must be a positive number");
		}


		[TestCase(1, -1, false)]
		public void ShouldThrowException_WhenScaleIsNegative(int precision, int scale, bool onlyPositive)
		{
			Action validator = () => new NumberValidator(precision, scale, onlyPositive);
			validator.Should().ThrowExactly<ArgumentException>()
				.WithMessage("scale must be a non-negative number less than precision");
		}


		[TestCase(1, 2, false)]
		public void ShouldThrowException_WhenScaleIsGreaterThenPrecision(int precision, int scale, bool onlyPositive)
		{
			Action validator = () => new NumberValidator(precision, scale, onlyPositive);
			validator.Should().ThrowExactly<ArgumentException>()
				.WithMessage("scale must be a non-negative number less than precision");
		}


		[TestCase(1,0,true)]
		[TestCase(2,1,true)]
		public void ConstructorShouldWorkCorrectly(int precision, int scale, bool onlyPositive)
		{
			Action validator = () => new NumberValidator(precision, scale, onlyPositive);
			validator.Should().NotThrow();
		}


		[TestCase("1.23", 4, 2, true,
			TestName = "Number 1.23 with three digits and precision=4 should be valid a number")]
		[TestCase("+1.23", 4, 2, true,
			TestName = "Number +1.23 with three digits, sign and precision=4 should be valid a number")]
		[TestCase("1.00", 3, 2, true,
			TestName = "Number 1.00 with three digits and precision=3 should be valid a number")]
		public void ShouldBeValidForPrecision
			(string valueForCheck,
			int precision,
			int scale,
			bool onlyPositive)
		{
			var validator = new NumberValidator(precision, scale, onlyPositive);
			var result = validator.IsValidNumber(valueForCheck);
			result.Should().BeTrue();
		}


		[TestCase("-1.23", 3, 2, false,
			TestName = "Number -1.23 with sign, three digits and precision=3 " +
			"shouldn't be valid a number")]
		[TestCase("111.23", 3, 2, true,
			TestName = "Number 111.23 with five digits and precision=3 " +
			"shouldn't be valid a number")]
		[TestCase("00.00", 3, 2, true,
			TestName = "Number 00.00 with four digits and precision=3 " +
			"shouldn't be valid a number")]
		public void ShouldBeInvalidForPrecision
			(string valueForCheck,
			int precision,
			int scale,
			bool onlyPositive)
        {
			var validator = new NumberValidator(precision, scale, onlyPositive);
			var result = validator.IsValidNumber(valueForCheck);
			result.Should().BeFalse();
		}


		[TestCase("", 17, 4, true,
			TestName = "Empty string shouldn't be valid a number")]
		[TestCase(null, 17, 4, true,
			TestName ="Null shouldn't be valid a number")]
		public void ShouldBeInvalidForNullOrEmptyString
			(string valueForCheck,
			int precision,
			int scale,
			bool onlyPositive)
        {
			var validator = new NumberValidator(precision, scale, onlyPositive);
			var result = validator.IsValidNumber(valueForCheck);
			result.Should().BeFalse();
		}


		[TestCase("0.000", 17, 2, true,
			TestName = "Number 0.000 with a fractional part of 3 digits " +
			"and scale=2 shouldn't be a valid number")]
		public void ShouldBeInvalidForScale
			(string valueForCheck,
			int precision,
			int scale,
			bool onlyPositive)
		{
			var validator = new NumberValidator(precision, scale, onlyPositive);
			var result = validator.IsValidNumber(valueForCheck);
			result.Should().BeFalse();
		}
		

		[TestCase("+1.23", 17, 2, true,
			TestName = "Number +1.23 with a fractional part of " +
			"2 digits and scale=2 should be a valid number")]
		[TestCase("0.1", 17, 2, true,
			TestName = "Number 0.1 with a fractional part of " +
			"1 digits and scale=2 should be a valid number")]
		public void ShouldBeValidForScale
			(string valueForCheck,
			int precision,
			int scale,
			bool onlyPositive)
		{
			var validator = new NumberValidator(precision, scale, onlyPositive);
			var result = validator.IsValidNumber(valueForCheck);
			result.Should().BeTrue();
		}

		[TestCase("-1.23", 17, 2, true,
			TestName = "Negative number -1.23 without the specified minus " +
			"shouldn't be a valid number")]
		public void ShouldBeInvalidForNumber_WithMinus
			(string valueForCheck,
			int precision,
			int scale,
			bool onlyPositive)
		{
			var validator = new NumberValidator(precision, scale, onlyPositive);
			var result = validator.IsValidNumber(valueForCheck);
			result.Should().BeFalse();
		}


		[TestCase("-1.23", 17, 2, false,
			TestName = "Negative number -1.23 with the specified minus should be a valid number")]
		public void ShouldBeValidForNumber_WithMinus
			(string valueForCheck,
			int precision,
			int scale,
			bool onlyPositive)
		{
			var validator = new NumberValidator(precision, scale, onlyPositive);
			var result = validator.IsValidNumber(valueForCheck);
			result.Should().BeTrue();
		}


		[TestCase("+", 17, 4, true, TestName ="+ shouldn't be a valid number")]
		[TestCase("-0.", 17, 4, false, TestName ="-0. shouldn't be a valid number")]
		[TestCase(".1", 17, 4, true, TestName =".1 shouldn't be a valid number")]
		[TestCase("-.0", 17, 4, true, TestName = "-.0 shouldn't be a valid number")]
		[TestCase("+1;7", 17, 4, true, TestName = "+1;7 shouldn't be a valid number")]
		[TestCase("a.sd", 6, 4, true, TestName = "a.sd shouldn't be a valid number")]
		public void ShouldBeInvalidForFormat
			(string valueForCheck,
			int precision,
			int scale,
			bool onlyPositive)
		{
			var validator = new NumberValidator(precision, scale, onlyPositive);
			var result = validator.IsValidNumber(valueForCheck);
			result.Should().BeFalse();
		}


		[TestCase("+1,7", 17, 4, true, TestName = "+1,7 should be a valid number")]
		public void ShouldBeValidForFormat(
			string valueForCheck, 
			int precision, 
			int scale,
			bool onlyPositive)
		{
			var validator = new NumberValidator(precision, scale, onlyPositive);
			var result = validator.IsValidNumber(valueForCheck);
			result.Should().BeTrue();
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