using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using FluentAssertions.Execution;
using NUnit.Framework;

namespace HomeExercises
{
    public class NumberValidatorTests
    {
        [Test]
        public void Test()
        {
            var exceptionDescriptionFormat = "for {0} number validator {1}";
            Action makingPrecisionNonPositive = () => new NumberValidator(-1, 1);
            Action makingScaleNegative = () => new NumberValidator(1, -1);
            Action makingScaleGreaterThanPrecision = () => new NumberValidator(1, 2);
            Action makingScaleZero = () => new NumberValidator(1, 0);

            var descriptionFormat = "{2} (on {0} validator for \"{1}\")";
            var simpleNumberValidator = new NumberValidator(3, 2, false);
            var integerOnlyNumberValidator = new NumberValidator(3, 0, false);
            var positiveOnlyNumberValidator = new NumberValidator(3, 2, true);
            using (new AssertionScope())
            {
                makingPrecisionNonPositive.ShouldThrow<ArgumentException>(
                    exceptionDescriptionFormat, "N(-1,1)",
                    "non-positive precision doesn't make sense");
                makingScaleNegative.ShouldThrow<ArgumentException>(
                    exceptionDescriptionFormat, "N(1,-1)",
                    "negative fraction part length doesn't make sense");
                makingScaleGreaterThanPrecision.ShouldThrow<ArgumentException>(
                    exceptionDescriptionFormat, "N(1,2)",
                    "length of fraction part can not be greater than total number length");
                makingScaleZero.ShouldNotThrow<ArgumentException>(
                    exceptionDescriptionFormat, "N(1,0)",
                    "some numbers can have zero-length fraction part (i.e. integers)");

                simpleNumberValidator.IsValidNumber("0").Should().BeTrue(
                    descriptionFormat, "N(3,2)", "0",
                    "it's a valid number");
                simpleNumberValidator.IsValidNumber("0.0").Should().BeTrue(
                    descriptionFormat, "N(3,2)", "0.0",
                    "it's a valid number");
                simpleNumberValidator.IsValidNumber("0,0").Should().BeTrue(
                    descriptionFormat, "N(3,2)", "0,0",
                    "fraction can also be denoted as ,");
                simpleNumberValidator.IsValidNumber("00.00").Should().BeFalse(
                    descriptionFormat, "N(3,2)", "00.00",
                    "number string length must be less or equal to validator precision");
                simpleNumberValidator.IsValidNumber("0.000").Should().BeFalse(
                    descriptionFormat, "N(3,2)", "0.000",
                    "fraction part length must be less or equal to validator fraction part precision");
                simpleNumberValidator.IsValidNumber("-0.00").Should().BeFalse(
                    descriptionFormat, "N(3,2)", "-0.00",
                    "minus sign must be accounted for when checking number precision");

                /* хоть в комментариях к методу IsValidNumber сказано, что знак должен влиять на
                 * длину числа лишь для отрицательных чисел,  
                 * но в оригинальных тестах данная проверка должна была выдавать False             
                 */
                simpleNumberValidator.IsValidNumber("+0.00").Should().BeFalse(
                    descriptionFormat, "N(3,2)", "+0.00",
                    "plus sign must not be accounted for when checking number precision");
                simpleNumberValidator.IsValidNumber("a.sd").Should().BeFalse(
                    descriptionFormat, "N(3,2)", "a.sd",
                    "number string must not consist of letters");
                simpleNumberValidator.IsValidNumber(" 0.0").Should().BeFalse(
                    descriptionFormat, "N(3,2)", " 0.0",
                    "number strings must not start with whitespace(s) or any other characters");
                simpleNumberValidator.IsValidNumber("0.0 ").Should().BeFalse(
                    descriptionFormat, "N(3,2)", "0.0 ",
                    "number strings must not end with whitespace(s) or any other characters");
                simpleNumberValidator.IsValidNumber(".1").Should().BeFalse(
                    descriptionFormat, "N(3,2)", ".1",
                    "integer part must be present");
                simpleNumberValidator.IsValidNumber("0.").Should().BeFalse(
                    descriptionFormat, "N(3,2)", "0.",
                    "fraction part must be present if there's a dot or comma");

                integerOnlyNumberValidator.IsValidNumber("0.1").Should().BeFalse(
                    descriptionFormat, "N(3)", "0.1",
                    "number with fraction part should not be validated");

                positiveOnlyNumberValidator.IsValidNumber("-1").Should().BeFalse(
                    descriptionFormat, "positive-only N(3,2)", "-1",
                    "negative number should not be validated");
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