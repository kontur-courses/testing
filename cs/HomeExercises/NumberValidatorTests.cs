using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		private static readonly TestCaseData[] casesForIncorrectNumber =
		{
			new TestCaseData(new NumberValidator(precision: 17, scale: 2, onlyPositive: true), null)
				.SetName("number is null"),
			new TestCaseData(new NumberValidator(precision: 17, scale: 2, onlyPositive: true), "")
				.SetName("number is empty"),
			new TestCaseData(new NumberValidator(precision: 17, scale: 4, onlyPositive: true), "/89")
				.SetName("number starts with wrong sign"),
			new TestCaseData(new NumberValidator(precision: 17, scale: 4, onlyPositive: true), "0.0.0")
				.SetName("two dots"),
			new TestCaseData(new NumberValidator(precision: 17, scale: 4, onlyPositive: true), ".0")
				.SetName("without intPart"),
			new TestCaseData(new NumberValidator(precision: 17, scale: 2, onlyPositive: true), "0.")
				.SetName("without fracPart"),
			new TestCaseData(new NumberValidator(precision: 17, scale: 2, onlyPositive: true), "++2")
				.SetName("too many signes"),
			new TestCaseData(new NumberValidator(precision: 17, scale: 2, onlyPositive: true), "ab.c")
				.SetName("letters instead of numbers"),
			new TestCaseData(new NumberValidator(precision: 17, scale: 2, onlyPositive: true), "-2.3")
				.SetName("expected only positive but was negative number"),
			new TestCaseData(new NumberValidator(precision: 17, scale: 2, onlyPositive: true), "2.333")
				.SetName("number length after dot > scale"),
			new TestCaseData(new NumberValidator(precision: 3, scale: 2, onlyPositive: true), "00.00")
				.SetName("number length > precision"),
			new TestCaseData(new NumberValidator(precision: 3, scale: 2, onlyPositive: true), "-0.00")
				.SetName("number length with sign > precision"),
			new TestCaseData(new NumberValidator(precision: 3, scale: 0, onlyPositive: true), "2.33")
				.SetName("expected int but was double")
		};

		private static readonly TestCaseData[] casesForCorrectNumber =
		{
			new TestCaseData(new NumberValidator(precision: 17, scale: 2, onlyPositive: false), "-2.3")
				.SetName("negative number"),
			new TestCaseData(new NumberValidator(precision: 17, scale: 2, onlyPositive: true), "2,3")
				.SetName("use comma"),
			new TestCaseData(new NumberValidator(precision: 17, scale: 2, onlyPositive: true), "2.3")
				.SetName("regular number"),
			new TestCaseData(new NumberValidator(precision: 17, scale: 2, onlyPositive: false), "-2.3")
				.SetName("regular double negative number"),
			new TestCaseData(new NumberValidator(precision: 17, scale: 2, onlyPositive: true), "5")
				.SetName("regular int number")
		};

		private static readonly TestCaseData[] casesForIncorrectCreation =
		{
			new TestCaseData(-1, 2).SetName("negative precision"),
			new TestCaseData(1, -2).SetName("negative scale"),
			new TestCaseData(3, 8).SetName("precision < scale"),
			new TestCaseData(0, 0).SetName("precision = 0"),
			new TestCaseData(2, 2).SetName("precision = scale")
		};

		[TestCaseSource(nameof(casesForIncorrectNumber))]
		public void Check_That_Number_Is_Incorrect_When(NumberValidator validator, string number)
		{
			validator.IsValidNumber(number).Should().BeFalse();
		}

		[TestCaseSource(nameof(casesForCorrectNumber))]
		public void Check_That_Number_Is_Correct_When(NumberValidator validator, string number)
		{
			validator.IsValidNumber(number).Should().BeTrue();
		}

		[TestCaseSource(nameof(casesForIncorrectCreation))]
		public void NumberValidator_ThrowException_On(int precision, int scale)
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(precision, scale, onlyPositive: true));
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