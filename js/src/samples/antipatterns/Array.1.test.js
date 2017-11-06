import { expect } from "chai";
import fs from "fs";
import path from "path";

describe("Array tests 1", () => {
    it("test", () => {
        const lines = fs.readFileSync("C:/work/edu/testing-course/Patterns/data.txt")
            .split("\r\n")
            .map(line => ({ command: line[0], value: line[1] }));

        const array = [];
        for (const line of lines) {
            if (line.command === "push") {
                array.push(line.value);
            } else {
                expect(array.pop()).to.eql(line.value);
            }
        }
    })

    // <editor-fold defaultstate="collapsed" desc="Почему это плохо?">

    /*
        ## Антипаттерн Local Hero

        Тест не будет работать на машине другого человека или на Build-сервере.
        Да и у того же самого человека после переустановки ОС / повторного Clone репозитория / ...

        ## Решение

        Тест не должен зависеть от особенностей локальной среды.

        var lines = fs.readFileSync(path.resolve("data.txt"));
    */

    // </editor-fold>
});