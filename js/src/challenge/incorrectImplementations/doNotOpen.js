import sortBy from "sort-by";
import deepEqual from "deep-equal";

import * as stringHelpers from "../infrastructure/stringHelpers";
import zip from "../../zip";

import WordsStatistics from "../wordsStatistics";

// <editor-fold defaultstate="collapsed" desc="Do not open!">

export class WordsStatisticsL2 extends WordsStatistics {
    addWord(word) {
        if (!stringHelpers.isDefinedString(word)) {
            throw Error("word should be a defined string");
        }

        if (stringHelpers.isWhitespace(word)) {
            return;
        }

        const lowerCaseWord = word.toLowerCase();
        this.statistics.set(lowerCaseWord, (this.statistics.get(lowerCaseWord) || 0) + 1);
    }
}

export class WordsStatisticsL3 extends WordsStatistics {
    addWord(word) {
        if (!stringHelpers.isDefinedString(word)) {
            throw Error("word should be a defined string");
        }

        if (stringHelpers.isWhitespace(word)) {
            return;
        }

        if (word.length > 10) {
            word = word.substr(0, 10);
        } else if (word.length > 5) {
            word = word.substr(0, word.length - 2);
        }

        const lowerCaseWord = word.toLowerCase();
        this.statistics.set(lowerCaseWord, (this.statistics.get(lowerCaseWord) || 0) + 1);
    }
}

export class WordsStatisticsL4 extends WordsStatistics {
    addWord(word) {
        if (!stringHelpers.isDefinedString(word)) {
            throw Error("word should be a defined string");
        }

        if (stringHelpers.isWhitespace(word)) {
            return;
        }

        if (word.length - 1 > 10) {
            word = word.substr(0, 10);
        }

        const lowerCaseWord = word.toLowerCase();
        this.statistics.set(lowerCaseWord, (this.statistics.get(lowerCaseWord) || 0) + 1);
    }
}

export class WordsStatisticsC extends WordsStatistics {
    addWord(word) {
        if (!stringHelpers.isDefinedString(word)) {
            throw Error("word should be a defined string");
        }

        if (stringHelpers.isWhitespace(word)) {
            return;
        }

        if (word.length > 10) {
            word = word.substr(0, 10);
        }

        if (!this.statistics.has(word.toLowerCase())) {
            this.statistics.set(word, 0);
        }

        this.statistics.set(word, (this.statistics.get(word) || 0) + 1);
    }
}

export class WordsStatisticsE extends WordsStatistics {
    addWord(word) {
        if (!stringHelpers.isDefinedString(word) || stringHelpers.isWhitespace(word)) {
            throw Error("word should be a defined string");
        }

        if (word.length > 10) {
            word = word.substr(0, 10);
        }

        const lowerCaseWord = word.toLowerCase();
        this.statistics.set(lowerCaseWord, (this.statistics.get(lowerCaseWord) || 0) + 1);
    }
}

export class WordsStatisticsE2 extends WordsStatistics {
    addWord(word) {
        if (!stringHelpers.isDefinedString(word) || stringHelpers.isWhitespace(word)) {
            return;
        }

        if (word.length > 10) {
            word = word.substr(0, 10);
        }

        const lowerCaseWord = word.toLowerCase();
        this.statistics.set(lowerCaseWord, (this.statistics.get(lowerCaseWord) || 0) + 1);
    }
}

export class WordsStatisticsE3 extends WordsStatistics {
    addWord(word) {
        if (!stringHelpers.isDefinedString(word)) {
            return;
        }

        if (word.length > 10) {
            word = word.substr(0, 10);
        }

        if (stringHelpers.isWhitespace(word)) {
            return;
        }

        const lowerCaseWord = word.toLowerCase();
        this.statistics.set(lowerCaseWord, (this.statistics.get(lowerCaseWord) || 0) + 1);
    }
}

export class WordsStatisticsE4 extends WordsStatistics {
    addWord(word) {
        if (stringHelpers.isWhitespace(word)) {
            return;
        }

        if (word.length > 10) {
            word = word.substr(0, 10);
        }

        const lowerCaseWord = word.toLowerCase();
        this.statistics.set(lowerCaseWord, (this.statistics.get(lowerCaseWord) || 0) + 1);
    }
}

