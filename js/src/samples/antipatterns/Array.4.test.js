import { expect } from "chai";
import fs from "fs";
import path from "path";

describe("Array tests 4", () => {
    it("test", () => {
        const array = [1, 2, 3, 4, 5];
        const result = array.pop();
        expect(result).to.equal(5);
        expect(array).to.not.be.empty;
        expect(array).to.have.lengthOf(4);
        expect(array.slice(-1)[0]).to.equal(4);
    })

    // <editor-fold defaultstate="collapsed" desc="Почему это плохо?">

    /*
        ## Антипаттерн Overspecification

        1. Непонятна область ответственности. Сложно придумать название. Не так плохо, как FreeRide, но плохо.

        2. Изменение API роняет сразу много подобных тестов, создавая много рутинной работы по их починке.

        3. Если все тесты будут такими, то при появлении бага, падают они большой компанией.


        ## Мораль

        Сфокусируйтесь на проверке одного конкретного требования в каждом тесте.
        Не старайтесь проверить "за одно" какое-то требование сразу во всех тестах — это может выйти боком.

        Признак возможной проблемы — более одного Assert на метод.
    */

    // </editor-fold>
});