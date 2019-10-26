using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	[TestFixture]
	public class NumberValidatorTests
	{
		[Test]
		public void NumberValidator_Precision_IsPositiveNumber()
		{
			Action act = () => new NumberValidator(0);
			act.ShouldThrow<ArgumentException>();
		}

		[Test]
		public void NumberValidator_ScaleBiggerThanPrecision_ThrowsArgumentException()
		{
			Action act = () => new NumberValidator(2, 3);
			act.ShouldThrow<ArgumentException>();
		}

		[Test]
		public void IsValidNumber_NonNumericString_ReturnsFalse()
		{
			var nv = new NumberValidator(3);
			nv.IsValidNumber("abc").Should().BeFalse();
		}

		[Test]
		public void IsValidNumber_NumberPrecision_ShouldBeLessOrEqualToValidatorPrecision()
		{
			var nv = new NumberValidator(3);
			nv.IsValidNumber("12").Should().BeTrue();
			nv.IsValidNumber("123").Should().BeTrue();
			nv.IsValidNumber("1234").Should().BeFalse();
		}

		[Test]
		public void IsValidNumber_NumberSigns_CountTowardsPrecision()
		{
			var nv = new NumberValidator(3);
			nv.IsValidNumber("123").Should().BeTrue();
			nv.IsValidNumber("+1234").Should().BeFalse();
			nv.IsValidNumber("-1234").Should().BeFalse();
		}

		[Test]
		public void IsValidNumber_DotAndCommaSeparators_AreAccepted()
		{
			var nv = new NumberValidator(4, 3);
			nv.IsValidNumber("1.234").Should().BeTrue();
			nv.IsValidNumber("1,234").Should().BeTrue();
		}

		[Test]
		public void IsValidNumber_NoNumberAfterSeparator_ReturnsFalse()
		{
			var nv = new NumberValidator(4, 3);
			nv.IsValidNumber("123.").Should().BeFalse();
		}

		[Test]
		public void IsValidNumber_NumberScale_ShouldBeLessOrEqualToValidatorScale()
		{
			var nv = new NumberValidator(4, 3);
			nv.IsValidNumber("0.12").Should().BeTrue();
			nv.IsValidNumber("0.123").Should().BeTrue();
			nv.IsValidNumber("0.1234").Should().BeFalse();
		}

		[Test]
		public void IsValidNumber_PassingNegativeNumberToPositiveNumberValidator_ReturnsFalse()
		{
			var nv = new NumberValidator(3, onlyPositive: true);
			nv.IsValidNumber("-12").Should().BeFalse();
		}

		[Test]
		public void IsValidNumber_OnlyOneSign_ReturnsFalse()
		{
			var nv = new NumberValidator(3);
			nv.IsValidNumber("+").Should().BeFalse();
		}

		[Test]
		public void IsValidNumber_MoreThanOneSignInNumber_ReturnsFalse()
		{
			var nv = new NumberValidator(4);
			nv.IsValidNumber("--00").Should().BeFalse();
			nv.IsValidNumber("++00").Should().BeFalse();
		}

		[Test]
		public void IsValidNumber_MoreThanOneSeparators_ReturnsFalse()
		{
			var nv = new NumberValidator(10, 5);
			nv.IsValidNumber("12..3").Should().BeFalse();
			nv.IsValidNumber("12.,3").Should().BeFalse();
			nv.IsValidNumber("12.3.4").Should().BeFalse();
		}

		[TestCase(10)]
		[TestCase(100)]
		[TestCase(1000)]
		public void IsValidNumber_Perfomance_IsTimePermissible(int numberPrecision)
		{
			var nv = new NumberValidator(numberPrecision);
			var s = new string('0', numberPrecision);
			Action action = () => nv.IsValidNumber(s);
			action.ExecutionTime().ShouldNotExceed(100.Milliseconds());
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
				throw new ArgumentException("scale must be a non-negative number less or equal than precision");
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