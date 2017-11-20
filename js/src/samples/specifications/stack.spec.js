import { assert } from "chai";
import Stack from '../../lib/stack.js'

describe("Stack", () => {
    describe("constructor", () => {
        it("creates empty stack", () => {
            const stack = new Stack();

            assert.equal(stack.count(), 0)
        });

        it("pushes items to empty stack", () => {
            const stack = new Stack([1, 2, 3]);

            assert.equal(stack.count(), 3);
            assert.equal(stack.pop(), 3);
            assert.equal(stack.pop(), 2);
            assert.equal(stack.pop(), 1);
            assert.equal(stack.count(), 0);
        });
    });

    describe("toArray", () => {
        it("returns items in pop order", () => {
            const stack = new Stack([1, 2, 3]);

            assert.deepEqual(stack.toArray(), [3, 2, 1]);
        });
    });

    describe("push", () => {
        it("adds item to stack top", () => {
            const stack = new Stack([1, 2, 3]);

            stack.push(42);

            assert.deepEqual(stack.toArray(), [42, 3, 2, 1]);
        });
    });

    describe("pop", () => {
        it("fails on empty stack", () => {
            const stack = new Stack();

            assert.throws(() => { stack.pop(); }, "Stack is empty")
        });

        it("returns last pushed item", () => {
            const stack = new Stack([1, 2, 3]);

            stack.push(42);

            assert.equal(stack.pop(), 42);
        });
    });
});
