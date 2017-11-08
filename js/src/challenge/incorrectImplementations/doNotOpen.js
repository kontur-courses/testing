import * as stringHelpers from "../infrastructure/stringHelpers";
import ArgumentNullError from "../infrastructure/argumentNullError";
import "../../lib/zip";

import WordsStatistics from "../wordsStatistics";

// <editor-fold defaultstate="collapsed" desc="Do not open!">

export class WordsStatisticsL2 extends WordsStatistics {
    addWord(word) {
        if (!stringHelpers.isDefinedString(word)) {
            throw new ArgumentNullError("word should be a defined string");
        }
        if (stringHelpers.isWhitespace(word)) {
            return;
        }
        this.statistics.set(word.toLowerCase(), 1 + (this.statistics.get(word.toLowerCase()) || 0));
    }
}

export class WordsStatisticsL3 extends WordsStatistics {
    addWord(word) {
        if (!stringHelpers.isDefinedString(word)) {
            throw new ArgumentNullError("word should be a defined string");
        }
        if (stringHelpers.isWhitespace(word)) {
            return;
        }
        if (word.length > 10) {
            word = word.substr(0, 10);
        } else if (word.length > 5) {
            word = word.substr(0, word.length - 2);
        }
        this.statistics.set(word.toLowerCase(), 1 + (this.statistics.get(word.toLowerCase()) || 0));
    }
}

export class WordsStatisticsL4 extends WordsStatistics {
    addWord(word) {
        if (!stringHelpers.isDefinedString(word)) {
            throw new ArgumentNullError("word should be a defined string");
        }
        if (stringHelpers.isWhitespace(word)) {
            return;
        }
        if (word.length - 1 > 10) {
            word = word.substr(0, 10);
        }
        this.statistics.set(word.toLowerCase(), 1 + (this.statistics.get(word.toLowerCase()) || 0));
    }
}

export class WordsStatisticsC extends WordsStatistics {
    addWord(word) {
        if (!stringHelpers.isDefinedString(word)) {
            throw new ArgumentNullError("word should be a defined string");
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
        this.statistics.set(word, 1 + this.statistics.get(word));
    }
}

export class WordsStatisticsE extends WordsStatistics {
    addWord(word) {
        if (!stringHelpers.isDefinedString(word) || stringHelpers.isWhitespace(word)) {
            throw new ArgumentNullError("word should be a defined string");
        }
        if (word.length > 10) {
            word = word.substr(0, 10);
        }
        this.statistics.set(word.toLowerCase(), 1 + (this.statistics.get(word.toLowerCase()) || 0));
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
        this.statistics.set(word.toLowerCase(), 1 + (this.statistics.get(word.toLowerCase()) || 0));
    }
}

export class WordsStatisticsE3 extends WordsStatistics {
    addWord(word) {
        if (!stringHelpers.isDefinedString(word)) {
            throw new ArgumentNullError("word should be a defined string");
        }
        if (word.length > 10) {
            word = word.substr(0, 10);
        }
        if (stringHelpers.isWhitespace(word)) {
            return;
        }
        this.statistics.set(word.toLowerCase(), 1 + (this.statistics.get(word.toLowerCase()) || 0));
    }
}

export class WordsStatisticsE4 extends WordsStatistics {
    addWord(word) {
        if (word.length === 0 || stringHelpers.isWhitespace(word)) {
            return;
        }
        if (word.length > 10) {
            word = word.substr(0, 10);
        }
        this.statistics.set(word.toLowerCase(), 1 + (this.statistics.get(word.toLowerCase()) || 0));
    }
}

export class WordsStatisticsO1 extends WordsStatistics {
    getStatistics() {
        return Array
            .from(this.statistics)
            .map((keyValue) => ({
                word: keyValue[0],
                count: keyValue[1]
            }))
            .sort((a, b) => a.word.localeCompare(b.word));
    }
}

export class WordsStatisticsO2 extends WordsStatistics {
    getStatistics() {
        return Array
            .from(this.statistics)
            .map((keyValue) => ({
                word: keyValue[0],
                count: keyValue[1]
            }))
            .sort((a, b) => b.count - a.count);
    }
}

export class WordsStatisticsO3 extends WordsStatistics {
    getStatistics() {
        return super.getStatistics().sort((a, b) => a.word.localeCompare(b.word));
    }
}

export class WordsStatisticsO4 extends WordsStatistics {
    getStatistics() {
        return super.getStatistics().sort((a, b) => a.count !== b.count
            ? b.count - a.count
            : -a.word.localeCompare(b.word));
    }
}

export class WordsStatisticsCR {
    constructor() {
        this.statistics = new Map();
    }

    addWord(word) {
        if (!stringHelpers.isDefinedString(word)) {
            throw new ArgumentNullError("word should be a defined string");
        }
        if (stringHelpers.isEmpty(word)) {
            return;
        }
        if (word.length > 10) {
            word = word.substr(0, 10);
        }
        word = word.toLowerCase();

        this.statistics.set(word, 1 + (this.statistics.get(word) || 0));
    }

    getStatistics() {
        return Array
            .from(this.statistics)
            .map((keyValue) => ({
                word: keyValue[0],
                count: keyValue[1]
            }))
            .sort((a, b) => a.count !== b.count
                ? b.count - a.count
                : a.word.localeCompare(b.word));
    }
}

const statistics_STA = new Map();

export class WordsStatisticsSTA {
    constructor() {
        statistics_STA.clear();
    }

    get statistics() {
        return statistics_STA;
    }

