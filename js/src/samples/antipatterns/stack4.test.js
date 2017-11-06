import { assert } from "chai";
import Stack from '../../lib/stack.js'

describe("Stack4", () => {
    it("test", () => {
        const stack = new Stack([1, 2, 3, 4, 5]);
        const result = stack.pop();
        assert.equal(result, 5);
        assert.isTrue(stack.any());
        assert.equal(stack.count(), 4);
        assert.equal(stack.peek(), 4);
        assert.deepEqual(stack.toArray(), [4, 3, 2, 1]);
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