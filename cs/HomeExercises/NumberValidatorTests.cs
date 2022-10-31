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

		private static IEnumerable<TestCaseData> CorrectParamsForNumberValidator
        {
			get
            {
				yield return new TestCaseData(10, 5, true).SetName("Correct params");
				yield return new TestCaseData(10, 5, false).SetName("Correct params");
				yield return new TestCaseData(2, 1, true).SetName("Correct params");
				yield return new TestCaseData(2, 1, false).SetName("Correct params");
			}
        }
		private static IEnumerable<TestCaseData> IncorrectParamsForNumberValidator
		{
			get
			{
				yield return new TestCaseData(-1, 2, true).SetName("Precision < 0");
				yield return new TestCaseData(-1, 2, false).SetName("Precision < 0");
				yield return new TestCaseData(-2, 1, true).SetName("Precision < 0");
				yield return new TestCaseData(-2, 1, false).SetName("Precision < 0");
				yield return new TestCaseData(0, 1, true).SetName("Precision == 0");
				yield return new TestCaseData(0, 1, false).SetName("Precision == 0");

				yield return new TestCaseData(5, 10, true).SetName("Scale > precision");
				yield return new TestCaseData(5, 10, false).SetName("Scale > precision");
				yield return new TestCaseData(1, 2, true).SetName("Scale > precision");
				yield return new TestCaseData(1, 2, false).SetName("Scale > precision");
				yield return new TestCaseData(1, 1, true).SetName("Scale == precision");
				yield return new TestCaseData(1, 1, false).SetName("Scale == precision");

				yield return new TestCaseData(5, -10, true).SetName("Scale < 0");
				yield return new TestCaseData(5, -10, false).SetName("Scale < 0");
				yield return new TestCaseData(1, -2, true).SetName("Scale < 0");
				yield return new TestCaseData(1, -2, false).SetName("Scale < 0");
			}
		}
		private static IEnumerable<TestCaseData> NumbersForValidation
		{
			get
			{
				yield return new TestCaseData(10, 5, false, "a").SetName("Number cannot include letters").Returns(false);
				yield return new TestCaseData(10, 5, false, "a.55").SetName("Number cannot include letters").Returns(false);
				yield return new TestCaseData(10, 5, false, "a.").SetName("Number cannot include letters").Returns(false);
				yield return new TestCaseData(10, 5, false, "5.bc").SetName("Number cannot include letters").Returns(false);
				yield return new TestCaseData(10, 5, false, ".bc").SetName("Number cannot include letters").Returns(false);
				yield return new TestCaseData(10, 5, false, "a.bc").SetName("Number cannot include letters").Returns(false);
				yield return new TestCaseData(10, 5, false, "+a.bc").SetName("Number cannot include letters").Returns(false);
				yield return new TestCaseData(10, 5, false, "-a.bc").SetName("Number cannot include letters").Returns(false);
				
				yield return new TestCaseData(10, 5, false, "12;3").SetName("Number part separator must be . or ,").Returns(false);
				yield return new TestCaseData(10, 5, false, "12/3").SetName("Number part separator must be . or ,").Returns(false);
				yield return new TestCaseData(10, 5, false, "12|3").SetName("Number part separator must be . or ,").Returns(false);
				yield return new TestCaseData(10, 5, false, "12?3").SetName("Number part separator must be . or ,").Returns(false);

                yield return new TestCaseData(10, 5, false, "12.3").SetName("Number part separator may be . or ,").Returns(true);
                yield return new TestCaseData(10, 5, false, "12,3").SetName("Number part separator may be . or ,").Returns(true);
                yield return new TestCaseData(10, 5, false, "1.23").SetName("Number part separator may be . or ,").Returns(true);
                yield return new TestCaseData(10, 5, false, "1,23").SetName("Number part separator may be . or ,").Returns(true);

                yield return new TestCaseData(10, 5, false, "1234").SetName("Number may have only integer part").Returns(true);
				yield return new TestCaseData(10, 5, false, "01234").SetName("Number may have only integer part").Returns(true);
				yield return new TestCaseData(10, 5, false, "+1234").SetName("Number may have only integer part").Returns(true);
				yield return new TestCaseData(10, 5, false, "-1234").SetName("Number may have only integer part").Returns(true);

				yield return new TestCaseData(10, 5, false, ".123").SetName("Number must include integer part").Returns(false);
				yield return new TestCaseData(10, 5, false, ".12").SetName("Number must include integer part").Returns(false);
				yield return new TestCaseData(10, 5, false, "+.123").SetName("Number must include integer part").Returns(false);
				yield return new TestCaseData(10, 5, false, "-.123").SetName("Number must include integer part").Returns(false);

				yield return new TestCaseData(10, 5, false, "1.").SetName("Number must include fractional part after part's separator").Returns(false);
				yield return new TestCaseData(10, 5, false, "12.").SetName("Number must include fractional part after part's separator").Returns(false);
				yield return new TestCaseData(10, 5, false, "+123.").SetName("Number must include fractional part after part's separator").Returns(false);
				yield return new TestCaseData(10, 5, false, "-123.").SetName("Number must include fractional part after part's separator").Returns(false);

				yield return new TestCaseData(10, 5, false, "0.0").SetName("Number must include fractional part after part's separator").Returns(true);
				yield return new TestCaseData(10, 5, false, "0.000").SetName("Number must include fractional part after part's separator").Returns(true);
				yield return new TestCaseData(10, 5, false, "1234.123").SetName("Number must include fractional part after part's separator").Returns(true);
				yield return new TestCaseData(10, 5, false, "01234.123").SetName("Number must include fractional part after part's separator").Returns(true);
				yield return new TestCaseData(10, 5, false, "+1234.123").SetName("Number must include fractional part after part's separator").Returns(true);
				yield return new TestCaseData(10, 5, false, "-1234.123").SetName("Number must include fractional part after part's separator").Returns(true);

				yield return new TestCaseData(10, 5, true, "-123").SetName("Number cannot be negative if onlyPositive == true").Returns(false);
				yield return new TestCaseData(10, 5, true, "-123.123456").SetName("Number cannot be negative if onlyPositive == true").Returns(false);
				yield return new TestCaseData(10, 5, true, "-0.123456").SetName("Number cannot be negative if onlyPositive == true").Returns(false);
				yield return new TestCaseData(10, 5, true, "-0").SetName("Number cannot be negative if onlyPositive == true").Returns(false);

				yield return new TestCaseData(10, 5, false, "123.123456").SetName("Number scale > Validator scale").Returns(false);
				yield return new TestCaseData(10, 5, false, "0.123456789").SetName("Number scale > Validator scale").Returns(false);
				yield return new TestCaseData(10, 5, false, "+123.1234561").SetName("Number scale > Validator scale").Returns(false);
				yield return new TestCaseData(10, 5, false, "-123.1234561").SetName("Number scale > Validator scale").Returns(false);

				yield return new TestCaseData(5, 2, false, "123123").SetName("Number precision > Validator precision").Returns(false);
				yield return new TestCaseData(5, 2, false, "+12312").SetName("Number precision > Validator precision").Returns(false);
				yield return new TestCaseData(5, 2, false, "-12312").SetName("Number precision > Validator precision").Returns(false);
				yield return new TestCaseData(5, 4, false, "123.123").SetName("Number precision > Validator precision").Returns(false);
				yield return new TestCaseData(3, 2, false, "123123").SetName("Number precision > Validator precision").Returns(false);
				yield return new TestCaseData(1, 0, false, "123123").SetName("Number precision > Validator precision").Returns(false);
				yield return new TestCaseData(1, 0, false, "12").SetName("Number precision > Validator precision").Returns(false);
				
				yield return new TestCaseData(10, 5, false, "++12").SetName("Number cannot have several signs").Returns(false);
				yield return new TestCaseData(10, 5, false, "+-12").SetName("Number cannot have several signs").Returns(false);
				yield return new TestCaseData(10, 5, false, "-+12").SetName("Number cannot have several signs").Returns(false);
				yield return new TestCaseData(10, 5, false, "--12").SetName("Number cannot have several signs").Returns(false);
				
				yield return new TestCaseData(10, 5, false, string.Empty).SetName("Number cannot be empty").Returns(false);
				yield return new TestCaseData(10, 5, false, null).SetName("Number cannot be null").Returns(false);			}
		}

		#endregion

		[TestCaseSource(nameof(CorrectParamsForNumberValidator)), Category("Correct params for number validator")]
		[Parallelizable(scope: ParallelScope.All)]
		public void Instance_ShouldNotThrowException(int precision, int scale, bool onlyPositive)
		{
			Action instantiatingNumberValidator = () =>
			{ NumberValidator number = new NumberValidator(precision, scale, onlyPositive); };
			instantiatingNumberValidator.Should().NotThrow<ArgumentException>();
		}

		[TestCaseSource(nameof(IncorrectParamsForNumberValidator)), Category("Incorrect params for number validator")]
		[Parallelizable(scope: ParallelScope.All)]
		public void Instance_ShouldThrowException(int precision, int scale, bool onlyPositive)
        {
			Action instantiatingNumberValidator = () =>
				{ NumberValidator number = new NumberValidator(precision, scale, onlyPositive); };
			instantiatingNumberValidator.Should().Throw<ArgumentException>();
		}

		[TestCaseSource(nameof(NumbersForValidation)), Category("Number validation")]
		[Parallelizable(scope: ParallelScope.All)]
		public bool IsValidNumber(int precision, int scale, bool onlyPositive, string number)
        {
			NumberValidator numberValidator = new NumberValidator(precision, scale, onlyPositive);
			return numberValidator.IsValidNumber(number);
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