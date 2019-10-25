using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		private NumberValidator positiveNumberValidator;
		private NumberValidator numberValidator;
		
		[SetUp]
		public void SetUp()
		{
			positiveNumberValidator = new NumberValidator(6, 2, true);
			numberValidator = new NumberValidator(6, 2);
		}
		
		[Test]
		public void NumberValidator_EmptyString_False()
		{
			positiveNumberValidator.IsValidNumber("").Should().BeFalse();
			numberValidator.IsValidNumber("").Should().BeFalse();
		}
		
		[Test]
		public void NumberValidator_NullString_False()
		{
			positiveNumberValidator.IsValidNumber(null).Should().BeFalse();
			numberValidator.IsValidNumber(null).Should().BeFalse();
		}
		
		[TestCase("abs")]
		[TestCase("a.sd")]
		public void NumberValidator_NotNumberString_False(string input)
		{
			numberValidator.IsValidNumber(input).Should().BeFalse();
			positiveNumberValidator.IsValidNumber(input).Should().BeFalse();
		}
		
		[Test]
		public void PositiveNumberValidator_NegativeNumber_False()
		{
			var number1 = "-1.1";
			var number2 = "-1123";

			positiveNumberValidator.IsValidNumber(number1).Should().BeFalse();
			positiveNumberValidator.IsValidNumber(number2).Should().BeFalse();
		}
		
		[TestCase("1.123")]
		[TestCase("-1.123")]
		public void NumberValidator_NumberScaleLongerThanValidatorScale_False(string number)
		{
			numberValidator.IsValidNumber(number).Should().BeFalse();
		}
		
		[TestCase("-12345.6")]
		[TestCase("+12345.6")]
		public void NumberValidator_NumberWithSignLongerThanPrecision_False(string number)
		{
			numberValidator.IsValidNumber(number).Should().BeFalse();
		}
		
		[TestCase("123456.7")]
		[TestCase("00000.00")]
		public void NumberValidator_NumberLongerThanPrecision_False(string number)
		{
			numberValidator.IsValidNumber(number).Should().BeFalse();
		}
		
		[TestCase("-3.2")]
		[TestCase("22.1")]
		[TestCase("-0.0")]
		[TestCase("-0")]
		public void NumberValidator_CorrectNegativeNumber_True(string number)
		{
			numberValidator.IsValidNumber(number).Should().BeTrue();
		}
		
		[TestCase("+3.2")]
		[TestCase("22.1")]
		[TestCase("+0.0")]
		[TestCase("0")]
		public void PositiveNumberValidator_CorrectPositiveNumber_True(string number)
		{
			positiveNumberValidator.IsValidNumber(number).Should().BeTrue();
		}
		
		[TestCase(-1)]
		[TestCase(0)]
		[TestCase(-22222)]
		public void ArgumentException_NumberValidatorBuilder_PrecisionLessThan1(int precision)
		{
			Action action = () => new NumberValidator(precision, 1, true);
			
			action.ShouldThrow<ArgumentException>();
		}
		
		[TestCase(-1)]
		[TestCase(-128)]
		public void ArgumentException_NumberValidatorBuilder_NegativeScale(int scale)
		{
			Action action = () => new NumberValidator(1, scale, true);

			action.ShouldThrow<ArgumentException>();
		}
		
		[Test]
		public void ArgumentException_NumberValidatorBuilder_PrecisionLessThanScale()
		{
			Action action = () => new NumberValidator(1, 2, true);

			action.ShouldThrow<ArgumentException>();
		}
		
		[Test]
		public void ArgumentException_NumberValidatorBuilder_PrecisionEqualToScale()
		{
			Action action = () => new NumberValidator(1, 1, true);

			action.ShouldThrow<ArgumentException>();
		}
		
		[Test]
		[Category("Не уверен что это нужно")]
		public void CorrectNumberValidatorBuilder_WithoutScale()
		{
			Action action = () => new NumberValidator(1,  onlyPositive:true);
			Action action1 = () => new NumberValidator(1, onlyPositive: false);
			
			action.ShouldNotThrow<ArgumentException>();
			action1.ShouldNotThrow<ArgumentException>();
		}
		
		[Test]
		[Category("Не уверен что это нужно")]
		public void CorrectNumberValidatorBuilder_WithoutSign()
		{
			Action action = () => new NumberValidator(2,  1);
			Action action1 = () => new NumberValidator(2, scale: 1);
			
			action.ShouldNotThrow<ArgumentException>();
			action1.ShouldNotThrow<ArgumentException>();
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