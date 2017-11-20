import {assert} from "chai";

describe("Number", () => {
    const tests = [
        {a: 12, b: 3, expected: 4},
        {a: 12, b: 2, expected: 6},
        {a: 12, b: 4, expected: 3}
    ];

    tests.forEach(test => {
        it(`should divide (${test.a}, ${test.b})`, () => {
            assert.equal(test.a / test.b, test.expected);
        });
    });

    [
        {input: "123", expectedResult: 123, testName: "integer"},
        {input: "1.1", expectedResult: 1.1, testName: "faction"},
        {input: "1.1e1", expectedResult: 1.1e1, testName: "scientific with positive exp"},
        {input: "1.1e-1", expectedResult: 1.1e-1, testName: "scientific with negative exp"},
        {input: "-0.1", expectedResult: -0.1, testName: "negative fraction"}
    ].forEach(({input, expectedResult, testName}) => {
        it("should parse " + testName, () => {
            assert.equal(Number.parseFloat(input), expectedResult);
        });
    });
});
