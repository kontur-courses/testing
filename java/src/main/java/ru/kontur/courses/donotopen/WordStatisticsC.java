package ru.kontur.courses.donotopen;

import ru.kontur.courses.WordStatisticImpl;

public class WordStatisticsC extends WordStatisticImpl {
    @Override
    public void addWord(String word) {
        if (word == null) throw new IllegalArgumentException();
        if (word.isBlank()) return;
        if (word.length() > 10) word = word.substring(0, 10);
        if (!statistics.containsKey(word.toLowerCase()))
            statistics.put(word, 0);

        var count = statistics.get(word);
        count++;
    }
}
