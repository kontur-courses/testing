using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
        #region PreviousTests
        //[Test]
        //public void Test()
        //{
        //    Assert.Throws<ArgumentException>(() => new NumberValidator(-1, 2, true));
        //    Assert.DoesNotThrow(() => new NumberValidator(1, 0, true));
        //    Assert.Throws<ArgumentException>(() => new NumberValidator(-1, 2, false));
        //    Assert.DoesNotThrow(() => new NumberValidator(1, 0, true));

        //    Assert.IsTrue(new NumberValidator(17, 2, true).IsValidNumber("0.0"));
        //    Assert.IsTrue(new NumberValidator(17, 2, true).IsValidNumber("0"));
        //    Assert.IsTrue(new NumberValidator(17, 2, true).IsValidNumber("0.0"));
        //    Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("00.00"));
        //    Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("-0.00"));
        //    Assert.IsTrue(new NumberValidator(17, 2, true).IsValidNumber("0.0"));
        //    Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("+0.00"));
        //    Assert.IsTrue(new NumberValidator(4, 2, true).IsValidNumber("+1.23"));
        //    Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("+1.23"));
        //    Assert.IsFalse(new NumberValidator(17, 2, true).IsValidNumber("0.000"));
        //    Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("-1.23"));
        //    Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("a.sd"));
        //}
#endregion

        [Test]
		public void ThrowsException_WhenPrecisionIsNegative()
		{
			Action validator = () => new NumberValidator(-1, 2);
			validator.ShouldThrow<ArgumentException>();
		}

		[Test]
		public void ThrowsException_WhenScaleBiggerThenPrecision()
		{
			Action validator = () => new NumberValidator(1, 2);
			validator.ShouldThrow<ArgumentException>();
		}

		[Test]
		public void ThrowsException_WhenScaleIsNegative()
		{
			Action validator = () => new NumberValidator(1, -2);
			validator.ShouldThrow<ArgumentException>();
		}

        [Test]
		public void DoesNotThrowsException_WhenValidData()
		{
			Action validator = () => new NumberValidator(1);
			validator.ShouldNotThrow<ArgumentException>();
        }

		[Test]
		public void IsValid_WhenIntAndFrancPartLessThanPrecisionAndFrancLessThanScale()
		{
			new NumberValidator(17, 2, true).IsValidNumber("0.0").Should().BeTrue();
		}

		[Test]
		public void IsValid_WhenIntAndFrancPartLessThanPrecisionAndFrancLessThanScaleWithSign()
		{
			new NumberValidator(4, 2, true).IsValidNumber("+1.23").Should().BeTrue();
		}

        [Test]
		public void IsNotValid_WhenIntAndFrancPartBiggerThanPrecision()
		{
			new NumberValidator(3, 2, true).IsValidNumber("00.00").Should().BeFalse();
		}

		[Test]
		public void IsNotValid_WhenIntAndFrancPartWithSignBiggerThanPrecision()
		{
			new NumberValidator(3, 2, true).IsValidNumber("-0.00").Should().BeFalse();
			new NumberValidator(3, 2, true).IsValidNumber("+0.00").Should().BeFalse();
        }

		[Test]
		public void IsNotValid_WhenFrancBiggerThenScale()
		{
			new NumberValidator(17, 2, true).IsValidNumber("0.000").Should().BeFalse();
		}

		[Test]
		public void IsNotValid_WhenNotANumber()
		{
			new NumberValidator(3, 2, true).IsValidNumber("a.sd").Should().BeFalse();
        }

		[Test]
		public void IsNotValid_WhenNull()
		{
			new NumberValidator(3, 2, true).IsValidNumber(null).Should().BeFalse();
        }

		[Test]
		public void IsNotValid_WhenAbrakadabra()
		{
			new NumberValidator(3, 2, true).IsValidNumber("abrakadabra").Should().BeFalse();
        }

		[Test]
		public void IsNotValid_WhenNegativeIfOnlyPositive()
		{
			new NumberValidator(17, 2, true).IsValidNumber("-5.7").Should().BeFalse();
        }

		[Test]
		public void IsValid_WithComma()
		{
			new NumberValidator(17, 2, true).IsValidNumber("0,0").Should().BeTrue();
        }

		[Test]
		public void IsNotValid_WhenEmpty()
		{
			new NumberValidator(3, 2, true).IsValidNumber("").Should().BeFalse();
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