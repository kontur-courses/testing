package ru.kontur.courses.donotopen;

import ru.kontur.courses.WordCount;
import ru.kontur.courses.WordStatisticImpl;

import java.util.Comparator;
import java.util.List;
import java.util.stream.Collectors;

public class WordStatistics01 extends WordStatisticImpl {
    @Override
    public List<WordCount> getStatistics() {
        return statistics.entrySet().stream().map(it -> new WordCount(it.getKey(), it.getValue()))
                .sorted(Comparator.comparing(WordCount::getWord)).collect(Collectors.toList());
    }
}
