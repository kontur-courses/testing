package ru.kontur.courses;

public interface WordStatistics {
    void addWord(String word);
    Iterable<WordCount> getStatistics();
}
