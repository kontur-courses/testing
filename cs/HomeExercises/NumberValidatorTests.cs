using FluentAssertions;
using NUnit.Framework;
using System;
using System.Text.RegularExpressions;

namespace HomeExercises
{
    [Category("NumberValidator creation")]
    [TestFixture]
    public class NumberValidatorCreation_Should
    {
        [TestCase(5, -3, "scale must be a non-negative number less than precision", TestName = "scale is negative")]
        [TestCase(0, -1, "precision must be a positive number", TestName = "precision is non positive")]
        [TestCase(5, 7, "scale must be a non-negative number less than precision", TestName =
            "scale is bigger than precision")]
        public void ThrowArgumentException_When(int precision, int scale, string message)
        {
            // ReSharper disable once ObjectCreationAsStatement
            Action creation = () => new NumberValidator(precision, scale);
            creation.Should()
                    .Throw<ArgumentException>()
                    .WithMessage(message);
        }
    }

    [Category("NumberValidator::IsValidNumber")]
    [TestFixture]
    public class NumberValidator_Should
    {
        [Category("Obvious cases")]
        [TestCase(null, TestName = "is null")]
        [TestCase("", TestName = "is empty")]
        [TestCase("   \t  ", TestName = "contains only whitespace characters")]
        [TestCase("a7,3e", TestName = "contains any letter")]
        [TestCase("1..1", TestName = "contains several dots")]
        [TestCase("1,,1", TestName = "contains several commas ")]
        [TestCase("++1", TestName = "contains several \"+\"")]
        [TestCase("--1", TestName = "contains several \"-\"")]
        [TestCase(",1", TestName = "starts with comma")]
        [TestCase(".1", TestName = "starts with dot")]
        [TestCase("1,", TestName = "ends with comma")]
        [TestCase("1.", TestName = "ends with dot")]
        [TestCase("-", TestName = "is minus sign")]
        [TestCase("+", TestName = "is plus sign")]
        public void BeFalse_WhenValue(string value)
        {
            new NumberValidator(5, 2).IsValidNumber(value)
                                     .Should()
                                     .Be(false);
        }

        [Category("Checked value cases")]
        [TestCase("1", TestName = "has only integral part")]
        [TestCase("0.13", TestName = "has only fractal part")]
        [TestCase("1,4", TestName = "has comma as delimiter")]
        [TestCase("1.4", TestName = "has dot as delimiter")]
        [TestCase("-1", TestName = "has \"-\" sign")]
        [TestCase("+1", TestName = "has \"+\" sign")]
        [TestCase("00.00", TestName = "has several zeroes")]
        public void BeTrue_WhenValue(string value)
        {
            new NumberValidator(5, 2).IsValidNumber(value)
                                     .Should()
                                     .Be(true);
        }

        [Category("Format cases")]
        [TestCase(3, 2, false, "12,34", TestName = "has less precision than needed")]
        [TestCase(4, 0, false, "12,34", TestName = "has less scale than needed")]
        [TestCase(4, 2, false, "-12,34", TestName =
            "counts \"-\" sign as part of precision equals to amount of digits")]
        [TestCase(4, 2, false, "+12,34", TestName =
            "counts \"+\" sign as part of precision equals to amount of digits")]
        [TestCase(4, 2, true, "-12,34", TestName = "allows only positive values and value is negative")]
        [TestCase(4, 2, true, "-0", TestName = "allows only positive values and value is minus zero")]
        public void BeFalse_WhenFormat(int precision, int scale, bool onlyPositive, string value)
        {
            new NumberValidator(precision, scale, onlyPositive).IsValidNumber(value)
                                                               .Should()
                                                               .Be(false);
        }

		[TestCase(7, 3, false, "12,344", TestName = "has scale as much as needed")]
		[TestCase(2, 1, false, "12", TestName = "has precision as much as needed")]
        public void BeTrue_WhenFormat(int precision, int scale, bool onlyPositive, string value)
		{
			new NumberValidator(precision, scale, onlyPositive).IsValidNumber(value)
															   .Should()
															   .Be(true);

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
                throw new ArgumentException("scale must be a non-negative number less than precision");
            numberRegex = new Regex(@"^([+-]?)(\d+)([.,](\d+))?$", RegexOptions.IgnoreCase);
        }

        public bool IsValidNumber(string value)
        {
            // Проверяем соответствие входного значения формату N(m,k),
            // в соответствии с правилом, 
            // описанным в Формате описи документов, направляемых в налоговый орган
            // в электронном виде по телекоммуникационным каналам связи:
            // Формат числового значения указывается в виде N(m.к),
            // где m – максимальное количество знаков в числе, включая знак
            // (для отрицательного числа), 
            // целую и дробную часть числа без разделяющей десятичной точки,
            // k – максимальное число знаков дробной части числа. 
            // Если число знаков дробной части числа равно 0 (т.е. число целое),
            // то формат числового значения имеет вид N(m).

            if (string.IsNullOrEmpty(value))
                return false;

            var match = numberRegex.Match(value);
            if (!match.Success)
                return false;

            // Знак и целая часть
            var intPart = match.Groups[1]
                               .Value.Length +
                          match.Groups[2]
                               .Value.Length;
            // Дробная часть
            var fracPart = match.Groups[4]
                                .Value.Length;

            if (intPart + fracPart > precision || fracPart > scale)
                return false;

            return !onlyPositive || match.Groups[1].Value != "-";
        }
    }
}