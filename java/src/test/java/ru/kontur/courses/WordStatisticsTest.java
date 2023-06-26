package ru.kontur.courses;

import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertTrue;


public class WordStatisticsTest {
    /**
     * Подставляются разные имплементации при прогоне IncorrectImplementation, по умолчанию reference
     */
    static WordStatisticFactory wordStatisticFactory = WordStatisticImpl::new;

    private WordStatistics wordStatistic;

    @BeforeEach
    public void setUp() {
        wordStatistic = wordStatisticFactory.create();
    }

    @Test
    public void getStatisticsIsEmptyAfterCreation() {
        assertTrue(wordStatistic.getStatistics().isEmpty());
    }

    @Test
    public void getStatisticsContainerItemAfterAddition() {
        wordStatistic.addWord("abc");
        assertTrue(wordStatistic.getStatistics().contains(new WordCount("abc", 1)));
    }

    @Test
    public void getStatisticsContainsManyItemsAfterAdditionOfDifferentWords() {
        wordStatistic.addWord("abc");
        wordStatistic.addWord("def");
        assertEquals(2, wordStatistic.getStatistics().size());
    }
}
