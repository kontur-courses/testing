package ru.kontur.courses;

import java.util.List;

public interface WordStatistics {
    void addWord(String word);
    List<WordCount> getStatistics();
}
