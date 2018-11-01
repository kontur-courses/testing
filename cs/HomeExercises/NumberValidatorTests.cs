using System;
using System.Dynamic;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using FluentAssertions;
using NUnit.Framework;
using NUnit.Framework.Internal.Filters;

namespace HomeExercises
{
	public class NumberValidatorTests
	{

		private NumberValidator CreateNumberValidator(int prec, int scale, bool onlyPositive = false)
		{
			return new NumberValidator(prec,scale,onlyPositive);
		}
		
		[TestCase(-1)]
		[TestCase(0)]
		[TestCase(-3)]
		[TestCase(-100)]
		public void ConstructorWillThrowExceptionIfPrecisionLessOrEqualZero(int precision)
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(precision, 2, true));
		}

		[TestCase(-1)]
		[TestCase(-23)]
		public void ConstructorWillThrowExceptionIfScaleLessThenZero(int scale)
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(10, scale, true));
		}
		
		[Test]
		public void NumberValidatorConstructorWillNotThrowExceptionIfScaleEqualsToZero()
		{
			Assert.DoesNotThrow(() => new NumberValidator(10,0,false));
		}

		[TestCase(10, 11)]
		[TestCase(10, 10)]
		[TestCase(1, 2)]
		[TestCase(1, 10)]
		public void  ConstructorWillThrowExceptionIfScaleGraterThanPrecision(int precision, int scale)
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(precision, scale, true));
		}

		[Description("Проверяем , что NumberValidator ")]
		[TestCase(10,9,null)]
		[TestCase(10,9,"")]
		[TestCase(10,9," ")]
		[TestCase(10,9,"     ")]
		public void WillFailOnNullOrEmptyInput(int prec, int scale, string input)
		{
			CreateNumberValidator(prec,scale).IsValidNumber(input).Should().BeFalse();
		}
		
		[Description("Проверяем, что положительный NumberValidator работает корректно.")]
		[TestCase(10,9,"-23.1",false)]
		[TestCase(10,9,"10.1",true)]
		[TestCase(10,9,"+11.1",true)]
		[TestCase(10,9,"-123.23",false)]
		public void PositiveNumberValidatorTest(int prec, int scale, string input,bool expected)
		{
			CreateNumberValidator(prec,scale, true).IsValidNumber(input).Should().Be(expected);
		}

		[Description("Проверяем, что RegExp работает корректно.")]
		[TestCase("asd.ddt")]
		[TestCase("WierdChinesSymbols.exe")]
		[TestCase("1111a11.322")]
		public void WillFailOnNotNumberInput(string input)
		{
			CreateNumberValidator(100, 99, false).IsValidNumber(input).Should().BeFalse();
		}

		[Description("Проверяем переполнение дробной части.")]
		[TestCase(10,3,"23.1000",false)]
		[TestCase(10,4,"-23.10000",false)]
		[TestCase(10,1,"0.011",false)]
		[TestCase(10,3,"0,123",true)]
		public void ScaleOverflowTest(int prec, int scale, string input,bool expected)
		{
			CreateNumberValidator(prec, scale, false).IsValidNumber(input).Should().Be(expected);
		}
		[Description("Проверяем перполнение precision.")]
		[TestCase(5,3,"230.10",true)]
		[TestCase(5,3,"+230.10",false)]
		[TestCase(3,2,"000",true)]
		[TestCase(3,2,"0000",false)]
		public void PrecisionOverflowTest(int prec, int scale, string input,bool expected)
		{
			CreateNumberValidator(prec, scale, false).IsValidNumber(input).Should().Be(expected);
		}

		[Description("Проверяем, что более чем 1 знак недопустим")]
		[TestCase("++32.0")]
		[TestCase("+-32.0")]
		[TestCase("-+32.0")]
		[TestCase("--32.0")]
		public void NumberValidatorFailIfInputHasMoreThanTwoSigns(string input)
		{
			CreateNumberValidator(100, 99).IsValidNumber(input).Should().BeFalse();
		}
		
		[Description("....")]
		[TestCase(100,99,"+.000",false)]
		[TestCase(5,3,"+0.",false)]
		[TestCase(3,2,"+.",false)]
		public void SomeWierdTests(int prec, int scale, string input,bool expected)
		{
			CreateNumberValidator(prec, scale, false).IsValidNumber(input).Should().Be(expected);
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