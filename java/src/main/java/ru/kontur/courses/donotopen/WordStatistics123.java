package ru.kontur.courses.donotopen;

import ru.kontur.courses.WordCount;
import ru.kontur.courses.WordStatistics;

import java.util.Comparator;
import java.util.List;
import java.util.stream.Collectors;
import java.util.stream.IntStream;

public class WordStatistics123 implements WordStatistics {
    private final int MAX_SIZE = 1237;

    private final int[] statistics = new int[MAX_SIZE];
    private final String[] words = new String[MAX_SIZE];

    @Override
    public void addWord(String word) {
        if (word == null) throw new IllegalArgumentException();
        if (word.isBlank()) return;
        if (word.length() > 10)
            word = word.substring(0, 10);
        String lowerWord = word.toLowerCase();

        var index = Math.abs(lowerWord.hashCode() % MAX_SIZE);
        statistics[index]++;
        words[index] = lowerWord;
    }

    @Override
    public List<WordCount> getStatistics() {
        return IntStream.range(0, Math.min(words.length, statistics.length))
                .mapToObj(it -> new WordCount(words[it], statistics[it]))
                .filter(it -> it.getCount() > 0)
                .sorted((left, right) -> Integer.compare(right.getCount(), left.getCount()))
                .sorted(Comparator.comparing(WordCount::getWord)).collect(Collectors.toList());
    }
}
