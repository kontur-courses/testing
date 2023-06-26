package ru.kontur.courses.donotopen;

import ru.kontur.courses.WordStatisticImpl;

public class WordStatisticsL2 extends WordStatisticImpl {
    @Override
    public void addWord(String word) {
        if (word == null) throw new IllegalArgumentException();
        if (word.isBlank()) return;
        var lowerWord = word.toLowerCase();
        statistics.put(lowerWord, 1 + statistics.getOrDefault(lowerWord, 0));
    }
}
