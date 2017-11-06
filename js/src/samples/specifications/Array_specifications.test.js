import chai, { expect } from "chai";
chai.should();

describe("Array specifications", () => {
    it("empty array has zero length", () => {
        const array = [];

        array.should.have.lengthOf(0);
    });

    it("pop on empty array returns undefined", () => {
        const array = [];

        const lastValue = array.pop();
        expect(lastValue).to.be.undefined;
    });

    it("push adds item to array end", () => {
        const array = [1, 2, 3];

        array.push(42);

        array.should.eql([1, 2, 3, 42]);
    });

    it("pop returns last pushed value", () => {
        const array = [1, 2, 3];

        array.push(42);

        const lastValue = array.pop();
        lastValue.should.be.equal(42);
    })
});