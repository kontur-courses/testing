import pytest

from python.challenge.word_statistics import WordCount


def get_params():
    return ['WordsStatistics123', 'WordsStatistics998', 'WordsStatistics999', 'WordsStatisticsC', 'WordsStatisticsE',
            'WordsStatisticsE2', 'WordsStatisticsE3', 'WordsStatisticsE4', 'WordsStatisticsEN1', 'WordsStatisticsEN2',
            'WordsStatisticsL2', 'WordsStatisticsL3', 'WordsStatisticsL4', 'WordsStatisticsO1', 'WordsStatisticsO2',
            'WordsStatisticsO3', 'WordsStatisticsO4', 'WordsStatisticsQWE']


class TestWordsStatistics:

    @pytest.fixture(params=get_params())
    def words_statistics(self, request):
        import importlib
        mod = importlib.import_module("python.challenge.incorrect_implementation.do_not_open")
        return getattr(mod, request.param)()

    def test_get_statistics_is_empty_after_creation(self, words_statistics):
        assert words_statistics.get_statistics() == []

    def test_get_statistics_contains_item_after_addition(self, words_statistics):
        words_statistics.add_word("abc")
        assert words_statistics.get_statistics() == [WordCount("abc", 1)]

    def test_get_statistics_contains_items_after_get_statistics(self, words_statistics):
        words_statistics.add_word("abc")
        assert words_statistics.get_statistics() == [WordCount("abc", 1)]
        assert len(words_statistics.get_statistics()) == 1

    def test_get_statistics_contains_many_items_after_addition_of_different_words(self, words_statistics):
        words_statistics.add_word("abc")
        words_statistics.add_word("def")
        assert len(words_statistics.get_statistics()) == 2

    def test_statistics_when_word_is_bigger_than_10(self, words_statistics):
        words_statistics.add_word("a" * 11)

        assert words_statistics.get_statistics() == [WordCount("a" * 10, 1)]

