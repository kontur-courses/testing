using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
    [TestFixture]
    public class NumberValidatorTests
    {
        public class TestsBase
        {
            [OneTimeSetUp]
            public void BaseOneTimeSetUp()
            {
                basicValidator = CreateBasicNumberValidator();
            }

            private NumberValidator CreateBasicNumberValidator()
            {
                return new NumberValidator(10, 2);
            }

            public NumberValidator basicValidator;
        }

        public class ConstructorShould : TestsBase
        {
            private static Action[] precisionCases =
            {
                () => { var x = new NumberValidator(0); },
                () => { var x = new NumberValidator(-1); }
            };

            [TestCaseSource("precisionCases")]
            public void ThrowArgumentException_OnZeroOrNegativePrecision(Action create)
            {
                create.Should().Throw<ArgumentException>()
                    .WithMessage("precision must be a positive number");
            }

            private static Action[] scaleCases =
            {
                () => { var x = new NumberValidator(1, 1); },
                () => { var x = new NumberValidator(1, 2); },
                () => { var x = new NumberValidator(1, -2); },
            };

            [TestCaseSource("scaleCases")]
            public void ThrowArgumentException_OnNegativeScaleOrScaleBiggerEqualPrecision(Action create)
            {
                create.Should().Throw<ArgumentException>()
                    .WithMessage("scale must be a non-negative number less or equal than precision");
            }

            [Test]
            public void NotThrowException_OnCorrectParams()
            {
                Action create = () => { var x = new NumberValidator(10, 2); };
                create.Should().NotThrow();
            }
        }

        public class IsValidNumberShould : TestsBase
        {
            [Test]
            public void ReturnFalse_OnNull()
            {
                basicValidator.IsValidNumber(null).Should().BeFalse();
            }

            [Test]
            public void ReturnFalse_OnEmpty()
            {
                basicValidator.IsValidNumber("").Should().BeFalse();
            }


            private static string[] incorrectNumberFormatCases =
                { "asd", ".11", "9.", "10,", "12.1e", "-a.89", "12. 4", "+\n", "6-", "++" };

            [TestCaseSource("incorrectNumberFormatCases")]
            public void ReturnFalse_OnIncorrectNumberFormat(string number)
            {
                basicValidator.IsValidNumber(number).Should().BeFalse();
            }

            private static string[] precisionLessThanNumberLengthCases = { "21.3", "+10", "100" };

            [TestCaseSource("precisionLessThanNumberLengthCases")]
            public void ReturnFalse_OnPrecisionLessThanNumberLength(string number)
            {
                var validator = new NumberValidator(2, 1);
                validator.IsValidNumber(number).Should().BeFalse();
            }

            [Test]
            public void ReturnFalse_OnFracPartBiggerThanScale()
            {
                new NumberValidator(4, 2).IsValidNumber("1.444").Should().BeFalse();
            }

            [Test]
            public void ReturnFalse_OnNegativeNumberWhenValidatorOnlyPositive()
            {
                new NumberValidator(10, 2, true).IsValidNumber("-10").Should().BeFalse();
            }

            private static string[] correctNumbersCases =
                { "55", "0.00", "10.13", "10,13", "-20", "+20", "-177.6", "+46,68", "1000000000" };

            [TestCaseSource("correctNumbersCases")]
            public void ReturnTrue_OnCorrectNumbers(string number)
            {
                basicValidator.IsValidNumber(number).Should().BeTrue();
            }
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