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

        [TestCase(-1, 0, TestName = "Throw then precision was non-positive")]
        [TestCase(3, -1, TestName = "Throw then scale was negative")]
        [TestCase(1, 2, TestName = "Throw then scale was non-negative")]
        public void Throw_WhenConstructorArgsAreIncorrect(int precision, int scale)
        {
            Action action = () => new NumberValidator(precision, scale);

            action.Should().Throw<ArgumentException>();
        }

        [TestCase(2)]
        [TestCase(2, 1)]
        [TestCase(2, 1, true)]
        public void NotThrow_WhenConstructorArgsAreCorrect(int precision, int scale = 0, bool onlyPositive = false)
        {
            Action action = () => new NumberValidator(precision, scale, onlyPositive);

            action.Should().NotThrow();
        }

        [TestCase("-1.23", true, TestName = "Return false then value is negative number whereas onlyPositive was 'true'")]
        [TestCase("1.2300", false, TestName = "Return false then value has fractional part bigger than validator's scale")]
        [TestCase("+1222.45", false, TestName = "Return false then value has length bigger than validator's precision")]
        public void ReturnFalse_WhenValueIsInvalidNumber(string value, bool onlyPositive)
        {
            var validator = new NumberValidator(5, 2, onlyPositive);

            var result = validator.IsValidNumber(value);

            result.Should().BeFalse();
        }

        [Test]
        public void ReturnFalse_WhenValueIsNull()
        {
            var validator = new NumberValidator(1);

            var result = validator.IsValidNumber(null!);

            result.Should().BeFalse("null was provided");
        }

        [Test]
        public void ReturnFalse_WhenValueIsEmpty()
        {
            var validator = new NumberValidator(1);

            var result = validator.IsValidNumber(string.Empty);

            result.Should().BeFalse();
        }

        [TestCase("a.bc", TestName = "Return false then a non-number was provided")]
        [TestCase("+-0.0", TestName = "Return false then number with several signs was provided")]
        [TestCase("+0.0.0", TestName = "Return false then number with several dots was provided")]
        [TestCase("0+0.0", TestName = "Return false then number with a sing inside was provided")]
        [TestCase("0 0.0", TestName = "Return false then number with space inside was provided")]
        [TestCase(".0", TestName = "Return false then number without starting digit was provided")]
        [TestCase("0.", TestName = "Return false then number without end digit was provided")]
        [TestCase(".", TestName = "Return false then number without digits was provided")]
        [TestCase(" ", TestName = "Return false then number with only white spaces was provided")]
        public void ReturnFalse_WhenValueIsInWrongFormat(string value)
        {
            var validator = new NumberValidator(10, 5);

            var result = validator.IsValidNumber(value);

            result.Should().BeFalse();
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