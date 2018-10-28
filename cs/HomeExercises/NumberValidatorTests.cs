using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[TestFixture]
		public class NumberValidatorConstructor_Should
		{
			[Test]
			public void ThrowExtention_WithNegativePrecision()
			{
				Assert.Throws<ArgumentException>(() => new NumberValidator(-1, 2));
            }
			[Test]
			public void ThrowExtention_WithZeroPrecision()
			{
				Assert.Throws<ArgumentException>(() => new NumberValidator(0, 2));
			}
            [Test]
			public void ThrowExtention_WithNegativeScale()
			{
				Assert.Throws<ArgumentException>(() => new NumberValidator(1, -2));
			}
			[Test]
			public void ThrowExtention_WithScaleGreaterThanPrecision()
			{
				Assert.Throws<ArgumentException>(() => new NumberValidator(1, 2));
			}

			[Test]
			public void DoesNotThrowExtention_WithCorrectArguments()
			{
				Assert.DoesNotThrow(() => new NumberValidator(1, 0, true));
            }
		}

		[TestFixture]
		public class NumberValidator_IsValidNumber_Should
		{
			[Test]
			public void BeFalse_OnNullString()
			{
				Assert.IsFalse(new NumberValidator(2, 1, true).IsValidNumber(null));
			}

			[Test]
			public void BeFalse_OnEmptyString()
			{
				Assert.IsFalse(new NumberValidator(2, 1, true).IsValidNumber(""));
            }

			[Test]
			public void BeFalse_OnIncorrectValue()
			{
				Assert.IsFalse(new NumberValidator(2, 1, true).IsValidNumber("testData"));
			}

			[Test]
			public void BeFalse_PrecisionLowerThanValue()
			{
				Assert.IsFalse(new NumberValidator(2, 0, true).IsValidNumber("111"));
            }

			[Test]
			public void BeFalse_ScaleLowerThanValue()
			{
				Assert.IsFalse(new NumberValidator(3, 1, true).IsValidNumber("1.11"));
			}
			[Test]
			public void BeFalse_PrecisionLowerThanValueWithSign()
			{
				Assert.IsFalse(new NumberValidator(3, 0, true).IsValidNumber("+111"));
			}
			[Test]
			public void BeFalse_WithNegativeValue_PositiveFlag()
			{
				Assert.IsFalse(new NumberValidator(2, 0, true).IsValidNumber("-1"));
			}
            [Test]
			public void BeTrue_WithNegativeValue_NotPoritiveFlag()
			{
				Assert.IsTrue(new NumberValidator(2, 0, false).IsValidNumber("-1"));
            }
			[Test]
			public void BeTrue_WithCorrectValue()
			{
				Assert.IsTrue(new NumberValidator(3, 2, false).IsValidNumber("1.1"));
			}

        }

		[Test]
		public void Test()
		{
			/*Assert.Throws<ArgumentException>(() => new NumberValidator(-1, 2, true));
			Assert.DoesNotThrow(() => new NumberValidator(1, 0, true));
			Assert.Throws<ArgumentException>(() => new NumberValidator(-1, 2, false));
			Assert.DoesNotThrow(() => new NumberValidator(1, 0, true));*/

			Assert.IsTrue(new NumberValidator(17, 2, true).IsValidNumber("0.0"));
			Assert.IsTrue(new NumberValidator(17, 2, true).IsValidNumber("0"));
			Assert.IsTrue(new NumberValidator(17, 2, true).IsValidNumber("0.0"));
			//Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("00.00"));
			//Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("-0.00"));
			Assert.IsTrue(new NumberValidator(17, 2, true).IsValidNumber("0.0"));
			//Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("+0.00"));
			Assert.IsTrue(new NumberValidator(4, 2, true).IsValidNumber("+1.23"));
			//Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("+1.23"));
			//Assert.IsFalse(new NumberValidator(17, 2, true).IsValidNumber("0.000"));
			//Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("-1.23"));
			//Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("a.sd"));
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