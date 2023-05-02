package ru.kontur.courses.donotopen;

import ru.kontur.courses.WordCount;
import ru.kontur.courses.WordStatistics;

import java.util.*;
import java.util.stream.Collectors;

public class WordStatistics999 implements WordStatistics {
    private final Set<String> usedWords = new HashSet<>();
    private final List<WordCount> statistics = new ArrayList<>();

    @Override
    public void addWord(String word) {
        if (word == null) throw new IllegalArgumentException();
        if (word.isBlank()) return;
        if (word.length() > 10)
            word = word.substring(0, 10);
        word = word.toLowerCase();
        if (usedWords.contains(word)) {
            final var matchedWord = word;
            var stat = statistics.stream().filter(it -> it.getWord().equals(matchedWord)).findFirst().orElse(null);
            statistics.remove(stat);
            statistics.add(new WordCount(stat.getWord(), stat.getCount() + 1));
        } else {
            statistics.add(new WordCount(word, 1));
            usedWords.add(word);
        }
    }

    @Override
    public Iterable<WordCount> getStatistics() {
        return statistics.stream().sorted((left, right) -> Integer.compare(right.getCount(), left.getCount()))
                .sorted(Comparator.comparing(WordCount::getWord)).collect(Collectors.toList());
    }
}
