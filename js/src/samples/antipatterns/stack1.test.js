import { expect } from "chai";
import fs from "fs";
import Stack from '../../lib/stack.js'

describe("Stack1", () => {
    it("test", () => {
        const lines = fs.readFileSync("C:\\work\\edu\\testing-course\\Patterns\\data.txt")
            .split(" ")
            .map(line => ({ command: line[0], value: line[1] }));

        const stack = new Stack();
        for (const line of lines) {
            if (line.command === "push") {
                stack.push(line.value);
            } else {
                assert.equal(stack.pop(), line.value);
            }
        }
    });

    //#region Почему это плохо?

    /*
    ## Антипаттерн Local Hero

    Тест не будет работать на машине другого человека или на Build-сервере.
    Да и у того же самого человека после переустановки ОС / повторного Clone репозитория / ...

    ## Решение

    Тест не должен зависеть от особенностей локальной среды.
    Если нужна работа с файлами, делайте это по относительным путям.

    // path.resolve превращает относительный путь в абсолютный
    var lines = fs.readFileSync(path.resolve("data.txt"));
    */

    //#endregion
});