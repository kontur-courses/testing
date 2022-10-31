using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[TestCaseSource(typeof(TestData), nameof(TestData.IncorrectCtorParams))]
		[Parallelizable(scope: ParallelScope.All)]	
		public void Ctor_IncorrectParams_ThrowArgumentException(int precision, int scale)
		{
			// ReSharper disable once ObjectCreationAsStatement
			var createNumberValidator = (Action) (() => new NumberValidator(precision, scale));
			createNumberValidator.Should().Throw<ArgumentException>();
		}

		[TestCaseSource(typeof(TestData), nameof(TestData.CorrectCtorParams))]
		[Parallelizable(scope: ParallelScope.All)]	
		public void Ctor_CorrectParams_NotThrowException(int precision, int scale)
		{
			// ReSharper disable once ObjectCreationAsStatement
			var createNumberValidator = (Action) (() => new NumberValidator(precision, scale));
			createNumberValidator.Should().NotThrow<Exception>();
		}

		[TestCaseSource(typeof(TestData), nameof(TestData.ValidNumberParams))]
		[Parallelizable(scope: ParallelScope.All)]
		public bool NumberValidator_IsValidNumber(int precision, int scale, bool onlyPositive, string value)
		{
			var numberValidator = new NumberValidator(precision, scale, onlyPositive);
			return numberValidator.IsValidNumber(value);
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

		public static TestCaseData[] ValidNumberParams =
		{
			new TestCaseData(10, 5, true, "13.231").SetName("Valid number").Returns(true),
			new TestCaseData(10, 5, false, "-13.231").SetName("Valid number with '-'").Returns(true),
			new TestCaseData(10, 5, true, "+13.231").SetName("Valid number with '+'").Returns(true),
			new TestCaseData(4, 1, true, "+13.3").SetName("'+' most include in precision").Returns(true),
			new TestCaseData(4, 1, false, "-13.3").SetName("'-' most include in precision").Returns(true),
			new TestCaseData(10, 5, false, "1,3").SetName("Value most to be correct with ','").Returns(true),

			new TestCaseData(10, 5, false, null).SetName("Null value").Returns(false),
			new TestCaseData(10, 5, false, "").SetName("Empty string value").Returns(false),
			new TestCaseData(10, 5, false, "adfa").SetName("Letters in value").Returns(false),
			new TestCaseData(10, 5, false, "adf.f").SetName("Letters in int and frac parts").Returns(false),
			new TestCaseData(10, 5, false, "1.a").SetName("Letter in fractal part").Returns(false),
			new TestCaseData(10, 5, false, "1;3").SetName("Value contais incorrect seperator").Returns(false),
			
			new TestCaseData(5, 1, false, "1.32").SetName("Scale greater than max scale of validator")
				.Returns(false),
			new TestCaseData(5, 2, true, "-1.3").SetName("Negative value with onlyPositive param in true")
				.Returns(false),
			new TestCaseData(10, 5, false, ";1.3").SetName("Value start from incorrect symbol ';'")
				.Returns(false),
			new TestCaseData(10, 5, false, "1,,,4").SetName("Value contais a lot of seperators ','")
				.Returns(false),
			new TestCaseData(3, 1, true, "123.1").SetName("Int part length grater than precision")
				.Returns(false)
		};
	}
}