export default class Stack {
    constructor(array) {
        this.array = [];
        if (array) {
            for (let i = 0; i < array.length; i++) {
                this.array.push(array[i]);
            }
        }
    }

    push(item) {
        this.array.push(item);
    }

    pop() {
        if (this.array.length > 0) {
            return this.array.pop();
        }
        throw "Stack is empty";
    }

    peek() {
        if (this.array.length > 0) {
            return this.array[this.array.length - 1];
        }
        throw "Stack is empty";
    }

    any() {
        return this.array.length > 0;
    }

    count() {
        return this.array.length;
    }

    toArray() {
        const result = [];
        for (let i = 0; i < this.array.length; i++)
            result[i] = this.array[this.array.length - i - 1];
        return result;
    }
}