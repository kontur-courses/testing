import sortBy from "sort-by";

import * as stringHelpers from "./infrastructure/stringHelpers";
import ArgumentNullError from "./infrastructure/argumentNullError";

export default class WordsStatistics {
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
        return Array
            .from(this.statistics)
            .map((keyValue) => ({
                count: keyValue[1],
                word: keyValue[0]
            }))
            .sort(sortBy("-count", "word"));
    }
}