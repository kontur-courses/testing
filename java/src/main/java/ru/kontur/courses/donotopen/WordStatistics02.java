package ru.kontur.courses.donotopen;

import ru.kontur.courses.WordCount;
import ru.kontur.courses.WordStatisticImpl;

import java.util.Comparator;
import java.util.stream.Collectors;

public class WordStatistics02 extends WordStatisticImpl {
    @Override
    public Iterable<WordCount> getStatistics() {
        return statistics.entrySet().stream().map(it -> new WordCount(it.getKey(), it.getValue()))
                .sorted(Comparator.comparingInt(WordCount::getCount)).collect(Collectors.toList());
    }
}
