import test from "mocha-cases";

describe("Number parseFloat should", () => {
    test({name: "parse integer", value: "123", expected: 123}, Number.parseFloat);
    test({name: "parse fraction", value: "1.1", expected: 1.1}, Number.parseFloat);
    test({name: "parse scientific with positive exp", value: "1.1e1", expected: 1.1e1}, Number.parseFloat);
    test({name: "parse scientific with negative exp", value: "1.1e-1", expected: 1.1e-1}, Number.parseFloat);
    test({name: "parse negative fraction", value: "-0.1", expected: -0.1}, Number.parseFloat);
});