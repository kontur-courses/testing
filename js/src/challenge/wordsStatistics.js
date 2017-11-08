import * as stringHelpers from "./infrastructure/stringHelpers";
import ArgumentNullError from "./infrastructure/argumentNullError";

/**
 * Частотный словарь добавленных слов.
 * Слова сравниваются без учета регистра символов.
 * Порядок — по убыванию частоты слова.
 * При одинаковой частоте — в лексикографическом порядке.
 */
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
                word: keyValue[0],
                count: keyValue[1]
            }))
            .sort((a, b) => a.count !== b.count
                ? b.count - a.count
                : a.word.localeCompare(b.word));
    }
}