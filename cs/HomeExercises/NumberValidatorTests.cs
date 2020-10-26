using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[TestCase("0.0")]
		[TestCase("00.00")]
		[TestCase("12.3")]
		[TestCase("9.99")]
		[TestCase("123.456")]
		public void IsValidNumber_BeTrue_ThanNormalValueAndDot(string number)
		{
			var numberValidator = new NumberValidator(17, 12, true);
			
			numberValidator.IsValidNumber(number).Should().BeTrue();
		}
		
		[TestCase("0,0")]
		[TestCase("00,00")]
		[TestCase("12,3")]
		[TestCase("9,99")]
		[TestCase("123,456")]
		public void IsValidNumber_BeTrue_ThanNormalValueAndComma(string number)
		{
			var numberValidator = new NumberValidator(17, 12, true);
			
			numberValidator.IsValidNumber(number).Should().BeTrue();
		}
		
		[TestCase("+0.0")]
		[TestCase("+1.2")]
		public void IsValidNumber_BeTrue_ThanSignPositive(string number)
		{
			var numberValidator = new NumberValidator(3, 2, true);
			
			numberValidator.IsValidNumber(number).Should().BeTrue();
		}
		
		[TestCase("-0.0")]
		[TestCase("-1.2")]
		public void IsValidNumber_BeTrue_ThanSignNegative(string number)
		{
			var numberValidator = new NumberValidator(3, 2);
			
			numberValidator.IsValidNumber(number).Should().BeTrue();
		}

		[TestCase("-0.0")]
		[TestCase("-1.2")]
		public void IsValidNumber_BeFalse_IfOnlyPositiveTrueAndNegativeSign(string number)
		{
			var numberValidator = new NumberValidator(3, 2, true);
			
			numberValidator.IsValidNumber(number).Should().BeFalse();
		}
		
		[TestCase("0")]
		public void IsValidNumber_BeTrue_WithoutFraction(string number)
		{
			var numberValidator = new NumberValidator(3, 2, true);
			
			numberValidator.IsValidNumber(number).Should().BeTrue();
		}
		
		[TestCase("")]
		public void IsValidNumber_BeFalse_ThanEmptyValue(string number)
		{
			var numberValidator = new NumberValidator(3, 2, true);
			
			numberValidator.IsValidNumber(number).Should().BeFalse();
		}

		[TestCase("0.000")]
		[TestCase("0.000000000000000")]
		public void IsValidNumber_BeFalse_IfFractionLengthMoreThanScale(string number)
		{
			var numberValidator = new NumberValidator(3, 2, true);
			
			numberValidator.IsValidNumber(number).Should().BeFalse();
		}
		
		[TestCase("0.00")]
		[TestCase("1,23")]
		public void IsValidNumber_BeTrue_IfFractionLengthEqualsThanScale(string number)
		{
			var numberValidator = new NumberValidator(3, 2, true);
			
			numberValidator.IsValidNumber(number).Should().BeTrue();
		}

		[TestCase("a.sd")]
		[TestCase("1.sd")]
		[TestCase("a.23")]
		[TestCase("1%23")]
		[TestCase("1:23")]
		public void IsValidNumber_BeFalse_ThanInvalidValue(string number)
		{
			var numberValidator = new NumberValidator(3, 2, true);
			
			numberValidator.IsValidNumber(number).Should().BeFalse();
		}
		
		[TestCase(".23")]
		public void IsValidNumber_BeFalse_WithoutIntPart(string number)
		{
			var numberValidator = new NumberValidator(4, 3, true);
			
			numberValidator.IsValidNumber(number).Should().BeFalse();
		}

		[TestCase("12345.000")]
		[TestCase("99999999999999999.000")]
		public void IsValidNumber_BeFalse_IfIntAndFractionLengthsMoreThanPrecision(string number)
		{
			var numberValidator = new NumberValidator(6, 3, true);
			
			numberValidator.IsValidNumber(number).Should().BeFalse();
		}
		
		[TestCase("2147483648.9")]
		[TestCase("99999999999999999999999999999999999999999999999999999999999")]
		public void IsValidNumber_BeTrue_IfValueMoreThanInteger(string number)
		{
			var numberValidator = new NumberValidator(100, 3, true);
			
			numberValidator.IsValidNumber(number).Should().BeTrue();
		}

		[Test]
		public void IsValidNumber_ThrowsArgumentException_IfPrecisionNegative()
		{
			Action act = () => new NumberValidator(-1, -2, true);

			act.Should().Throw<ArgumentException>()
				.WithMessage("precision must be a positive number");
		}
		
		[Test]
		public void IsValidNumber_ThrowsArgumentException_IfScaleNegative()
		{
			Action act = () => new NumberValidator(2, -1, true);

			act.Should().Throw<ArgumentException>()
				.WithMessage("precision must be a non-negative number less or equal than precision");
		}
		
		[Test]
		public void IsValidNumber_ThrowsArgumentException_IfScaleMoreThanPrecision()
		{
			Action act = () => new NumberValidator(1, 2, true);

			act.Should().Throw<ArgumentException>()
				.WithMessage("precision must be a non-negative number less or equal than precision");
		}
		
		
		
		/*
		[Test]
		public void Test()
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(-1, 2, true));
			Assert.DoesNotThrow(() => new NumberValidator(1, 0, true));
			Assert.Throws<ArgumentException>(() => new NumberValidator(-1, 2, false));
			Assert.DoesNotThrow(() => new NumberValidator(1, 0, true));

			Assert.IsTrue(new NumberValidator(17, 2, true).IsValidNumber("0.0"));
			Assert.IsTrue(new NumberValidator(17, 2, true).IsValidNumber("0"));
			Assert.IsTrue(new NumberValidator(17, 2, true).IsValidNumber("0.0"));
			Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("00.00"));
			Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("-0.00"));
			Assert.IsTrue(new NumberValidator(17, 2, true).IsValidNumber("0.0"));
			Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("+0.00"));
			Assert.IsTrue(new NumberValidator(4, 2, true).IsValidNumber("+1.23"));
			Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("+1.23"));
			Assert.IsFalse(new NumberValidator(17, 2, true).IsValidNumber("0.000"));
			Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("-1.23"));
			Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("a.sd"));
		}
		*/
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