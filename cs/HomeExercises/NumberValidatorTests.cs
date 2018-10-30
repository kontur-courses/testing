using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
    [TestFixture]
    public class NumberValidatorTests
    {
        [TestFixture]
        public class Constructor_Should
        {
            [Test]
            public void ThrowsArgumentException_WhenPrecisionIsNegative()
            {
                Action constructor = () => new NumberValidator(-1);

                constructor.Should().Throw<ArgumentException>()
                    .WithMessage("precision must be a positive number");
            }

            [Test]
            public void ThrowsArgumentException_WhenPrecisionIsZero()
            {
                Action constructor = () => new NumberValidator(0);

                constructor.Should().Throw<ArgumentException>()
                    .WithMessage("precision must be a positive number");
            }

            [Test]
            public void ThrowsArgumentException_WhenScaleIsNegative()
            {
                Action constructor = () => new NumberValidator(1, -1);

                constructor.Should().Throw<ArgumentException>()
                    .WithMessage("scale must be a non-negative number less or equal than precision");
            }

            [Test]
            public void ThrowsArgumentException_WhenScaleIsGreaterPrecision()
            {
                Action constructor = () => new NumberValidator(1, 2);

                constructor.Should().Throw<ArgumentException>()
                    .WithMessage("scale must be a non-negative number less or equal than precision");
            }

            [Test]
            public void ThrowsArgumentException_WhenScaleIsEqualToPrecision()
            {
                Action constructor = () => new NumberValidator(1, 1);

                constructor.Should().Throw<ArgumentException>()
                    .WithMessage("scale must be a non-negative number less or equal than precision");
            }

            [Test]
            public void DoesNotThrowException_WhenPrecisionIsPositiveAndScaleIsNonNegativeAndLessThanPrecision()
            {
                Action constructor = () => new NumberValidator(1);

                constructor.Should().NotThrow<Exception>();
            }
        }

        [TestFixture]
        public class Method_IsValidNumber_Should
        {
            [Test]
            public void ReturnFalse_WhenInputIsEmpty()
            {
                var validator = new NumberValidator(6, 2);

                validator.IsValidNumber(" ").Should().BeFalse();
            }

            [Test]
            public void ReturnFalse_WhenInputIsNull()
            {
                var validator = new NumberValidator(6, 2);

                validator.IsValidNumber(null).Should().BeFalse();
            }

            [Test]
            public void ReturnFalse_WhenFractPartIsGreaterThanScale()
            {
                var validator = new NumberValidator(6, 2);

                validator.IsValidNumber("1.002").Should().BeFalse();
            }

            [Test]
            public void ReturnFalse_WhenIntPartIsGreaterThanPrecision()
            {
                var validator = new NumberValidator(6, 2);

                validator.IsValidNumber("1234567").Should().BeFalse();
            }

            [Test]
            public void ReturnTrue_WhenIntPartLessThanPrecision()
            {
                var validator = new NumberValidator(6, 2);

                validator.IsValidNumber("-0").Should().BeTrue();
            }

            [Test]
            public void ReturnTrue_WhenIntPartEqualToPrecision()
            {
                var validator = new NumberValidator(6, 2);

                validator.IsValidNumber("-12345").Should().BeTrue();
            }

            [Test]
            public void ReturnTrue_WhenFractPartIsZero()
            {
                var validator = new NumberValidator(6, 2);

                validator.IsValidNumber("0").Should().BeTrue();
            }

            [Test]
            public void ReturnTrue_WhenFractPartLessThanScale()
            {
                var validator = new NumberValidator(6, 2);

                validator.IsValidNumber("-0.0").Should().BeTrue();
            }

            [Test]
            public void ReturnTrue_WhenFractPartEqualToScale()
            {
                var validator = new NumberValidator(6, 2);

                validator.IsValidNumber("0.00").Should().BeTrue();
            }

            [Test]
            public void ReturnFalse_WhenInputIsNotNumber()
            {
                var validator = new NumberValidator(6, 2);

                validator.IsValidNumber("-2.-1").Should().BeFalse();
            }

            [TestFixture]
            public class WhenValidatorIsOnlyForPositive
            {
                [Test]
                public void ReturnFalse_WhenInputIsNegative()
                {
                    var validator = new NumberValidator(6, 0, true);

                    validator.IsValidNumber("-1").Should().BeFalse();
                }
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