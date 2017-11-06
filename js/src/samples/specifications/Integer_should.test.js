import test from "mocha-cases";

describe("integer should", () => {
    test({
        name: "divide",
        values: [{ a: 12, b: 3 }, { a: 12, b: 2 }, { a: 12, b: 4 }],
        expected: [4, 6, 3]
    }, ({a, b}) => a / b)
});