export class WordsStatisticsO1 extends WordsStatistics {
    getStatistics() {
        return Array
            .from(this.statistics)
            .map((keyValue) => ({
                count: keyValue[1],
                word: keyValue[0]
            }))
            .sort(sortBy("word"));
    }
}

export class WordsStatisticsO2 extends WordsStatistics {
    getStatistics() {
        return Array
            .from(this.statistics)
            .map((keyValue) => ({
                count: keyValue[1],
                word: keyValue[0]
            }))
            .sort(sortBy("-count"));
    }
}

export class WordsStatisticsO3 extends WordsStatistics {
    getStatistics() {
        return super.getStatistics().sort(sortBy("word"));
    }
}

export class WordsStatisticsO4 extends WordsStatistics {
    getStatistics() {
        return super.getStatistics().sort(sortBy("-count", "-word"));
    }
}

export class WordsStatisticsCR {
    constructor() {
        this.statistics = new Map();
    }

    addWord(word) {
        if (!stringHelpers.isDefinedString(word)) {
            throw Error("word should be a defined string");
        }

        if (word.length === 0) {
            return;
        }

        if (word.length > 10) {
            word = word.substr(0, 10);
        }

        const lowerCaseWord = word.toLowerCase();
        this.statistics.set(lowerCaseWord, (this.statistics.get(lowerCaseWord) || 0) + 1);
    }

    getStatistics() {
        return Array
            .from(this.statistics)
            .map((keyValue) => ({
                count: keyValue[1],
                word: keyValue[0]
            }))
            .sort(sortBy("-count", "word"));
    }
}

// noinspection ES6ConvertVarToLetConst
var statistics_STA = new Map();

export class WordsStatisticsSTA {
    constructor() {
        statistics_STA.clear();
    }

    get statistics() {
        return statistics_STA;
    }

    addWord(word) {
        if (!stringHelpers.isDefinedString(word)) {
            throw Error("word should be a defined string");
        }

        if (stringHelpers.isWhitespace(word)) {
            return;
        }

        if (word.length > 10) {
            word = word.substr(0, 10);
        }

        const lowerCaseWord = word.toLowerCase();
        this.statistics.set(lowerCaseWord, (this.statistics.get(lowerCaseWord) || 0) + 1);
    }

    getStatistics() {
        return Array
            .from(this.statistics)
            .map((keyValue) => ({
                count: keyValue[1],
                word: keyValue[0]
            }))
            .sort(sortBy("-count", "word"));
    }
}

export class WordsStatistics123 {
    constructor() {
        this.MAX_SIZE = 12347;

        this.stats = new Array(this.MAX_SIZE);
        this.words = new Array(this.MAX_SIZE);
    }

    addWord(word) {
        if (!stringHelpers.isDefinedString(word)) {
            throw Error("word should be a defined string");
        }

        if (stringHelpers.isWhitespace(word)) {
            return;
        }

        if (word.length > 10) {
            word = word.substr(0, 10);
        }

        const lowerCaseWord = word.toLowerCase();
        const index = Math.abs(stringHelpers.calculateHash(lowerCaseWord)) % this.MAX_SIZE;
        this.stats[index] = this.stats[index] !== undefined
            ? this.stats[index] + 1
            : 1;
        this.words[index] = lowerCaseWord;
    }

    getStatistics() {
        return zip(this.stats, this.words, (s, w) => ({ count: s, word: w }))
            .filter((s) => s.count > 0)
            .sort(sortBy("-count", "word"))
    }
}

export class WordsStatisticsQWE {
    constructor() {
        this.statistics = new Map();
    }

    addWord(word) {
        if (!stringHelpers.isDefinedString(word)) {
            throw Error("word should be a defined string");
        }

        if (stringHelpers.isWhitespace(word)) {
            return;
        }

        if (word.length > 10) {
            word = word.substr(0, 10);
        }

        const lowerCaseWord = this.toLower(word);
        this.statistics.set(lowerCaseWord, (this.statistics.get(lowerCaseWord) || 0) + 1);
    }

