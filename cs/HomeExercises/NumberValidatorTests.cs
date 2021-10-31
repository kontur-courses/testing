using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using FluentAssertions.Execution;
using NUnit.Framework;

namespace HomeExercises
{
	public class PlugException : Exception{}
	
	public class NumberValidatorTests
	{
		// 1 - пустая строка или null
		[TestCase(null, false)]
		[TestCase("", false)]
		// [TestCase(" ", false)]
		
		// 2 - проверка на регур
		// [TestCase("a.sd", false)]
		[TestCase("1.1d", false)]
		[TestCase("d.1", false)]
		[TestCase("d1", false)]
		
		[TestCase("1.1.1", false)]
		[TestCase(".1", false)]
		[TestCase("1.", false)]
		[TestCase(".", false)]
		
		[TestCase("--1.0", false, 17, 2, false)]
		[TestCase("+-1.0", false, 17, 2, false)]
		[TestCase("1+1.0", false, 17, 2, false)]
		[TestCase("1.0+", false)]
		
		// 3 // Добавить сюда пограничные случаи
		// проварка на общую длину
		[TestCase("-12.23", true, 5, 2, false)] 
		[TestCase("-321.23", false, 5, 2, false)] 
		[TestCase("21.23", true, 5, 2, false)] 
		
		// проверка на длину дробной части
		[TestCase("+1.234", false, 55, 2, false)]
		[TestCase("+1.234", true, 55, 3, false)]
		[TestCase("+1.23", true, 55, 2, false)]
		
		// 4 - проверка на знак
		[TestCase("-2.1", false, 10, 5, true)]
		[TestCase("+2.1", true, 10, 5, true)]
		[TestCase("-2.1", true, 10, 5, false)]
		[TestCase("+2.1", true, 10, 5, false)]
		
		
		// [TestCase("+0.00", true, 3)]// падает специально
		public void Test_IsValidNumber(
			string numberForCheck,
			bool expected, 
			int precision = 17, 
			int scale = 2, 
			bool onlyPositive = true)
		{
			// if (throwException is null)
			// {
			// 	Assert.DoesNotThrow(() => new NumberValidator(precision, scale, onlyPositive));
			// }
			// else
			// {
			// 	Assert.Throws<T>(() => new NumberValidator(precision, scale, onlyPositive));
			// }

			var validatorResult = new NumberValidator(precision, scale, onlyPositive).IsValidNumber(numberForCheck);
			var message = $"Number {numberForCheck} with parameters: " +
			              $"\n\tprecision = {precision}, " +
			              $"\n\tscale = {scale}, " +
			              $"\n\tonlyPositive = {onlyPositive}" +
			              $"\n\n  IsValidNumber result";
			if (expected)
			{
				Assert.That(validatorResult, Is.True, message);
				Assert.That(validatorResult, Is.True, $"Exception on double call\n{message}");
				
				validatorResult = new NumberValidator(precision, scale, onlyPositive).IsValidNumber(numberForCheck);
				Assert.That(validatorResult, Is.True, $"Exception on double NumberValidator constructor call\n{message}");
			}
			else
			{
				Assert.That(validatorResult, Is.False, message);
				Assert.That(validatorResult, Is.False, $"Exception on double call\n{message}");
				
				validatorResult = new NumberValidator(precision, scale, onlyPositive).IsValidNumber(numberForCheck);
				Assert.That(validatorResult, Is.False, $"Exception on double NumberValidator constructor call\n{message}");
			}
		}

		[TestCase(-1, 2)]
		[TestCase(0, 2)]
		[TestCase(2, -1)]
		[TestCase(2, 2)]
		[TestCase(2, 3)]
		public void NumberValidatorConstructor_ThrowArgumentException_IncorrectPrecisionOrScale(int precision, int scale)
		{
			var message = $"NumberValidator constructor with parameters: " +
			              $"\n\tprecision = {precision}, " +
			              $"\n\tscale = {scale}, ";

			using (new AssertionScope())
			{
				Action action1 = () => new NumberValidator(precision, scale);
				Assert.Throws<ArgumentException>(() => action1(), message + $"\n\tonlyPositive = {false}");
				
				Action action2 = () => new NumberValidator(precision, scale, true);
				Assert.Throws<ArgumentException>(() => action2(), message + $"\n\tonlyPositive = {true}");
			}
		}
		
		[TestCase(1, 0)]
		[TestCase(2, 0)]
		[TestCase(2, 1)]
		public void NumberValidatorConstructor_DoesNotThrow_CorrectPrecisionAndScale(int precision, int scale)
		{
			Action action1 = () => new NumberValidator(precision, scale);
			action1.Should().NotThrow();
			action1.Should().NotThrow();
			
			Action action2 = () => new NumberValidator(precision, scale, true);
			action2.Should().NotThrow();
			action2.Should().NotThrow();
		}
		
		// [Test]
		// public void Test_NumberValidatorConstructor()
		// {
		// 	Assert.Throws<ArgumentException>(() => new NumberValidator(-1, 2, true));
		// 	Assert.DoesNotThrow(() => new NumberValidator(1, 0, true));
		// 	Assert.Throws<ArgumentException>(() => new NumberValidator(-1, 2, false));
		// 	Assert.DoesNotThrow(() => new NumberValidator(1, 0, true));
		//
		// 	var p = -1;
		// 	var s = 2;
		// 	var op = true;
		// 	Action action = () => new NumberValidator(p, s, op);
		// 	action.Should().Throw<ArgumentException>();
		// 	
		// 	p = 1;
		// 	s = 0;
		// 	op = true;
		// 	action.Should().NotThrow();
		// 	
		// 	p = -1;
		// 	s = 2;
		// 	op = false;
		// 	action.Should().Throw<ArgumentException>();
		// 	
		// 	p = 1;
		// 	s = 0;
		// 	op = false;
		// 	action.Should().NotThrow();
		// }
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
				throw new ArgumentException("precision must be a non-negative number less or equal than precision"); //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
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