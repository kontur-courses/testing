using System;
using System.Reflection;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidator_Should
	{
		[Test]
		public void Test()
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(-1, 2, true));
			Assert.DoesNotThrow(() => new NumberValidator(1, 0, true));
			Assert.Throws<ArgumentException>(() => new NumberValidator(-1, 2, false));
			Assert.DoesNotThrow(() => new NumberValidator(1, 0, true));

			Assert.IsTrue(new NumberValidator(17, 2, true).IsValidNumber("0.0"));
			Assert.IsTrue(new NumberValidator(17, 2, true).IsValidNumber("0"));
			Assert.IsTrue(new NumberValidator(17, 2, true).IsValidNumber("0.0"));
			Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("00.00"));
			Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("-0.00"));
			Assert.IsTrue(new NumberValidator(17, 2, true).IsValidNumber("0.0"));
			Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("+0.00"));
			Assert.IsTrue(new NumberValidator(4, 2, true).IsValidNumber("+1.23"));
			Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("+1.23"));
			Assert.IsFalse(new NumberValidator(17, 2, true).IsValidNumber("0.000"));
			Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("-1.23"));
			Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("a.sd"));
		}

		[TestCase("0")]
		[TestCase("0.0")]
		[TestCase("+1.23",4)]
		[TestCase("1.23", 3)]
		[TestCase("1.23", 4)]
		[TestCase("1.23", 1000)]
		[TestCase("-1.23", 4, 2, false)]
		[TestCase("-1.23", 1000, 2,false)]
		public void BeValid_With(string value, int precision = 17, int scale = 2, bool onlyPositive = true)=>
			new NumberValidator(precision,scale,onlyPositive).IsValidNumber(value).Should().BeTrue();

		[TestCase("00.00")]
		[TestCase("-0.00")]
		[TestCase("+0.00")]
		[TestCase("+1.23")]
		[TestCase("1.2345")]
		[TestCase("0.000",17)]
		[TestCase("-1.23")] 
		[TestCase("-1.23",4)]
		[TestCase("a.sd")]
		[TestCase("a.0")]
		[TestCase("0.sd")]
		[TestCase("0sd")]
		[TestCase("123123")]
		[TestCase("")]
		[TestCase(null)]
		public void BeInvalid_With(string value, int precision = 3, int scale = 2, bool onlyPositive = true) =>
			new NumberValidator(precision, scale, onlyPositive).IsValidNumber(value).Should().BeFalse();

		[TestCase(-1,2)]
		[TestCase(2, 2)]
		[TestCase(2, -2)]
		[TestCase(-1, 2, false)]
		[TestCase(2, 2, false)]
		[TestCase( 2, -2, false)]
		public void ThrowArgumentException_With(int precision, int scale, bool onlyPositive = true)=>
				Assert.Throws(typeof(ArgumentException), () => new NumberValidator(precision, scale, onlyPositive));

		[TestCase(1, 0)]	
		public void DoesNotThrow_With(int precision, int scale, bool onlyPositive = true)=>
				Assert.DoesNotThrow(() => new NumberValidator(precision, scale, onlyPositive));
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