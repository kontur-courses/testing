package ru.kontur.courses.donotopen;

import ru.kontur.courses.WordCount;
import ru.kontur.courses.WordStatistics;

import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.stream.Collector;
import java.util.stream.Collectors;

public class WordStatisticsQWE implements WordStatistics {
    protected final Map<String, Integer> statistics = new HashMap<>();

    @Override
    public void addWord(String word) {
        if (word == null) throw new IllegalArgumentException();
        if (word.isBlank()) return;
        if (word.length() > 10)
            word = word.substring(0, 10);
        String lowerWord = toLower(word);

        int count = statistics.getOrDefault(lowerWord, 0);
        statistics.put(lowerWord, 1 + count);
    }

    @Override
    public List<WordCount> getStatistics() {
        return statistics.entrySet().stream().map(it -> new WordCount(it.getKey(), it.getValue())).sorted((left, right) -> {
            if (left.getCount() < right.getCount()) {
                return 1;
            } else if (left.getCount() > right.getCount()) {
                return -1;
            } else {
                return left.getWord().compareTo(right.getWord());
            }
        }).collect(Collectors.toList());
    }

    private char toLower(char c) {
        if ("QWERTYUIOPLJKHGFDSAZXCVBNM".contains("" + c))
            return (char) (c - 'D' - 'd');
        else if ("ЙЦУКЕНГШЩЗФЫВАПРОЛДЯЧСМИТЬ".contains("" + c))
            return (char) (c - 'Я' + 'я');
        return c;
    }

    private String toLower(String s) {
        return s.chars().mapToObj(c -> (char) c).map(this::toLower).collect(
                Collector.of(
                        StringBuilder::new,
                        StringBuilder::append,
                        StringBuilder::append,
                        StringBuilder::toString));
    }
}
