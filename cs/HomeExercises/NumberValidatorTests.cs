using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
    [TestFixture]
	public class NumberValidatorTests
	{
        #region TestCases
		private static IEnumerable<TestCaseData> NonPositivePrecisionCases
		{
            get
            {
				yield return new TestCaseData(-1, 2, true);
				yield return new TestCaseData(-1, 2, false);
				yield return new TestCaseData(0, 1, true);
				yield return new TestCaseData(0, 1, false);
				yield return new TestCaseData(-2, 1, true);
				yield return new TestCaseData(-2, 1, false);
			}
        }

		private static IEnumerable<TestCaseData> PositivePrecisionCases
		{
			get
			{
				yield return new TestCaseData(10, 5, true);
				yield return new TestCaseData(10, 5, false);
				yield return new TestCaseData(2, 1, true);
				yield return new TestCaseData(2, 1, false);
			}
		}

		private static IEnumerable<TestCaseData> ScaleGreaterOrEqualToPrecisionCases
		{
			get
			{
				yield return new TestCaseData(5, 10, true);
				yield return new TestCaseData(5, 10, false);
				yield return new TestCaseData(1, 2, true);
				yield return new TestCaseData(1, 2, false);				
				yield return new TestCaseData(1, 1, true);
				yield return new TestCaseData(1, 1, false);
			}
		}

		private static IEnumerable<TestCaseData> NegativeScaleCases
		{
			get
			{
				yield return new TestCaseData(5, -10, true);
				yield return new TestCaseData(5, -10, false);
				yield return new TestCaseData(1, -2, true);
				yield return new TestCaseData(1, -2, false);
				yield return new TestCaseData(1, -1, true);
				yield return new TestCaseData(1, -1, false);
			}
		}

		private static IEnumerable<TestCaseData> NonDigitCharsCases
		{
			get
			{
				yield return new TestCaseData(10, 5, "a");
				yield return new TestCaseData(10, 5, "a.55");
				yield return new TestCaseData(10, 5, "a.");
				yield return new TestCaseData(10, 5, "5.bc");
				yield return new TestCaseData(10, 5, ".bc");
				yield return new TestCaseData(10, 5, "a.bc");
				yield return new TestCaseData(10, 5, "+a.bc");
				yield return new TestCaseData(10, 5, "-a.bc");
			}
		}

		private static IEnumerable<TestCaseData> OnlyIntegerPartCases
		{
            get
            {
				yield return new TestCaseData(10, 5, "1234");
				yield return new TestCaseData(10, 5, "01234");
				yield return new TestCaseData(10, 5, "+1234");
				yield return new TestCaseData(10, 5, "-1234");
            }
        }		
		
		private static IEnumerable<TestCaseData> IntegerPartAndFractionalPartCases
		{
            get
            {
				yield return new TestCaseData(10, 5, "0.0");
				yield return new TestCaseData(10, 5, "0.000");
				yield return new TestCaseData(10, 5, "1234.123");
				yield return new TestCaseData(10, 5, "01234.123");
				yield return new TestCaseData(10, 5, "+1234.123");
				yield return new TestCaseData(10, 5, "-1234.123");
            }
        }

		private static IEnumerable<TestCaseData> PrecisionGreaterThanValidatorPrecisionCases
		{
            get
            {
				yield return new TestCaseData(5, 2, "123123");
				yield return new TestCaseData(5, 2, "+12312");
				yield return new TestCaseData(5, 2, "-12312");
				yield return new TestCaseData(5, 4, "123.123");
				yield return new TestCaseData(3, 2, "123123");
				yield return new TestCaseData(1, 0, "123123");
				yield return new TestCaseData(1, 0, "12");
            }
        }		
		
		private static IEnumerable<TestCaseData> ScaleGreaterThanValidatorScaleCases
		{
            get
            {
				yield return new TestCaseData(15, 5, "123.123456");
				yield return new TestCaseData(15, 5, "0.123456789");
				yield return new TestCaseData(15, 5, "+123.1234561");
				yield return new TestCaseData(15, 5, "-123.1234561");
            }
        }			
		
		private static IEnumerable<TestCaseData> NegativeNumberWithValidatorOnlyPositiveAttributeCases
		{
            get
            {
				yield return new TestCaseData(10, 5, "-123");
				yield return new TestCaseData(10, 5, "-123.123456");
				yield return new TestCaseData(10, 5, "-0.123456");
				yield return new TestCaseData(10, 5, "-0");
            }
        }		
		
		private static IEnumerable<TestCaseData> OnlyFractionPartCases
		{
            get
            {
				yield return new TestCaseData(15, 10, ".123456");
				yield return new TestCaseData(15, 10, ".123");
				yield return new TestCaseData(15, 10, "+.123456");
				yield return new TestCaseData(15, 10, "-.123456");
            }
        }		
		
		private static IEnumerable<TestCaseData> IntegerPartWithPointButWithoutFractionPart
		{
            get
            {
				yield return new TestCaseData(15, 10, "1.");
				yield return new TestCaseData(15, 10, "12.");
				yield return new TestCaseData(15, 10, "+123.");
				yield return new TestCaseData(15, 10, "-123.");
            }
        }


		#endregion

		#region Instance Tests
		[Test, Category("Instance"), TestCaseSource(nameof(NonPositivePrecisionCases))]
		public void Instance_NonPositivePrecision_ShouldThrowException(int precision, int scale, bool onlyPositive)
        {
			Action action = () => new NumberValidator(precision, scale, onlyPositive);
			action.Should().Throw<ArgumentException>();
		}

		[Test, Category("Instance"), TestCaseSource(nameof(PositivePrecisionCases))]
		public void Instance_PositivePrecision_ShouldNotThrowException(int precision, int scale, bool onlyPositive)
		{
			Action action = () => new NumberValidator(precision, scale, onlyPositive);
			action.Should().NotThrow<ArgumentException>();
		}

		[Test, Category("Instance"), TestCaseSource(nameof(ScaleGreaterOrEqualToPrecisionCases))]
		public void Instance_ScaleGreaterOrEqualToPrecision_ShouldThrowException(int precision, int scale, bool onlyPositive)
		{
			Action action = () => new NumberValidator(1, 2, onlyPositive);
			action.Should().Throw<ArgumentException>();
		}

		[Test, Category("Instance"), TestCaseSource(nameof(NegativeScaleCases))]
		public void Instance_NegativeScale_ShouldThrowException(int precision, int scale, bool onlyPositive)
		{
			Action action = () => new NumberValidator(precision, scale, onlyPositive);
			action.Should().Throw<ArgumentException>();
		}
        #endregion

        #region IsValidNumber Tests
        [Test, Category("IsValidNumber"), TestCaseSource(nameof(NonDigitCharsCases))]
		public void IsValidNumber_NonDigitChars_ShouldBeFalse(int precision, int scale, string number)
        {
			NumberValidator numberValidator = new NumberValidator(precision, scale, false);
			numberValidator.IsValidNumber(number).Should().BeFalse();
        }

		[Test, Category("IsValidNumber"), TestCaseSource(nameof(OnlyIntegerPartCases))]
		public void IsValidNumber_OnlyIntegerPart_ShouldBeTrue(int precision, int scale, string number)
        {
			NumberValidator numberValidator = new NumberValidator(precision, scale, false);
			numberValidator.IsValidNumber(number).Should().BeTrue();
		}

		[Test, Category("IsValidNumber"), TestCaseSource(nameof(OnlyFractionPartCases))]
		public void IsValidNumber_OnlyFractionPart_ShouldBeFalse(int precision, int scale, string number)
		{
			NumberValidator numberValidator = new NumberValidator(precision, scale, false);
			numberValidator.IsValidNumber(number).Should().BeFalse();
		}

		[Test, Category("IsValidNumber"), TestCaseSource(nameof(IntegerPartAndFractionalPartCases))]
		public void IsValidNumber_IntegerPartAndFractionalPart_ShouldBeTrue(int precision, int scale, string number)
		{
			NumberValidator numberValidator = new NumberValidator(precision, scale, false);
			numberValidator.IsValidNumber(number).Should().BeTrue();
		}

		[Test, Category("IsValidNumber"), TestCaseSource(nameof(IntegerPartWithPointButWithoutFractionPart))]
		public void IsValidNumber_IntegerPartWithPointButWithoutFractionPart_ShouldBeFalse(int precision, int scale, string number)
		{
			NumberValidator numberValidator = new NumberValidator(precision, scale, false);
			numberValidator.IsValidNumber(number).Should().BeFalse();
		}

		[Test, Category("IsValidNumber"), TestCaseSource(nameof(PrecisionGreaterThanValidatorPrecisionCases))]
		public void IsValidNumber_PrecisionGreaterThanValidatorPrecision_ShouldBeFalse(int precision, int scale, string number)
		{
			NumberValidator numberValidator = new NumberValidator(precision, scale, false);
			numberValidator.IsValidNumber(number).Should().BeFalse();
		}

		[Test, Category("IsValidNumber"), TestCaseSource(nameof(ScaleGreaterThanValidatorScaleCases))]
		public void IsValidNumber_ScaleGreaterThanValidatorScale_ShouldBeFalse(int precision, int scale, string number)
		{
			NumberValidator numberValidator = new NumberValidator(precision, scale, false);
			numberValidator.IsValidNumber(number).Should().BeFalse();
		}

		[Test, Category("IsValidNumber"), TestCaseSource(nameof(NegativeNumberWithValidatorOnlyPositiveAttributeCases))]
		public void IsValidNumber_NegativeNumberWithValidatorOnlyPositiveAttribute_ShouldBeFalse(int precision, int scale, string number)
        {
			NumberValidator numberValidator = new NumberValidator(precision, scale, true);
			numberValidator.IsValidNumber(number).Should().BeFalse();
		}
		#endregion
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