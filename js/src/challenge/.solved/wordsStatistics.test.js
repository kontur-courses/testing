import chai from "chai";
chai.should();

import WordsStatistics from "../wordsStatistics";
import ArgumentNullError from "../../lib/argumentNullError";


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

        //Документация по BDD стилю проверок Chai Assertion Library (http://chaijs.com/api/bdd/)

        it("sorts words by frequency", () => {
            wordsStatistics.addWord("aaaaaaaaaa");
            wordsStatistics.addWord("bbbbbbbbbb");
            wordsStatistics.addWord("bbbbbbbbbb");
            wordsStatistics.getStatistics().map(e => e.word)
                .should.be.eql(["bbbbbbbbbb", "aaaaaaaaaa"]);//strict ordering
        });

        it("sorts words by ABC when frequencies are same", () => {
            wordsStatistics.addWord("cccccccccc");
            wordsStatistics.addWord("aaaaaaaaaa");
            wordsStatistics.addWord("bbbbbbbbbb");
            wordsStatistics.getStatistics().map(e => e.word)
                .should.be.eql(["aaaaaaaaaa", "bbbbbbbbbb", "cccccccccc"]);
        });

        it("returns same result on second call", () => {
            wordsStatistics.addWord("abc");
            wordsStatistics.getStatistics()[0].should.be.eql({word: "abc", count: 1});
            wordsStatistics.getStatistics()[0].should.be.eql({word: "abc", count: 1});
        });

        it("builds result on every call", () => {
            wordsStatistics.addWord("abc");
            wordsStatistics.getStatistics().should.have.lengthOf(1);
            wordsStatistics.addWord("def");
            wordsStatistics.getStatistics().should.have.lengthOf(2);
        });
    });

    describe("addWord", () => {
        it("allows short words", () => {
            wordsStatistics.addWord("aaa");
        });

        it("counts once when same word", () => {
            wordsStatistics.addWord("aaaaaaaaaa");
            wordsStatistics.addWord("aaaaaaaaaa");
            wordsStatistics.getStatistics().should.have.lengthOf(1);
        });

        it("increments counter when same word", () => {
            wordsStatistics.addWord("aaaaaaaaaa");
            wordsStatistics.addWord("aaaaaaaaaa");
            wordsStatistics.getStatistics()[0].count
                .should.be.eql(2);
        });

        it("throws when word is null", () => {
            (() => {wordsStatistics.addWord(null)}).should.throw(ArgumentNullError);
        });

        it("ignores empty word", () => {
            wordsStatistics.addWord("");
            wordsStatistics.getStatistics().should.be.empty;
        });

        it("ignores whitespace word", () => {
            wordsStatistics.addWord(" ");
            wordsStatistics.getStatistics().should.be.empty;
        });

        it("cuts words longer than 10", () => {
            wordsStatistics.addWord("12345678901");
            wordsStatistics.getStatistics().map(e => e.word)[0]
                .should.be.eql("1234567890");
        });

        it("cuts words joined", () => {
            wordsStatistics.addWord("12345678901");
            wordsStatistics.addWord("1234567890");
            wordsStatistics.getStatistics().map(e => e.count)[0]
                .should.be.eql(2);
        });

        it("allows word and cut it to whitespaces when word preceded by whitespaces", () => {
            wordsStatistics.addWord("          a");
            wordsStatistics.getStatistics().map(e => e.word)[0]
                .should.be.eql("          ");
        });

        it("is case insensitive", () => {
            const codes = [];
            for (let c = "a".charCodeAt(0); c <= "z".charCodeAt(0); c++) {
                codes.push(c);
            }
            for (let c = "а".charCodeAt(0); c <= "я".charCodeAt(0) || c <= "ё".charCodeAt(0); c++) {
                codes.push(c);
            }
            for (let c of codes) {
                wordsStatistics.addWord(String.fromCharCode(c));
                wordsStatistics.addWord(String.fromCharCode(c).toUpperCase());
            }

            wordsStatistics.getStatistics().should.have.lengthOf(codes.length);
        });

        it("has no collisions", () => {
            const wordCount = 500;
            for (let i = 0; i < wordCount; i++) {
                wordsStatistics.addWord(i.toString());
            }
            wordsStatistics.getStatistics().should.be.lengthOf(wordCount);
        });

        it("has sufficient performance on adding different words", () => {
            for (let i = 0; i < 100; i++) {
                wordsStatistics.addWord(i.toString());
            }
            wordsStatistics.getStatistics();
        }).timeout(10);

        it("has sufficient performance on adding same word", () => {
            for (let i = 0; i < 300; i++) {
                wordsStatistics.addWord(i.toString());
            }
            const sameWord = "9";
            for (let i = 0; i < 300; i++) {
                wordsStatistics.addWord(sameWord);
            }
            wordsStatistics.getStatistics();
        }).timeout(10);
    });

    it("supports several instances", () => {
        const anotherWordsStatistics = createWordStatistics();
        wordsStatistics.addWord("aaaaaaaaaa");
        anotherWordsStatistics.addWord("bbbbbbbbbb");
        wordsStatistics.getStatistics().should.be.lengthOf(1);
    });
}