    getStatistics() {
        return Array
            .from(this.statistics)
            .map((keyValue) => ({
                count: keyValue[1],
                word: keyValue[0]
            }))
            .sort(sortBy("-count", "word"));
    }

    toLower(word) {
        const englishCharsArray = [..."QWERTYUIOPLJKHGFDSAZXCVBNM"];
        const russianCharsArray = [..."ЙЦУКЕНГШЩЗФЫВАПРОЛДЯЧСМИТЬ"];

        const lowerCharacter = character => {
            const charCode = character.charCodeAt(0);
            if (englishCharsArray.includes(character)) {
                return String.fromCharCode(charCode - "D".charCodeAt(0) + "d".charCodeAt(0));
            } else if (russianCharsArray.includes(character)) {
                return String.fromCharCode(charCode - "Я".charCodeAt(0) + "я".charCodeAt(0));
            }

            return charCode;
        };

        return String.fromCharCode(...[...word].map(lowerCharacter));
    };
}

export class WordsStatistics998 {
    constructor() {
        this.statistics = [];
    }

    addWord(word) {
        if (!stringHelpers.isDefinedString(word)) {
            throw Error("word should be a defined string");
        }

        if (stringHelpers.isWhitespace(word)) {
            return;
        }

        if (word.length > 10) {
            word = word.substr(0, 10);
        }

        const lowerCaseWord = word.toLowerCase();
        let stat = this.statistics.filter(s => s.word === lowerCaseWord)[0] || null;
        if (stat !== null) {
            this.statistics = this.statistics.filter(s => !deepEqual(s, stat, { strict: true }));
        } else {
            stat = { count: 0, word: lowerCaseWord };
        }

        this.statistics.push({ count: stat.count - 1, word: stat.word });
        this.statistics.sort(sortBy("count", "word"));
    }

    getStatistics() {
        return this.statistics.map(s => ({count: -s.count, word: s.word}))
    }
}

export class WordsStatistics999 {
    constructor() {
        this.usedWords = new Set();
        this.statistics = [];
    }

    addWord(word) {
        if (!stringHelpers.isDefinedString(word)) {
            throw Error("word should be a defined string");
        }

        if (stringHelpers.isWhitespace(word)) {
            return;
        }

        if (word.length > 10) {
            word = word.substr(0, 10);
        }

        const lowerCaseWord = word.toLowerCase();

        if (this.usedWords.has(lowerCaseWord)) {
            const stat = this.statistics.filter(s => s.word === lowerCaseWord)[0] || null;
            this.statistics = this.statistics.filter(s => !deepEqual(s, stat, { strict: true }));
            this.statistics.push({ count: stat.count + 1, word: stat.word });
        } else {
            this.statistics.push({ count: 1, word: lowerCaseWord});
            this.usedWords.add(lowerCaseWord);
        }
    }

    getStatistics() {
        return this.statistics.sort(sortBy("-count", "word"));
    }
}

export class WordsStatisticsEN1 {
    constructor() {
        this.statistics = new Map();
    }

    addWord(word) {
        if (!stringHelpers.isDefinedString(word)) {
            throw Error("word should be a defined string");
        }

        if (stringHelpers.isWhitespace(word)) {
            return;
        }

        if (word.length > 10) {
            word = word.substr(0, 10);
        }

        const lowerCaseWord = word.toLowerCase();
        this.statistics.set(lowerCaseWord, (this.statistics.get(lowerCaseWord) || 0) + 1);
    }

    getStatistics() {
        const statistics = this.statistics;
        this.statistics = new Map();
        return Array
            .from(statistics)
            .map((keyValue) => ({
                count: keyValue[1],
                word: keyValue[0]
            }))
            .sort(sortBy("-count", "word"));
    }
}

export class WordsStatisticsEN2 extends WordsStatistics {
    constructor() {
        super();

        this.result = null;
    }

    getStatistics() {
        if (this.result !== undefined && this.result !== null) {
            return this.result;
        }

        this.result = super.getStatistics();
        return this.result;
    }
}

// </editor-fold>