import chai from "chai";
chai.should();

import zip from "../../zip";

describe("zip should", function() {
    it("give resulting array of same length on arrays of equal length", function() {
        const arr1 = [1];
        const arr2 = [2];

        const result = zip(arr1, arr2, (a, b) => ({a, b}));

        result.should.eql([{a: 1, b: 2}]);
    });

    it("be empty when both inputs are empty", function() {
        const arr1 = [];
        const arr2 = [];

        const result = zip(arr1, arr2, (a, b) => ({a, b}));

        result.should.be.empty;
    });

    it("be empty when first is empty", function() {
        const arr1 = [];
        const arr2 = [1, 2];

        const result = zip(arr1, arr2, (a, b) => ({a, b}));

        result.should.be.empty;
    });

    it("be empty when second is empty", function() {
        const arr1 = [1, 2];
        const arr2 = [];

        const result = zip(arr1, arr2, (a, b) => ({a, b}));

        result.should.be.empty;
    });

    it("have length of second array when first contains more elements", function() {
        const arr1 = [1, 3, 5, 7];
        const arr2 = [2, 4];

        const result = zip(arr1, arr2, (a, b) => ({a, b}));

        result.should
            .have.lengthOf(arr2.length)
            .and.eql([{a: 1, b: 2}, {a: 3, b: 4}]);
    });

    it("have length of first array when second contains more elements", function() {
        const arr1 = [1, 3];
        const arr2 = [2, 4, 6, 8];

        const result = zip(arr1, arr2, (a, b) => ({a, b}));

        result.should
            .have.lengthOf(arr1.length)
            .and.eql([{a: 1, b: 2}, {a: 3, b: 4}]);
    });
});