import sortBy from "sort-by";

import * as stringHelpers from "./infrastructure/stringHelpers";

export default class WordsStatistics {
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
        return Array
            .from(this.statistics)
            .map((keyValue) => ({
                count: keyValue[1],
                word: keyValue[0]
            }))
            .sort(sortBy("-count", "word"));
    }
}