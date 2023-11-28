using System;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[Test]
		public void NumberValidatorCtor_WhenPassNegativePrecision_ShouldThrowsArgumentException()
		{
			TestDelegate testDelegate = () => new NumberValidator(-1, 2, true);

			Assert.Throws<ArgumentException>(testDelegate);
		}

		[Test]
		public void NumberValidatorCtor_WhenPassNegativeScale_ShouldThrowsArgumentException()
		{
			TestDelegate testDelegate = () => new NumberValidator(1, -2);

			Assert.Throws<ArgumentException>(testDelegate);
		}

		[Test]
		public void NumberValidatorCtor_WhenPassValidArguments_ShouldDoesNotThrows()
		{
			TestDelegate testDelegate = () => new NumberValidator(1, 0, true);

			Assert.DoesNotThrow(testDelegate);
		}

		[Test]
		public void NumberValidatorCtor_WhenPrecisionIsEqualToTheScale_ShouldThrowsArgumentException()
		{
			TestDelegate testDelegate = () => new NumberValidator(2, 2, true);

			Assert.Throws<ArgumentException>(testDelegate);
		}

		public void IsValidNumberTest(int precision, int scale, bool onlyPositive,
			string number, bool expectedResult)
		{
			var validator = new NumberValidator(precision, scale, onlyPositive);

			var actualResult = validator.IsValidNumber(number);

			Assert.AreEqual(expectedResult, actualResult);
		}

		[Test]
		[TestOf(nameof(NumberValidator.IsValidNumber))]
		public void WhenFractionalPartIsMissing_ShouldReturnTrue()
		{
			IsValidNumberTest(17,2,true,"0", true);
		}

        [Test]
		[TestOf(nameof(NumberValidator.IsValidNumber))]
        public void WhenLettersInsteadOfNumber_ShouldReturnFalse()
        {
	        IsValidNumberTest(3, 2, true, "a.sd", false);
		}

		[Test]
		[TestOf(nameof(NumberValidator.IsValidNumber))]
        public void WhenSymbolsInsteadOfNumber_ShouldReturnFalse()
		{
			IsValidNumberTest(3, 2, true, "2.!", false);
		}

		[Test]
		[TestOf(nameof(NumberValidator.IsValidNumber))]
        public void WhenNumberIsNull_ShouldReturnFalse()
		{
			IsValidNumberTest(17,2,true, null!, false);
		}

		[Test]
		[TestOf(nameof(NumberValidator.IsValidNumber))]
        public void WhenPassNumberIsEmpty_ShouldReturnFalse()
		{
			IsValidNumberTest(3,2,true,"", false);
		}

		[Test]
		[TestOf(nameof(NumberValidator.IsValidNumber))]
        public void WhenIntPartWithNegativeSignMoreThanPrecision_ShouldReturnFalse()
		{
			IsValidNumberTest(3,2,true,"-0.00", false);
		}

		[Test]
		[TestOf(nameof(NumberValidator.IsValidNumber))]
        public void WhenIntPartWithPositiveSignMoreThanPrecision_ShouldReturnFalse()
		{
			IsValidNumberTest(3,2,true,"+1.23", false);
		}

		[Test]
		[TestOf(nameof(NumberValidator.IsValidNumber))]
        public void WhenFractionalPartMoreThanScale_ShouldReturnFalse()
		{
			IsValidNumberTest(17,2,true, "0.000", false);
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