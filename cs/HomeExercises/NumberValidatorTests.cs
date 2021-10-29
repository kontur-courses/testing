using FluentAssertions;
using NUnit.Framework;
using System;
using System.Text.RegularExpressions;

namespace HomeExercises
{
    [TestFixture]
    public class ValidatorShould
    {
        [Test]
        public void DoEverything()
        {
            Assert.Throws<ArgumentException>(() => new NumberValidator(-1, 2, true));
            Assert.DoesNotThrow(() => new NumberValidator(1, 0, true));
            Assert.Throws<ArgumentException>(() => new NumberValidator(-1, 2, false));//повтор
            Assert.DoesNotThrow(() => new NumberValidator(1, 0, true));//повтор

            Assert.IsTrue(new NumberValidator(17, 2, true).IsValidNumber("0.0"));
            Assert.IsTrue(new NumberValidator(17, 2, true).IsValidNumber("0"));
            Assert.IsTrue(new NumberValidator(17, 2, true).IsValidNumber("0.0"));//повтор
            Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("00.00"));
            Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("-0.00"));//проверяет одновременно onlyPositive и precision
            Assert.IsTrue(new NumberValidator(17, 2, true).IsValidNumber("0.0"));//повтор
            Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("+0.00"));//повтор теста precision
            Assert.IsTrue(new NumberValidator(4, 2, true).IsValidNumber("+1.23"));
            Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("+1.23"));//повтор теста precision
            Assert.IsFalse(new NumberValidator(17, 2, true).IsValidNumber("0.000"));
            Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("-1.23"));//проверяет одновременно onlyPositive и precision, повтор
            Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("a.sd"));
            //не хватает тестов на некорректный формат, на scale, на пустую (или null) строку
        }

        [TestCase(-1, 0, "negative precision (\"{0}\") was provided")]
        [TestCase(3, -1, "negative scale (\"{1}\") was provided")]
        [TestCase(1, 2, "scale (\"{1}\") was bigger than precision (\"{0}\")")]
        public void Throw_WhenConstructorArgsAreIncorrect(int precision, int scale, string because)
        {
            Action action = () => new NumberValidator(precision, scale);

            action.Should().Throw<ArgumentException>(because, precision, scale);
        }

        [TestCase(2)]
        [TestCase(2, 1)]
        [TestCase(2, 1, true)]
        public void NotThrow_WhenConstructorArgsAreCorrect(int precision, int scale = 0, bool onlyPositive = false)
        {
            Action action = () => new NumberValidator(precision, scale, onlyPositive);

            action.Should().NotThrow();
        }

        [TestCase("-1.23", true, "a negative number (\"{0}\") was provided when onlyPositive was (\"{3}\")")]
        [TestCase("1.2300", false, "a number with big frac part (\"{0}\") was provided when scale was ({2})")]
        [TestCase("+1222.45", false, "a number with big total length (\"{0}\") was provided when scale was ({2}) and precision was ({1})")]
        public void ReturnFalse_WhenValueIsInvalidNumber(string value, bool onlyPositive, string because)
        {
            var precision = 5;
            var scale = 2;
            var validator = new NumberValidator(precision, scale, onlyPositive);

            var result = validator.IsValidNumber(value);

            result.Should().BeFalse(because, value, precision, scale, onlyPositive);
        }

        [Test]
        public void ReturnFalse_WhenValueIsNull()
        {
            string? value = null;
            var validator = new NumberValidator(1);

            var result = validator.IsValidNumber(value!);

            result.Should().BeFalse("null was provided");
        }

        [Test]
        public void ReturnFalse_WhenValueIsEmpty()
        {
            var arg = "";
            var validator = new NumberValidator(1);

            var result = validator.IsValidNumber(arg);

            result.Should().BeFalse("an empty number (\"{0}\") was provided", arg);
        }

        [TestCase("a.bc", "a non-number (\"{0}\") was provided")]
        [TestCase("+-0.0", "a number with several signs (\"{0}\") was provided")]
        [TestCase("+0.0.0", "a number with several dots (\"{0}\") was provided")]
        [TestCase("0+0.0", "a number with sing inside (\"{0}\") was provided")]
        [TestCase(".0", "a number without starting digit (\"{0}\") was provided")]
        [TestCase("0.", "a number without end digit (\"{0}\") was provided")]
        [TestCase(".", "a number without digits (\"{0}\") was provided")]
        [TestCase(" ", "a number with only white spaces (\"{0}\") was provided")]
        public void ReturnFalse_WhenValueIsInWrongFormat(string value, string because)
        {
            var validator = new NumberValidator(10, 5);

            var result = validator.IsValidNumber(value);

            result.Should().BeFalse(because, value);
        }

        [TestCase("0", 1, 0, true)]
        [TestCase("-0", 2, 0, false)]
        [TestCase("01", 2, 0, true)]
        [TestCase("+1", 2, 0, true)]
        [TestCase("12", 2, 0, true)]
        [TestCase("12.3", 3, 1, true)]
        [TestCase("0.12", 3, 2, true)]
        [TestCase("12.34", 4, 2, true)]
        [TestCase("-12.34", 5, 2, false)]
        public void ReturnTrue_WhenValueIsCorrectNumber(string value, int precision, int scale, bool onlyPositive)
        {
            var validator = new NumberValidator(precision, scale, onlyPositive);

            var result = validator.IsValidNumber(value);

            result.Should().BeTrue("a number with correct format (\"{0}\") for Validator with settings ({1}, {2}, {3}) was provided", value, precision, scale, onlyPositive);
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
                throw new ArgumentException("scale must be a non-negative number less or equal than precision");

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