    addWord(word) {
        if (!stringHelpers.isDefinedString(word)) {
            throw new ArgumentNullError("word should be a defined string");
        }
        if (stringHelpers.isWhitespace(word)) {
            return;
        }
        if (word.length > 10) {
            word = word.substr(0, 10);
        }
        this.statistics.set(word.toLowerCase(), 1 + (this.statistics.get(word.toLowerCase()) || 0));
    }

    getStatistics() {
        return Array
            .from(this.statistics)
            .map((keyValue) => ({
                word: keyValue[0],
                count: keyValue[1]
            }))
            .sort((a, b) => a.count !== b.count
                ? b.count - a.count
                : a.word.localeCompare(b.word));
    }
}

export class WordsStatistics123 {
    constructor() {
        this.MAX_SIZE = 1237;

        this.statistics = new Array(this.MAX_SIZE);
        this.words = new Array(this.MAX_SIZE);
    }

    addWord(word) {
        if (!stringHelpers.isDefinedString(word)) {
            throw new ArgumentNullError("word should be a defined string");
        }
        if (stringHelpers.isWhitespace(word)) {
            return;
        }
        if (word.length > 10) {
            word = word.substr(0, 10);
        }
        const index = Math.abs(stringHelpers.calculateHash(word.toLowerCase())) % this.MAX_SIZE;
        this.statistics[index] = 1 + (this.statistics[index] || 0);
        this.words[index] = word.toLowerCase();
    }

    getStatistics() {
        return this.statistics
            .zip(this.words, (s, w) => ({ word: w, count: s }))
            .filter(s => s.count > 0)
            .sort((a, b) => a.count !== b.count
                ? b.count - a.count
                : a.word.localeCompare(b.word))
    }
}

export class WordsStatisticsQWE {
    constructor() {
        this.statistics = new Map();
    }

    addWord(word) {
        if (!stringHelpers.isDefinedString(word)) {
            throw new ArgumentNullError("word should be a defined string");
        }
        if (stringHelpers.isWhitespace(word)) {
            return;
        }
        if (word.length > 10) {
            word = word.substr(0, 10);
        }
        this.statistics.set(this.toLower(word), 1 + (this.statistics.get(this.toLower(word)) || 0));
    }

    getStatistics() {
        return Array
            .from(this.statistics)
            .map((keyValue) => ({
                word: keyValue[0],
                count: keyValue[1]
            }))
            .sort((a, b) => a.count !== b.count
                ? b.count - a.count
                : a.word.localeCompare(b.word));
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
            throw new ArgumentNullError("word should be a defined string");
        }
        if (stringHelpers.isWhitespace(word)) {
            return;
        }
        if (word.length > 10) {
            word = word.substr(0, 10);
        }
        let lowerCaseWord = word.toLowerCase();
        let wordCount = this.statistics.find(s => s.word === lowerCaseWord) || null;
        if (wordCount !== null) {
            this.statistics = this.statistics.filter(s => s.word !== wordCount.word || s.count !== wordCount.count);
        } else {
            wordCount = { word: lowerCaseWord, count: 0 };
        }
        this.statistics.push({ word: wordCount.word, count: wordCount.count - 1 });
        this.statistics.sort((a, b) => a.count === b.count
            ? a.word.localeCompare(b.word)
            : a.count - b.count);
    }

    getStatistics() {
        return this.statistics.map(s => ({word: s.word, count: -s.count}))
    }
}

export class WordsStatistics999 {
    constructor() {
        this.usedWords = new Set();
        this.statistics = [];
    }

    addWord(word) {
        if (!stringHelpers.isDefinedString(word)) {
            throw new ArgumentNullError("word should be a defined string");
        }
        if (stringHelpers.isWhitespace(word)) {
            return;
        }
        if (word.length > 10) {
            word = word.substr(0, 10);
        }
        word = word.toLowerCase();

        if (this.usedWords.has(word)) {
            let wordCount = this.statistics.find(s => s.word === word) || null;
            this.statistics = this.statistics.filter(s => s.word !== wordCount.word || s.count !== wordCount.count);
            this.statistics.push({ word: wordCount.word, count: wordCount.count + 1 });
        } else {
            this.statistics.push({ word: word, count: 1 });
            this.usedWords.add(word);
        }
    }

    getStatistics() {
        return this.statistics.sort((a, b) => a.count !== b.count
            ? b.count - a.count
            : a.word.localeCompare(b.word));
    }
}

export class WordsStatisticsEN1 {
    constructor() {
        this.statistics = new Map();
    }

    addWord(word) {
        if (!stringHelpers.isDefinedString(word)) {
            throw new ArgumentNullError("word should be a defined string");
        }
        if (stringHelpers.isWhitespace(word)) {
            return;
        }
        if (word.length > 10) {
            word = word.substr(0, 10);
        }
        this.statistics.set(word.toLowerCase(), 1 + (this.statistics.get(word.toLowerCase()) || 0));
    }

    getStatistics() {
        const temp = this.statistics;
        this.statistics = new Map();
        return Array
            .from(temp)
            .map((keyValue) => ({
                word: keyValue[0],
                count: keyValue[1]
            }))
            .sort((a, b) => a.count !== b.count
                ? b.count - a.count
                : a.word.localeCompare(b.word));
    }
}

export class WordsStatisticsEN2 extends WordsStatistics {
    constructor() {
        super();
        this.result = null;
    }

    getStatistics() {
        if (this.result) {
            return this.result;
        }
        this.result = super.getStatistics();
        return this.result;
    }
}

// </editor-fold>