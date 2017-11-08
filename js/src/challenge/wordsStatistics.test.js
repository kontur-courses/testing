import chai from "chai";

chai.should();

import WordsStatistics from "./wordsStatistics";
import ArgumentNullError from "../lib/argumentNullError";


const tests = (wordsStatisticsFactory) => {
    return () => {
        let wordsStatistics;

        beforeEach(() => {
            wordsStatistics = wordsStatisticsFactory();
        });

        it("getStatistics is empty after creation", () => {
            wordsStatistics.getStatistics().should.be.empty;
        });

        it("getStatistics contains item after addition", () => {
            const word = "abc";
            wordsStatistics.addWord(word);
            wordsStatistics
                .getStatistics()
                .should.be.eql([{word: word, count: 1}])
        });

        it("getStatistics contains many items after addition of different words", () => {
            wordsStatistics.addWord("abc");
            wordsStatistics.addWord("def");
            wordsStatistics
                .getStatistics()
                .should.have.lengthOf(2);
        });

        //Документация по BDD стилю проверок Chai Assertion Library (http://chaijs.com/api/bdd/)

    };
};

describe("words statistics tests", tests(() => new WordsStatistics()));

export default tests;