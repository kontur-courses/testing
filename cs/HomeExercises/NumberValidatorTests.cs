using System;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace HomeExercises
{
    public class NumberValidatorTests
    {
        [Test]
        public void NumberValidatorConstructorMultipleTest()
        {
            Assert.Multiple(() =>
            {
                Assert.Throws<ArgumentException>(() => new NumberValidator(-1, 2, true),
                    "precision should be positive");

                Assert.Throws<ArgumentException>(() => new NumberValidator(2, -1), "scale should be positive");
                Assert.Throws<ArgumentException>(() => new NumberValidator(2, 3, true),
                    "scale should be less than precision");

                Assert.Throws<ArgumentException>(() => new NumberValidator(2, 2, true),
                    "scale couldn't be same as precision");


                Assert.DoesNotThrow(() => new NumberValidator(1, 0, true), "scale could be 0");
                Assert.DoesNotThrow(() => new NumberValidator(1, 0), "scale could be 0");
            });
        }

        [Test]
        public void NumberValidationMultiTest()
        {
            Assert.Multiple(() =>
            {
                Assert.IsTrue(new NumberValidator(4, 2, true).IsValidNumber("+1.23"),
                    "correct result for insignificant sign"); //насчет "звучит как "исправить результат""
                //google translate переводит как "правильный результат для незначащего знака", что и задумывалось
                //не учитывая корявость машинного перевода


                var validator = new NumberValidator(17, 2, true);

                Assert.IsFalse(validator.IsValidNumber(string.Empty), "empty string is invalid number");
                Assert.IsFalse(validator.IsValidNumber("     "), "string with spaces is invalid number");
                Assert.True(validator.IsValidNumber("0"), "number may not have a fractional part");
                Assert.IsFalse(validator.IsValidNumber("0.000"),
                    "number of fractional part digits should be less than scale");

                Assert.IsTrue(validator.IsValidNumber("0.0"), "the number can have an insignificant fractional part");
                Assert.IsTrue(validator.IsValidNumber("1"), "should accept positive numbers");
                Assert.IsTrue(new NumberValidator(4, 1, false).IsValidNumber("1"), "should accept positive numbers");
                Assert.IsFalse(validator.IsValidNumber("-1"),
                    "should deny negative numbers, if valid number should be positive");
                Assert.IsFalse(validator.IsValidNumber(".1"), "before . should be at least 1 digit");
                Assert.IsFalse(validator.IsValidNumber("1."), "after . should be at least 1 digit");
                Assert.IsTrue(validator.IsValidNumber("1,2"), "should work with comma");

                validator = new NumberValidator(3, 2, true);


                Assert.IsFalse(validator.IsValidNumber("+0.00"),
                    "number invalid if number of digits with sign more than precision");

                Assert.IsFalse(validator.IsValidNumber("-0.00"),
                    "number invalid if number of digits with sign more than precision");

                Assert.IsFalse(validator.IsValidNumber("00.00"), "number of digits should be less than precision");
                Assert.IsFalse(validator.IsValidNumber("-a.sd"), "number should contain digits");
                Assert.IsFalse(validator.IsValidNumber("1.a"), "number should not contain letters");
                Assert.IsFalse(validator.IsValidNumber(" 1"), "number should not contain spaces in the beginning");
                Assert.IsFalse(validator.IsValidNumber("1 "), "number must not contain trailing spaces");
                Assert.IsFalse(validator.IsValidNumber("-+1"), "number could have only one sign");
            });
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