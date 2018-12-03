import chai, {expect} from "chai";
chai.should();

import WordsStatistics from "./wordsStatistics";
import ArgumentNullError from "../lib/argumentNullError";

describe("words statistics", function () {
    wordStatisticsTests(() => new WordsStatistics());
});

export default function wordStatisticsTests(createWordStatistics) {
    let wordsStatistics;

    beforeEach(() => {
        wordsStatistics = createWordStatistics();
    });

    describe("getStatistics", () => {
        it("is empty after creation", () => {
            wordsStatistics.getStatistics().should.be.empty;
        });

        it("contains item after addition", () => {
            const word = "abc";
            wordsStatistics.addWord(word);
            wordsStatistics
                .getStatistics()
                .should.be.eql([{word: word, count: 1}])
        });

        it("contains many items after addition of different words", () => {
            wordsStatistics.addWord("abc");
            wordsStatistics.addWord("def");
            wordsStatistics
                .getStatistics()
                .should.have.lengthOf(2);
        });
    });

    //Документация по BDD стилю проверок Chai Assertion Library (http://chaijs.com/api/bdd/)
}
