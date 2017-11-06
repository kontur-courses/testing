import chai from "chai";

chai.should();

import WordsStatistics from "./wordsStatistics";

const tests = (wordsStatisticsFactory) => {
    return () => {
        let wordsStatistics;

        beforeEach(() => {
            wordsStatistics = wordsStatisticsFactory();
        });

        it("getStatistics is empty after creation", () => {
            wordsStatistics.getStatistics().should.be.empty;
        });

        it("getStatistics contains item after adding", () => {
            const word = "abc";
            wordsStatistics.addWord(word);
            wordsStatistics
                .getStatistics()
                .should.be.eql([{ word: word, count: 1 }])
        });

        it("getStatistics contains many items after different words adding", () => {
            wordsStatistics.addWord("abc");
            wordsStatistics.addWord("def");
            wordsStatistics
                .getStatistics()
                .should.have.lengthOf(2);
        });

        //Документация по Chai Assertion Library (http://chaijs.com/guide/styles)
    };
};

describe("words statistics tests", tests(() => new WordsStatistics()));

export default tests;