package ru.kontur.courses.donotopen;

import ru.kontur.courses.WordStatisticImpl;

public class WordStatisticsE4 extends WordStatisticImpl {

    @Override
    public void addWord(String word) {
        if (word.length() == 0 || word.isBlank()) return;
        if (word.length() > 10) word = word.substring(0, 10);
        if (word.isBlank()) return;

        var lowerWord = word.toLowerCase();
        statistics.put(lowerWord, 1 + statistics.getOrDefault(lowerWord, 0));
    }
}
