using NUnit.Framework;
using System;

namespace HomeExercises.Tests
{
    [TestFixture(TestOf = typeof(NumberValidator))]
    public class NumberValidatorTests
    {
        [Test]
        public void NumberValidatorCtor_WhenPassValidArguments_ShouldNotThrows() =>
            Assert.DoesNotThrow(() => new NumberValidator(1, 0, true));

        private static TestCaseData[] ArgumentExceptionTestCases =
        {
            new TestCaseData(-1, 2, true).SetName("NegativePrecision"),
            new TestCaseData(0, 2, true).SetName("PrecisionEqualToZero"),
            new TestCaseData(1, -2, true).SetName("NegativeScale"),
            new TestCaseData(2, 2, true).SetName("PrecisionIsEqualToTheScale"),
            new TestCaseData(2, 3, true).SetName("ScaleIsMorePrecision")
        };

        [TestCaseSource(nameof(ArgumentExceptionTestCases))]
        public void NumberValidatorCtor_WhenPassInvalidArguments_ShouldThrowArgumentException(int precision, int scale, bool onlyPositive) =>
            Assert.Throws<ArgumentException>(() => new NumberValidator(precision, scale, onlyPositive));

        private static TestCaseData[] InvalidArgumentTestCases =
        {
            new TestCaseData(3,2,true,"a.sd").Returns(false).SetName("LettersInsteadOfNumber"),
            new TestCaseData(3,2,true,"2.!").Returns(false).SetName("SymbolsInsteadOfNumber"),
            new TestCaseData(3,2,true,null!).Returns(false).SetName("PassNumberIsNull"),
            new TestCaseData(3,2,true,"").Returns(false).SetName("PassNumberIsEmpty"),
            new TestCaseData(3,2,true,"       ").Returns(false).SetName("OnlySpaceInNumber"),
            new TestCaseData(3,2,true,"          2").Returns(false).SetName("MultipleSpacesAndOneDigitInNumber"),
            new TestCaseData(3,2,true,"2,.3").Returns(false).SetName("TwoSeparatorsArePassed"),
            new TestCaseData(3,2,true,".").Returns(false).SetName("OnlySeparatorArePassed"),
            new TestCaseData(3,2,true,"2 3").Returns(false).SetName("SeparatedBySpace"),
            new TestCaseData(3,2,true,"-0.00").Returns(false).SetName("IntPartWithNegativeSignMoreThanPrecision"),
            new TestCaseData(3,2,true,"+1.23").Returns(false).SetName("IntPartWithPositiveSignMoreThanPrecision"),
            new TestCaseData(3,2,true,"0.000").Returns(false).SetName("FractionalPartMoreThanScale"),
            new TestCaseData(3,2,true,"2%3").Returns(false).SetName("PercentSignInTheFormOfSeparator"),
            new TestCaseData(3,2,true,"2$").Returns(false).SetName("AmpersandInTheFormOfSeparator"),
            new TestCaseData(3,2,true,"#").Returns(false).SetName("OctothorpeInTheFormOfNumber"),
            new TestCaseData(3,2,true,"2@3").Returns(false).SetName("CommercialAtSymbolInTheFormOfSeparator"),
            new TestCaseData(3,2,true,"(2.3)").Returns(false).SetName("NumberInParentheses"),
            new TestCaseData(3,2,true,"2;3").Returns(false).SetName("SemicolonInTheFormOfSeparator"),
            new TestCaseData(3,2,true,"2/r").Returns(false).SetName("CarriageReturnInNumber"),
            new TestCaseData(3,2,true,"/n3").Returns(false).SetName("NewLineInNumber"),
            new TestCaseData(3,2,true,"/t3.4").Returns(false).SetName("TabInNumber"),
            new TestCaseData(3,2,true,"3.4/b").Returns(false).SetName("BackSpaceInNumber"),
            new TestCaseData(3,2,true,"3.47e+10").Returns(false).SetName("NumberInExponentialForm"),
            new TestCaseData(3,2,true,"10^3").Returns(false).SetName("NumberInAPower"),
            new TestCaseData(3,2,true,"11101010").Returns(false).SetName("BinaryNumberSystem"),
            new TestCaseData(3,2,true,"0xEA").Returns(false).SetName("HexadecimalNumberSystem"),
        };

        private static TestCaseData[] ValidArgumentTestCases =
        {
            new TestCaseData(3,2,true,"2,3").Returns(true).SetName("CharactersAreSeparatedByComma"),
            new TestCaseData(3,2,true,"0").Returns(true).SetName("FractionalPartIsMissing"),
            new TestCaseData(3,2,true,"0.0").Returns(true).SetName("NumberIsValid"),
            new TestCaseData(19,2,true,"9223372036854775807").Returns(true).SetName("LargeIntPartInNumber"),
            new TestCaseData(27,25,true,"3.1415926535897932384626433").Returns(true).SetName("LargeFracPartInNumber"),
            new TestCaseData(45,25,true,"9223372036854775807.1415926535897932384626433").Returns(true).SetName("LargeNumber")
        };

        [TestOf(nameof(NumberValidator.IsValidNumber))]
        [TestCaseSource(nameof(InvalidArgumentTestCases))]
        [TestCaseSource(nameof(ValidArgumentTestCases)), Repeat(2)]
        public bool NumberValidation_ShouldBeCorrect(int precision, int scale, bool onlyPositive, string number) =>
            new NumberValidator(precision, scale, onlyPositive).IsValidNumber(number);
    }
}
