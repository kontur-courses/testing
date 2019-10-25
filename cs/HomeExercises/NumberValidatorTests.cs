using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
        static class GlobalMethods
        {
            public static string GetValidatorArgsAsString((int precision, int scale, bool onlyPositive) validatorInitArgs) =>
                validatorInitArgs.ToString();

            public static NumberValidator GetValidator(int precision, int scale = 0, bool onlyPositive = false) =>
                new NumberValidator(precision, scale, onlyPositive);

            public static NumberValidator GetValidator((int precision, int scale, bool onlyPositive) validatorInitArgs) =>
                GetValidator(validatorInitArgs.precision, validatorInitArgs.scale, validatorInitArgs.onlyPositive);
        }

        public class InitializationTests
        {
            #region Positive tests

            [Test]
            public void ShouldNotThrow_WithCorrectArguments()
            {
                Action act = () => GlobalMethods.GetValidator(1, 0, true);
                act.ShouldNotThrow($"arguments {GlobalMethods.GetValidatorArgsAsString((1, 0, true))}" +
                    $" are correct for NumberValidator initialization");
            }

            #endregion

            #region Negative tests

            private string MakeNegativeInitializationBecause(string validatorArgs) =>
                $"arguments ({validatorArgs}) are incorrect for NumberValidator initialization";

            private void Check_ShouldThrow<TException>((int precision, int scale, bool onlyPositive) validatorInitArgs)
                where TException : Exception
            {
                Action act = () => GlobalMethods.GetValidator(validatorInitArgs);
                var validatorArgsAsString = GlobalMethods.GetValidatorArgsAsString(validatorInitArgs);

                act.ShouldThrow<TException>(MakeNegativeInitializationBecause(validatorArgsAsString));
            }

            [Test]
            public void ShouldThrow_WithNegativePrecision() =>
                Check_ShouldThrow<ArgumentException>((-1, 2, true));

            [Test]
            public void ShouldThrow_WithZeroPrecision() =>
                Check_ShouldThrow<ArgumentException>((0, 0, false));

            [Test]
            public void ShouldThrow_WithNegativeScale() =>
                Check_ShouldThrow<ArgumentException>((10, -1, true));

            [Test]
            public void ShouldThrow_WhenScaleEqualsPrecision() =>
                Check_ShouldThrow<ArgumentException>((10, 10, true));

            [Test]
            public void ShouldThrow_WhenScaleMoreThanPrecision() =>
                Check_ShouldThrow<ArgumentException>((10, 15, true));

            #endregion
        }

        public class IsValidNumberMethodTests
        {
            private void Do_Test_ForEachNumber((int precision, int scale, bool onlyPositive) validatorInitArgs, string[] numbers,
                Action<NumberValidator, string, string> testForEachNumber)
            {
                var validator = GlobalMethods.GetValidator(validatorInitArgs);
                var validatorArgsAsString = GlobalMethods.GetValidatorArgsAsString(validatorInitArgs);
                foreach (var n in numbers)
                    testForEachNumber(validator, validatorArgsAsString, n);
            }

            private string MakeBecause(string number, string validatorArgs, bool isPositive) =>
                $"\"{number}\" is " +
                    (isPositive ? "valid" : "invalid")
                    + $" number in {validatorArgs} NumberValidator";

            #region Positive tests

            private string MakePositiveBecause(string number, string validatorArgs) =>
                MakeBecause(number, validatorArgs, true);

            private void CheckValidity_ForEachNumber((int precision, int scale, bool onlyPositive) validatorInitArgs, params string[] numbers) =>
                    Do_Test_ForEachNumber(validatorInitArgs, numbers,
                        (validator, validatorArgsAsString, number) =>
                            validator.IsValidNumber(number).Should().BeTrue(MakePositiveBecause(number, validatorArgsAsString)));

            [Test]
            public void ShouldValid_OnSomePositiveInteger() =>
                CheckValidity_ForEachNumber((17, 2, true), "0", "+0");

            [Test]
            public void ShouldValid_WhenNumberStartsWithZero_SignIsIgnored() =>
                CheckValidity_ForEachNumber((17, 2, false), "000", "+02.3", "-012");

            [Test]
            public void ShouldValid_OnSomeNegativeInteger() =>
                CheckValidity_ForEachNumber((17, 2, false), "-0");

            [Test]
            public void ShouldValid_OnSomePositiveFractionalNumber() =>
                CheckValidity_ForEachNumber((17, 2, true), "0.0", "+0.0");

            [Test]
            public void ShouldValid_OnSomeNegativeFractionalNumber() =>
                CheckValidity_ForEachNumber((17, 10, false), "-0.0");

            [Test]
            public void ShouldValid_WhenNumberLengthEqualsMaxValidatorLength() =>
                CheckValidity_ForEachNumber((4, 2, false), "1234", "123.4", "-123", "+123", "-12.3", "+1.23");

            [Test]
            public void ShouldValid_WhenNumberFracPartLengthEqualsMaxValidatorFracPartLength() =>
                CheckValidity_ForEachNumber((5, 2, false), "1.23", "-1.23", "+1.23");

            [Test]
            public void ShouldValid_WhenSeparatorIsDot() =>
                CheckValidity_ForEachNumber((10, 5, false), "1233.21", "-12.23", "+1.43");

            [Test]
            public void ShouldValid_WhenSeparatorIsComma() =>
                CheckValidity_ForEachNumber((10, 5, false), "1233,21", "-12,23", "+1,43");

            [Test]
            public void ShouldValid_WhenSignIsPlus() =>
                CheckValidity_ForEachNumber((10, 5, true), "+0", "+1.12", "+13,32");

            [Test]
            public void ShouldValid_WhenSignIsMinus() =>
                CheckValidity_ForEachNumber((10, 5, false), "-0", "-1.12", "-13,32");

            [Test]
            public void SeparatorShouldNotCountInNumberLength() =>
                CheckValidity_ForEachNumber((3, 2, true), "12.3", "12,3");

            #endregion

            #region Negative tests

            private string MakeNegativeBecause(string number, string validatorArgs) =>
                MakeBecause(number, validatorArgs, false);

            private void CheckNotValidity_ForEachNumber((int precision, int scale, bool onlyPositive) validatorInitArgs, params string[] numbers) =>
                Do_Test_ForEachNumber(validatorInitArgs, numbers,
                    (validator, validatorArgsAsString, number) =>
                        validator.IsValidNumber(number).Should().BeFalse(MakeNegativeBecause(number, validatorArgsAsString)));

            [Test]
            public void ShouldInvalid_WhenNumberLengthMoreThanMaxValidatorNumberLength() =>
                CheckNotValidity_ForEachNumber((3, 2, true), "00.00", "1234");

            [Test]
            public void ShouldInvalid_WithNegativeNumberInOnlyPositiveValidator() =>
                CheckNotValidity_ForEachNumber((3, 2, true), "-123", "-0.00");

            [Test]
            public void PlusShouldCountInNumberLength() =>
                CheckNotValidity_ForEachNumber((3, 2, true), "+0.00");

            [Test]
            public void MinusShouldCountInNumberLength() =>
                CheckNotValidity_ForEachNumber((3, 2, false), "-0.00");

            [Test]
            public void ShouldInvalid_WhenFracPartLengthMoreThanMaxValidatorFracPartLength() =>
                CheckNotValidity_ForEachNumber((17, 2, true), "0.000");

            [Test]
            public void ShouldInvalid_WithNotANumber() =>
                CheckNotValidity_ForEachNumber((17, 2, false), "asd", "a.sd", "+asd", "-asd");

            [Test]
            public void ShouldInvalid_WhenWhiteSpaceBeforeNumber() =>
                CheckNotValidity_ForEachNumber((17, 2, false), "  123");

            [Test]
            public void ShouldInvalid_WithNull() =>
                CheckNotValidity_ForEachNumber((17, 2, true), new string[] { null });

            [Test]
            public void ShouldInvalid_WithEmptyString() =>
                CheckNotValidity_ForEachNumber((17, 2, true), string.Empty);

            [Test]
            public void ShouldInvalid_WhenArgumentStartsWithSeparator() =>
                CheckNotValidity_ForEachNumber((17, 2, true), ".12", ",12");

            #endregion
        }

        [Test, Description("Создается последовательно два разных объекта NumberValidator, " +
            "первый объект не должен менять значения своих полей на значения полей второго объекта, " +
            "и второй объект не должен принимать значения полей первого объекта")]
        public void ValidatorObjectsShouldBeIndependOfEachOther()
        {
            var firstValidator = GlobalMethods.GetValidator(10, 3, false);
            var secondValidator = GlobalMethods.GetValidator(3, 2, true);
            firstValidator.IsValidNumber("-13456.678").Should().BeTrue();
            secondValidator.IsValidNumber("-3").Should().BeFalse();
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