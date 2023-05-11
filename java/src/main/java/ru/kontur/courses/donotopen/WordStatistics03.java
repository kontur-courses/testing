package ru.kontur.courses.donotopen;

import ru.kontur.courses.WordCount;
import ru.kontur.courses.WordStatisticImpl;

import java.util.Comparator;
import java.util.List;
import java.util.stream.Collectors;
import java.util.stream.StreamSupport;

public class WordStatistics03 extends WordStatisticImpl {
    @Override
    public List<WordCount> getStatistics() {
        var statistics = super.getStatistics();
        return StreamSupport.stream(statistics.spliterator(), false).sorted(
                Comparator.comparing(WordCount::getWord)
        ).collect(Collectors.toList());
    }
}
