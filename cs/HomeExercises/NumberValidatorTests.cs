using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[Test]
		public void NumberValidator_PrecisionIsNegative_ThrowException()
		{
			Action act = () => new NumberValidator(-1, 2, true);
			act.ShouldThrow<ArgumentException>();
		}
		
		[Test] 
		public void NumberValidator_ValidValues_WithoutException()
		{
			Action act = () => new NumberValidator(1, 0, true);
			act.ShouldNotThrow();
		}
		
		[Test]
		public void NumberValidator_ScaleIsNegative_ThrowException()
		{
			Action act = () => new NumberValidator(5, -5, true);
			act.ShouldThrow<ArgumentException>();
		}
		
		[Test]
		public void NumberValidator_ScaleGreaterPrecision_ThrowException()
		{
			Action act = () => new NumberValidator(3, 5, true);
			act.ShouldThrow<ArgumentException>();
		}

		[Test] //my
		public void IsValidNumber_Null_IsFalse()
		{
			var isValidNumber = new NumberValidator(10, 2, true).IsValidNumber(null);
			isValidNumber.Should().BeFalse();
		}
		
		[Test] //my
		public void IsValidNumber_EmptyLine_IsFalse()
		{
			var isValidNumber = new NumberValidator(10, 2, true).IsValidNumber("");
			isValidNumber.Should().BeFalse();
		}
		
		[Test]
		public void IsValidNumber_FloatPositiveNumberOnlyPositive_IsTrue()
		{
			var isValidNumber = new NumberValidator(17, 2, true).IsValidNumber("0.0");
			isValidNumber.Should().BeTrue();
		}
		
		[Test]//my
		public void IsValidNumber_FloatNegativeNumberOnlyPositive_IsFalse()
		{
			var isValidNumber = new NumberValidator(17, 2, true).IsValidNumber("-1.0");
			isValidNumber.Should().BeFalse();
		}
		
		[Test]
		public void IsValidNumber_FloatWithPlusOnlyPositive_IsTrue()
		{
			var isValidValue = new NumberValidator(4, 2, true).IsValidNumber("+1.23");
			isValidValue.Should().BeTrue();
		}
		
		[Test]
		public void IsValidNumber_IntPositiveNumberOnlyPositive_IsTrue()
		{
			var isValidNumber = new NumberValidator(17, 2, true).IsValidNumber("0");
			isValidNumber.Should().BeTrue();
		}
		
		[Test] //my
		public void IsValidNumber_IntNegativeNumberOnlyPositive_IsFalse()
		{
			var isValidNumber = new NumberValidator(17, 2, true).IsValidNumber("-5");
			isValidNumber.Should().BeFalse();
		}
		
		[Test]
		public void IsValidNumber_LineWithLetters_IsFalse()
		{
			var isValidValue = new NumberValidator(3, 2, true).IsValidNumber("a.sd");
			isValidValue.Should().BeFalse();
		}
		
		[Test]
		public void IsValidNumber_LengthGreaterPrecision_IsFalse()
		{
			var isValidValue = new NumberValidator(3, 2, true).IsValidNumber("00.00");
			isValidValue.Should().BeFalse();
		}
		
		[Test]
		public void IsValidNumber_LengthWitMinusGreaterPrecision_IsFalse()
		{
			var isValidValue = new NumberValidator(3, 2, true).IsValidNumber("-0.00");
			isValidValue.Should().BeFalse();
		}
		
		[Test]
		public void IsValidNumber_LengthWithPlusGreaterPrecision_IsFalse()
		{
			var isValidValue = new NumberValidator(3, 2, true).IsValidNumber("+0.00");
			isValidValue.Should().BeFalse();
		}

		[Test]
		public void IsValidNumber_LengthFractionalPartGreaterScale_IsFalse()
		{
			var isValidValue = new NumberValidator(17, 2, true).IsValidNumber("0.000");
			isValidValue.Should().BeFalse();
		}
		
		[Test]
		public void IsValidNumber_LengthFloatNumberWithMinusGreaterPrecision_IsFalse()
		{
			var isValidValue = new NumberValidator(3, 2, true).IsValidNumber("-1.23");
			isValidValue.Should().BeFalse();
		}
		
		[Test]
		public void Test()
		{
			/*Assert.Throws<ArgumentException>(() => new NumberValidator(-1, 2, true));
			Assert.DoesNotThrow(() => new NumberValidator(1, 0, true));
			Assert.Throws<ArgumentException>(() => new NumberValidator(-1, 2, false));
			Assert.DoesNotThrow(() => new NumberValidator(1, 0, true));  //повтор

			Assert.IsTrue(new NumberValidator(17, 2, true).IsValidNumber("0.0"));
			Assert.IsTrue(new NumberValidator(17, 2, true).IsValidNumber("0"));
			Assert.IsTrue(new NumberValidator(17, 2, true).IsValidNumber("0.0"));  // повтор
			Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("00.00"));
			Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("-0.00"));
			Assert.IsTrue(new NumberValidator(17, 2, true).IsValidNumber("0.0"));  // повтор
			Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("+0.00"));
			Assert.IsTrue(new NumberValidator(4, 2, true).IsValidNumber("+1.23"));
			Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("+1.23"));
			Assert.IsFalse(new NumberValidator(17, 2, true).IsValidNumber("0.000"));
			Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("-1.23"));
			Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("a.sd"));*/
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