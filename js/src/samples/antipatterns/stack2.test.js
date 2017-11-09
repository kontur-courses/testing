import { expect } from "chai";
import Stack from '../../lib/stack.js'

describe("Stack2", () => {
    it("test", () => {
        const stack = new Stack();
        stack.push(10);
        stack.push(20);
        stack.push(30);
        while (stack.any()) {
            console.log(stack.pop());
        }
    })

    //#region Почему это плохо?

    /*
    ## Антипаттерн Loudmouth

    Тест не является автоматическим. Если он сломается, никто этого не заметит.

    ## Мораль

    Вместо вывода на консоль, используйте Assert-ы.
    */

    //#endregion
});