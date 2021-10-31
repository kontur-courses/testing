using System;
using System.Text.RegularExpressions;

namespace HomeExercises.NumberValidatorTask
{
	public class NumberValidator
	{
		private readonly Regex numberRegex;
		private readonly bool onlyPositive;
		private readonly Regex plusMinusZeroRegex;
		private readonly int precision;
		private readonly int scale;
		private readonly Regex zeroAtHighestDigitRegex;

		public NumberValidator(int precision, int scale = 0, bool onlyPositive = false)
		{
			if (precision <= 0)
				throw new ArgumentException("precision must be a positive number");
			if (scale < 0 || scale >= precision)
				throw new ArgumentException("precision must be a non-negative number less or equal than precision");

			this.precision = precision;
			this.scale = scale;
			this.onlyPositive = onlyPositive;

			numberRegex = new Regex(@"^([+-]?)(\d+)([.,](\d+))?$", RegexOptions.IgnoreCase);
			zeroAtHighestDigitRegex = new Regex(@"^([+-])?(0+\d)");
			plusMinusZeroRegex = new Regex(@"^([+-]{1})(0{1})(([.,]{1})(0*)$|$)");
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

			value = value.Trim();

			if (plusMinusZeroRegex.IsMatch(value))
				return false;

			if (zeroAtHighestDigitRegex.IsMatch(value))
				return false;

			var match = numberRegex.Match(value);
			if (!match.Success)
				return false;

			var intPart = GetIntPartLength(match);
			var fracPart = match.Groups[4].Value.Length;

			if (intPart + fracPart > precision || fracPart > scale)
				return false;

			if (onlyPositive && match.Groups[1].Value == "-")
				return false;
			return true;
		}

		private int GetIntPartLength(Match match)
		{
			var plusSign = "+";
			var intPart = match.Groups[1].Value.Length + match.Groups[2].Value.Length;
			intPart = match.Groups[1].Value == plusSign ? intPart - 1 : intPart;

			return intPart;
		}
	}
}