package ru.kontur.courses.solved;

import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.Timeout;
import ru.kontur.courses.WordCount;
import ru.kontur.courses.WordStatisticFactory;
import ru.kontur.courses.WordStatisticImpl;
import ru.kontur.courses.WordStatistics;

import static org.junit.jupiter.api.Assertions.*;

public class WordStatisticsSolved {
    public static WordStatisticFactory wordStatisticFactory = WordStatisticImpl::new;

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

    @Test
    public void getStatisticsSortsWordsByFrequency() {
        wordStatistic.addWord("aaaaaaaaaa");
        wordStatistic.addWord("bbbbbbbbbb");
        wordStatistic.addWord("bbbbbbbbbb");
        var statistic = wordStatistic.getStatistics().stream().map(WordCount::getWord).toList();

        assertArrayEquals(new String[]{"bbbbbbbbbb", "aaaaaaaaaa"}, statistic.toArray());
    }

    @Test
    public void getStatisticsSortsWordsByAbcWhenFrequenciesAreSame() {
        wordStatistic.addWord("cccccccccc");
        wordStatistic.addWord("aaaaaaaaaa");
        wordStatistic.addWord("bbbbbbbbbb");

        var statistic = wordStatistic.getStatistics().stream().map(WordCount::getWord).toList();

        assertArrayEquals(new String[]{"aaaaaaaaaa", "bbbbbbbbbb", "cccccccccc"}, statistic.toArray());
    }

    @Test
    public void getStatisticsReturnsSameResultOnSecondCall() {
        wordStatistic.addWord("abc");
        var result1 = wordStatistic.getStatistics();
        assertArrayEquals(new WordCount[]{new WordCount("abc", 1)}, result1.toArray());

        var result2 = wordStatistic.getStatistics();
        assertArrayEquals(new WordCount[]{new WordCount("abc", 1)}, result2.toArray());
    }

    @Test
    public void getStatisticsBuildsResultOnEveryCall() {
        wordStatistic.addWord("abc");
        assertEquals(1, wordStatistic.getStatistics().size());

        wordStatistic.addWord("def");
        assertEquals(2, wordStatistic.getStatistics().size());
    }

    @Test
    public void addWordAllowsShortWords() {
        wordStatistic.addWord("aaa");
    }

    @Test
    public void addWordCountsOnceWhenSameWord() {
        wordStatistic.addWord("aaaaaaaaaa");
        wordStatistic.addWord("aaaaaaaaaa");

        assertEquals(1, wordStatistic.getStatistics().size());
    }

    @Test
    public void addWordIncrementsCounterWhenSameWord() {
        wordStatistic.addWord("aaaaaaaaaa");
        wordStatistic.addWord("aaaaaaaaaa");

        assertEquals(2, wordStatistic.getStatistics().get(0).getCount());
    }

    @Test
    public void addWordThrowsWhenWordIsNull() {
        assertThrows(IllegalArgumentException.class, () -> {
            wordStatistic.addWord(null);
        });
    }

    @Test
    public void addWordIgnoresEmptyWord() {
        wordStatistic.addWord("");

        assertTrue(wordStatistic.getStatistics().isEmpty());
    }

    @Test
    public void addWordIgnoresWhitespaceWords() {
        wordStatistic.addWord("   ");

        assertTrue(wordStatistic.getStatistics().isEmpty());
    }

    @Test
    public void addWordCutsWordsLongerThan10() {
        wordStatistic.addWord("12345678901");

        assertEquals("1234567890", wordStatistic.getStatistics().get(0).getWord());
    }

    @Test
    public void addWordCutsWordsJoined() {
        wordStatistic.addWord("12345678901");
        wordStatistic.addWord("1234567890");

        assertEquals(2, wordStatistic.getStatistics().get(0).getCount());
    }

    @Test
    public void addWordAllowsWordAndCutItToWhitespacesWhenWordPrecededByWhitespaces() {
        wordStatistic.addWord("          a");

        assertEquals("          ", wordStatistic.getStatistics().get(0).getWord());
    }

    @Test
    public void addWordIsCaseIntensive() {
        var counter = 0;
        for (char c = 'a'; c <= 'z'; c++) {
            wordStatistic.addWord(String.valueOf(c));
            wordStatistic.addWord(String.valueOf(c).toUpperCase());
            counter++;
        }
        for (char c = 'а'; c <= 'я' || c <= 'ё'; c++) {
            wordStatistic.addWord(String.valueOf(c));
            wordStatistic.addWord(String.valueOf(c).toUpperCase());
            counter++;
        }
        assertEquals(counter, wordStatistic.getStatistics().size());
    }

    @Test
    public void addWordHasNotCollisions() {
        final int wordCount = 500;
        for (int i = 0; i < wordCount; i++) {
            wordStatistic.addWord(String.valueOf(i));
        }
        assertEquals(wordCount, wordStatistic.getStatistics().size());
    }

    @Test
    @Timeout(1500)
    public void addWordHasSufficientPerformanceOnAddingDifferentWords() {
        for (int i = 0; i < 5000; i++) {
            wordStatistic.addWord(String.valueOf(i));
        }
        wordStatistic.getStatistics();
    }

    @Test
    @Timeout(1500)
    public void addWordHasSufficientPerformanceOnAddingSameWord() {
        for (int i = 0; i < 20000; i++) {
            wordStatistic.addWord(String.valueOf(i));
        }
        var sameWord = String.valueOf(9);
        for (int i = 0; i < 20000; i++) {
            wordStatistic.addWord(sameWord);
        }
        wordStatistic.getStatistics();
    }
}
