package ru.kontur.courses.donotopen;

import ru.kontur.courses.WordCount;
import ru.kontur.courses.WordStatistics;

import java.util.*;
import java.util.stream.Collectors;

public class WordStatistics998 implements WordStatistics {
    protected final List<WordCount> statistics = new ArrayList<>();

    @Override
    public void addWord(String word) {
        if (word == null) throw new IllegalArgumentException();
        if (word.isBlank()) return;
        if (word.length() > 10)
            word = word.substring(0, 10);
        String lowerWord = word.toLowerCase();
        var wordCount = statistics.stream().filter(it -> it.getWord().equals(lowerWord)).findFirst();
        if (wordCount.isPresent()) {
            statistics.remove(wordCount.get());
        } else {
            wordCount = Optional.of(new WordCount(lowerWord, 0));
        }
        statistics.add(new WordCount(wordCount.get().getWord(), wordCount.get().getCount() - 1));

        statistics.sort((a, b) -> a.getCount() == b.getCount() ? a.getWord().compareTo(b.getWord()) : a.getCount() - b.getCount());
    }

    @Override
    public List<WordCount> getStatistics() {
        return statistics.stream().map(it -> new WordCount(it.getWord(), -it.getCount())).collect(Collectors.toList());
    }
}
