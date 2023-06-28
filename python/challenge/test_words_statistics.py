import pytest

from python.challenge.word_statistics import WordCount, WordsStatistics


@pytest.fixture
def words_statistics():
    return WordsStatistics()


class TestWordsStatistics:
    def test_get_statistics_is_empty_after_creation(self, words_statistics):
        assert words_statistics.get_statistics() == []

    def test_get_statistics_contains_item_after_addition(self, words_statistics):
        words_statistics.add_word("abc")
        assert words_statistics.get_statistics() == [WordCount("abc", 1)]

    def test_get_statistics_contains_many_items_after_addition_of_different_words(self, words_statistics):
        words_statistics.add_word("abc")
        words_statistics.add_word("def")
        assert len(words_statistics.get_statistics()) == 2
