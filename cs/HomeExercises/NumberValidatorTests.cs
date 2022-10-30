using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		// [TestCase(0, 2, TestName = "Zero precision")]
		// [TestCase(-1, 2, TestName = "Negative precision")]
		// [TestCase(10, -2, TestName = "Negative scale")]
		// [TestCase(5, 10, TestName = "Scale greater than precision")]
		// [TestCase(5, 5, TestName = "Scale equal precision")]
		[TestCaseSource(typeof(TestData), nameof(TestData.IncorrectCtorParams))]
		[Parallelizable(scope: ParallelScope.All)]	
		public void Ctor_IncorrectParams_ThrowArgumentException(int precision, int scale)
		{
			// ReSharper disable once ObjectCreationAsStatement
			var createNumberValidator = (Action) (() => new NumberValidator(precision, scale));
			createNumberValidator.Should().Throw<ArgumentException>();
		}

		// [TestCase(10, 5, TestName = "Precision greater than zero")]
		// [TestCase(10, 0, TestName = "Scale equal to zero")]
		[TestCaseSource(typeof(TestData), nameof(TestData.CorrectCtorParams))]
		[Parallelizable(scope: ParallelScope.All)]	
		public void Ctor_CorrectParams_NotThrowException(int precision, int scale)
		{
			// ReSharper disable once ObjectCreationAsStatement
			var createNumberValidator = (Action) (() => new NumberValidator(precision, scale));
			createNumberValidator.Should().NotThrow<Exception>();
		}

		// [TestCase(10, 5, true, "13.231", TestName = "Positive number")]
		// [TestCase(10, 5, false, "-13.231", TestName = "Negative number")]
		// [TestCase(10, 5, true, "+13.231", TestName = "Positive number with '+'")]
		// [TestCase(4, 1, true, "+13.3", TestName = "'+' include in precision")]
		// [TestCase(4, 1, false, "-13.3", TestName = "'-' include in precision")]
		// [TestCase(10, 5, false, "1,3", TestName = "Value with ','")]
		[TestCaseSource(typeof(TestData), nameof(TestData.ValidParams))]
		[Parallelizable(scope: ParallelScope.All)]	
		public void IsValidNumber_ValidParams_True(int precision, int scale, bool onlyPositive, string value)
		{
			var numberValidator = new NumberValidator(precision, scale, onlyPositive);
			numberValidator.IsValidNumber(value).Should().BeTrue();
		}

		// [TestCase(3, 1, true, "123.1", TestName = "Value grater than precision")]
		// [TestCase(3, 1, true, "+123", TestName = "'+' include in precision")]
		// [TestCase(3, 1, false, "-123", TestName = "'-' include in precision")]
		[TestCaseSource(typeof(TestData), nameof(TestData.ValueWithLengthGreaterThanMaxPrecision))]
		[Parallelizable(scope: ParallelScope.All)]	
		public void IsValidNumber_ValueWithLengthGreaterThanMaxPrecision_False(int precision, int scale,
			bool onlyPositive, string value)
		{
			var numberValidator = new NumberValidator(precision, scale, onlyPositive);
			numberValidator.IsValidNumber(value).Should().BeFalse();
		}

		
		// [TestCase(10, 5, false, null, TestName = "Null value")]
		// [TestCase(10, 5, false, "", TestName = "Empty string value")]
		[TestCaseSource(typeof(TestData), nameof(TestData.NullOrEmptyString))]
		[Parallelizable(scope: ParallelScope.All)]	
		public void IsValidNumber_NullOrEmptyString_False(int precision, int scale, bool onlyPositive, string value)
		{
			var numberValidator = new NumberValidator(precision, scale, onlyPositive);
			numberValidator.IsValidNumber(value).Should().BeFalse();		
		}
		
		// [TestCase(10, 5, false, "adfja", TestName = "Letters in value")]
		// [TestCase(10, 5, false, "1,,,,3", TestName = "Value contais a lot of ','")]
		// [TestCase(10, 5, false, "1;3", TestName = "Value contais incorrect seperator")]
		// [TestCase(10, 5, false, ";1.3", TestName = "Value start from incorrect symbol ';'")]
		[TestCaseSource(typeof(TestData), nameof(TestData.IncorrectValueForRegex))]
		[Parallelizable(scope: ParallelScope.All)]	
		public void IsValidNumber_IncorrectValueForRegex_False(int precision, int scale, bool onlyPositive,
			string value)
		{
			var numberValidator = new NumberValidator(precision, scale, onlyPositive);
			numberValidator.IsValidNumber(value).Should().BeFalse();
		}

		[Test]
		[Parallelizable(scope: ParallelScope.Self)]	
		public void IsValidNumber_ParamOfScaleGreaterThanMaxScaleOfValidator_False()
		{
			var numberValidator = new NumberValidator(5, 1);
			numberValidator.IsValidNumber("1.32").Should().BeFalse();
		}

		[Test]
		[Parallelizable(scope: ParallelScope.Self)]
		public void IsValidNumber_NegativeValueWithOnlyPositiveParamTrue_False()
		{
			var numberValidator = new NumberValidator(5, 2, true);
			numberValidator.IsValidNumber("-1.3").Should().BeFalse();		
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

			var intPart = match.Groups[1].Value.Length + match.Groups[2].Value.Length;
			var fracPart = match.Groups[4].Value.Length;

			if (intPart + fracPart > precision || fracPart > scale)
				return false;

			if (onlyPositive && match.Groups[1].Value == "-")
				return false;
			return true;
		}
	}

	public class TestData
	{
		public static TestCaseData[] IncorrectCtorParams =
		{
			new TestCaseData(0, 2).SetName("Zero precision"), new TestCaseData(-1, 2).SetName("Negative precision"),
			new TestCaseData(10, -2).SetName("Negative scale"), new TestCaseData(5, 10)
				.SetName("Scale greater than precision"), new TestCaseData(5, 5).SetName("Scale equal precision")
		};

		public static TestCaseData[] CorrectCtorParams =
		{
			new TestCaseData(10, 5).SetName("Precision greater than zero"),
			new TestCaseData(10, 0).SetName("Scale equal to zero")
		};

		public static TestCaseData[] ValidParams =
		{
			new TestCaseData(10, 5, true, "13.231").SetName("Positive number"),
			new TestCaseData(10, 5, false, "-13.231").SetName("Negative number"),
			new TestCaseData(10, 5, true, "+13.231").SetName("Positive number with '+'"),
			new TestCaseData(4, 1, true, "+13.3").SetName("'+' include in precision"),
			new TestCaseData(4, 1, false, "-13.3").SetName("'-' include in precision"),
			new TestCaseData(10, 5, false, "1,3").SetName("Value with ','")
		};

		public static TestCaseData[] ValueWithLengthGreaterThanMaxPrecision =
		{
			new TestCaseData(3, 1, true, "123.1").SetName("Value grater than precision"),
			new TestCaseData(3, 1, true, "+123").SetName("'+' include in precision"),
			new TestCaseData(3, 1, false, "-123").SetName("'-' include in precision")
		};

		public static TestCaseData[] NullOrEmptyString =
		{
			new TestCaseData(10, 5, false, null).SetName("Null value"),
			new TestCaseData(10, 5, false, "").SetName("Empty string value")
		};

		public static TestCaseData[] IncorrectValueForRegex =
		{
			new TestCaseData(10, 5, false, "adfa").SetName("Letters in value"),
			new TestCaseData(10, 5, false, "adf.f").SetName("Letters with '.'"),
			new TestCaseData(10, 5, false, "1.a").SetName("Letter in fractal part"),
			new TestCaseData(10, 5, false, "1,,,4").SetName("Value contais a lot of ','"),
			new TestCaseData(10, 5, false, "1;3").SetName("Value contais incorrect seperator"),
			new TestCaseData(10, 5, false, ";1.3").SetName("Value start from incorrect symbol ';'")
		};
	}
}