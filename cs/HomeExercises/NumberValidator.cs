using System;
using System.Text.RegularExpressions;

namespace HomeExercises
{
    public class NumberValidator
    {
        private static readonly Regex numberRegex;
        private readonly bool onlyPositive;
        private readonly int precision; // Максимальное количество цифр
        private readonly int scale; // Максимальное количество цифр в дробной части

        static NumberValidator()
        {
            // Для валидности ".2" пришлось поправить паттерн \d* вместо \d+ в группе 2
            numberRegex = new Regex(@"^([+-]?)(\d*)([.,](\d+))?$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }

        /// <summary>
        /// Создает объект NumberValidator
        /// </summary>
        /// <param name="precision">Максимальное количество цифр</param>
        /// <param name="scale">Максимальное количество цифр в дробной части</param>
        /// <param name="onlyPositive"></param>
        public NumberValidator(int precision, int scale = 0, bool onlyPositive = false)
        {
            this.precision = precision;
            this.scale = scale;
            this.onlyPositive = onlyPositive;
            if (precision <= 0)
                throw new ArgumentException("precision must be a positive number");
            if (scale < 0 || scale > precision)
                throw new ArgumentException("scale must be a non-negative number less or equal than precision");            
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