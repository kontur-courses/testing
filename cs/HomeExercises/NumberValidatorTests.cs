using FluentAssertions;
using NUnit.Framework;
using System;
using System.Text.RegularExpressions;

namespace HomeExercises
{
    public class NumberValidatorTests
    {
        [Test]
        public void Test()
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

        private NumberValidator _smallValidator = null!;
        private NumberValidator _bigValidator = null!;
        private NumberValidator _negativeValidator = null!;

        [SetUp]
        public void Setup()
        {
            _smallValidator = new NumberValidator(6, 2, true);
            _bigValidator = new NumberValidator(17, 8, true);
            _negativeValidator = new NumberValidator(17, 8, false);
        }

        [Test]
        public void Validator_Throws_WhenNonPositivePrecision()
        {
            Action action = () => new NumberValidator(-1);
            action.Should().Throw<ArgumentException>();
        }

        [Test]
        public void Validator_Throws_WhenNegativeScale()
        {
            Action action = () => new NumberValidator(3, -1);
            action.Should().Throw<ArgumentException>();
        }

        [Test]
        public void Validator_Throws_WhenScaleLargerThenPrecision()
        {
            Action action = () => new NumberValidator(3, 5);
            action.Should().Throw<ArgumentException>();
        }

        [Test]
        public void Validator_NotThrows_WhenCorrectArgs()
        {
            Action action = () => new NumberValidator(2);
            action.Should().NotThrow();
        }

        [Test]
        public void Validator_False_WhenNotNumber()
        {
            var arg = "a.bc";
            _smallValidator.IsValidNumber(arg).Should().BeFalse("a non-number (\"{0}\") was provided", arg);
        }

        [Test]
        public void Validator_False_WhenOnlyPositiveWithNegativeNumber()
        {
            var arg = "-1.23";
            _bigValidator.IsValidNumber(arg).Should().BeFalse("a negative number (\"{0}\") was provided", arg);
        }

        [Test]
        public void Validator_False_WhenFracPartToBig()
        {
            var arg = "1.2300";
            _smallValidator.IsValidNumber(arg).Should().BeFalse("a number with big frac part (\"{0}\") was provided", arg);
        }

        [Test]
        public void Validator_False_WhenTotalLengthToBig()
        {
            var arg = "+1222.45";
            _smallValidator.IsValidNumber(arg).Should().BeFalse("a number with big total length (\"{0}\") was provided", arg);
        }

        [Test]
        public void Validator_False_WhenNullOrEmpty()
        {
            var arg = "";
            _smallValidator.IsValidNumber(arg).Should().BeFalse("an empty number (\"{0}\") was provided", arg);

            arg = null;
            _smallValidator.IsValidNumber(arg!).Should().BeFalse("null was provided");
        }

        [Test]
        public void Validator_False_WhenWrongFormat()
        {
            var arg = "+-0.0";
            _smallValidator.IsValidNumber(arg).Should().BeFalse("a number with several signs (\"{0}\") was provided", arg);

            arg = "+0.0.0";
            _smallValidator.IsValidNumber(arg).Should().BeFalse("a number with several dots (\"{0}\") was provided", arg);

            arg = "0+0.0";
            _smallValidator.IsValidNumber(arg).Should().BeFalse("a number with sing inside (\"{0}\") was provided", arg);

            arg = ".0";
            _smallValidator.IsValidNumber(arg).Should().BeFalse("a number without starting digit (\"{0}\") was provided", arg);

            arg = "0.";
            _smallValidator.IsValidNumber(arg).Should().BeFalse("a number without end digit (\"{0}\") was provided", arg);

            arg = ".";
            _smallValidator.IsValidNumber(arg).Should().BeFalse("a number without digits (\"{0}\") was provided", arg);
        }

        [Test]
        public void Validator_True_OnCorrectNumber()
        {
            var arg = "+1.23";
            _smallValidator.IsValidNumber(arg).Should().BeTrue("a number with correct format (\"{0}\") was provided", arg);

            arg = "2";
            _smallValidator.IsValidNumber(arg).Should().BeTrue("a number with correct format (\"{0}\") was provided", arg);

            arg = "+0.12";
            _smallValidator.IsValidNumber(arg).Should().BeTrue("a number with correct format (\"{0}\") was provided", arg);

            arg = "0.12";
            _smallValidator.IsValidNumber(arg).Should().BeTrue("a number with correct format (\"{0}\") was provided", arg);

            arg = "12.34";
            _smallValidator.IsValidNumber(arg).Should().BeTrue("a number with correct format (\"{0}\") was provided", arg);

            arg = "+12.34";
            _smallValidator.IsValidNumber(arg).Should().BeTrue("a number with correct format (\"{0}\") was provided", arg);

            arg = "-0.123";
            _negativeValidator.IsValidNumber(arg).Should().BeTrue("a number with correct format (\"{0}\") was provided", arg);
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
            {
                throw new ArgumentException("precision must be a positive number");
            }

            if (scale < 0 || scale >= precision)
            {
                throw new ArgumentException("precision must be a non-negative number less or equal than precision");
            }

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
            {
                return false;
            }

            Match? match = numberRegex.Match(value);
            if (!match.Success)
            {
                return false;
            }

            // Знак и целая часть
            var intPart = match.Groups[1].Value.Length + match.Groups[2].Value.Length;
            // Дробная часть
            var fracPart = match.Groups[4].Value.Length;

            if (intPart + fracPart > precision || fracPart > scale)
            {
                return false;
            }

            if (onlyPositive && match.Groups[1].Value == "-")
            {
                return false;
            }

            return true;
        }
    }
}