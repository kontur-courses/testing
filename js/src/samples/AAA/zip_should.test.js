import "../../lib/zip";
import {assert} from "chai";

suite("zip should", function() {
    test("give result of same size on equal size arrays", function() {
        const arr1 = [1];
        const arr2 = [2];

        const result = arr1.zip(arr2, (a, b) => ({a, b}));

        assert.deepEqual(result, [{a: 1, b: 2}]);
    });

    test("be empty when both inputs are empty", function() {
        const arr1 = [];
        const arr2 = [];

        const result = arr1.zip(arr2, (a, b) => ({a, b}));

        assert.isEmpty(result);
    });

    test("be empty when first is empty", function() {
        const arr1 = [];
        const arr2 = [1, 2];

        const result = arr1.zip(arr2, (a, b) => ({a, b}));

        assert.isEmpty(result);
    });

    test("be empty when second is empty", function() {
        const arr1 = [1, 2];
        const arr2 = [];

        const result = arr1.zip(arr2, (a, b) => ({a, b}));

        assert.isEmpty(result);
    });

    test("have length of second when first contains more elements", function() {
        const arr1 = [1, 3, 5, 7];
        const arr2 = [2, 4];

        const result = arr1.zip(arr2, (a, b) => ({a, b}));

        assert.deepEqual(result, [{a: 1, b: 2}, {a: 3, b: 4}]);
    });

    test("have length of first when second contains more elements", function() {
        const arr1 = [1, 3];
        const arr2 = [2, 4, 6, 8];

        const result = arr1.zip(arr2, (a, b) => ({a, b}));

        assert.deepEqual(result, [{a: 1, b: 2}, {a: 3, b: 4}]);
    });
});