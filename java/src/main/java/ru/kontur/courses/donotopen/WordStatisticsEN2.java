package ru.kontur.courses.donotopen;

import ru.kontur.courses.WordCount;
import ru.kontur.courses.WordStatisticImpl;

import java.util.List;

public class WordStatisticsEN2 extends WordStatisticImpl {
    private List<WordCount> result;

    @Override
    public List<WordCount> getStatistics() {
        return result != null ? result : super.getStatistics();
    }
}
