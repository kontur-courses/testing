using System;
using System.Dynamic;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using FluentAssertions;
using NUnit.Framework;
using NUnit.Framework.Internal.Filters;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		
		[TestCase(-1,0, TestName = "Precision must be a positive number")]
		[TestCase(0,0, TestName = "Precision must be > 0")]
		[TestCase(5,5, TestName = "Precision must be > scale")]
		[TestCase(5,-5, TestName = "Scale must be >= 0")]
		public void ConstractorTest(int precision, int scale)
		{
			Action act = () => new NumberValidator(precision,scale);
			act.Should().Throw<ArgumentException>();
		}

		[TestCase(10, 9, "abc.dbc", false, TestName = "Not number input")]
		[TestCase(10, 9, "-.000", false, TestName = "No digits befor point")]
		[TestCase(10, 9, "-10.", false, TestName = "No digits after point")]
		[TestCase(10, 9, "++10.", false, TestName = "Double signs is't allowed")]
		[TestCase(10, 9, "-10", true, TestName = "No point in input")]
		[TestCase(10,9, "   ", false, TestName =  "White Space will fail")]
		[TestCase(10,9,null,false,TestName = "Null string will fail")]
		public void RegExpTest(int precision, int scale, string input, bool expected)
		{
			new NumberValidator(precision, scale, false).IsValidNumber(input).Should().Be(expected);
		}
	
		[TestCase(5,4,"1234.45",false, TestName = "Precision overflow but scale is not overflow")]
		[TestCase(10,4,"12.123412",false, TestName = "Scale overflow but precision is not overlow")]
		[TestCase(10,4,"123123,12312",false, TestName = "Precision and Scale overflow")]
		[TestCase(10,4,"12.34",true, TestName = "Valid input")]
		[TestCase(5,4,"+123.22",false, TestName = "Sign can case Overflow")]
		public void PrecisionAndScaleOverflowingTest(int precision, int scale, string input, bool expected)
		{
			new NumberValidator(precision, scale, false).IsValidNumber(input).Should().Be(expected);
		}

		[TestCase(10,9,"-12.10",false, TestName = "Return false on Negative input")]
		[TestCase(10,9,"12,10",true,TestName = "Return true on Positive input")]
		[TestCase(10,9,"+12,10",true,TestName = "+sign doesn't make any sence")]
		public void PositiveNumberValidatorTest(int precision, int scale, string input, bool expected)
		{
			new NumberValidator(precision, scale,true).IsValidNumber(input).Should().Be(expected);
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