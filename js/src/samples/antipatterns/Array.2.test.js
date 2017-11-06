import { expect } from "chai";
import fs from "fs";
import path from "path";

describe("Array tests 2", () => {
    it("test", () => {
        const array = [];
        array.push(10);
        array.push(20);
        array.push(30);

        while (array.length > 0) {
            console.log(array.pop());
        }
    })

    // <editor-fold defaultstate="collapsed" desc="Почему это плохо?">

    /*
        ## Антипаттерн Loudmouth

        Тест не является автоматическим. Если он сломается, никто этого не заметит.

        ## Мораль

        Вместо вывода на консоль, используйте Assert-ы.
    */

    // </editor-fold>
});