from itertools import combinations_with_replacement

import pytest

from python.challenge.word_statistics import WordCount, WordsStatistics


class TestWordsStatistics:

    @pytest.fixture
    def words_statistics(self):
        return WordsStatistics()

    @pytest.mark.parametrize('length', (1, 2, 3, 4, 5, 6, 7, 8, 9))
    def test_statistics_when_word_is_from_one_to_nine(self, words_statistics, length):
        words_statistics.add_word("a" * length)

        assert words_statistics.get_statistics() == [WordCount("a" * length, 1)]

    def test_statistics_when_word_lower_and_not(self, words_statistics):
        words_statistics.add_word("aaa")
        words_statistics.add_word("AAA")

        assert words_statistics.get_statistics() == [WordCount("aaa", 2)]

    def test_statistics_when_word_is_empty(self, words_statistics):
        words_statistics.add_word("")

        assert words_statistics.get_statistics() == []

    @pytest.mark.parametrize('length', (1, 2, 3, 4, 5, 6, 7, 8, 9, 10))
    def test_statistics_when_word_is_space(self, words_statistics, length):
        words_statistics.add_word(" " * length)

        assert words_statistics.get_statistics() == []

    def test_statistics_when_word_is_none(self, words_statistics):
        with pytest.raises(ValueError):
            words_statistics.add_word(None)

    def test_statistics_spaces_checked_before_cut(self, words_statistics):
        words_statistics.add_word("          a")

        assert words_statistics.get_statistics() == [WordCount(" " * 10, 1)]

    def test_statistics_ordering(self, words_statistics):
        for i in range(3):
            words_statistics.add_word("abd")
            words_statistics.add_word("abc")
        for i in range(2):
            words_statistics.add_word("aab")
            words_statistics.add_word("aac")

        words_statistics.add_word("aad")
        words_statistics.add_word("aae")

        assert words_statistics.get_statistics() == [
            WordCount("abc", 3),
            WordCount("abd", 3),
            WordCount("aab", 2),
            WordCount("aac", 2),
            WordCount("aad", 1),
            WordCount("aae", 1),
        ]

    @pytest.mark.timeout(2)
    def test_get_statistics_when_a_lot_of_calls(self, words_statistics):
        result = []
        combinations = list(combinations_with_replacement(list('abcdefg'), 7))
        num = len(combinations)
        for comb in combinations:
            comb = "".join(comb)
            for i in range(num):
                words_statistics.add_word(comb)
            result.append(WordCount(word=comb, count=num))
            num -= 1

        assert words_statistics.get_statistics() == result

    def test_get_statistics_when_russian_letters(self, words_statistics):
        russ_lower = "абвгдеёжзиклмнопрстуфхцчшщъыьэюя"
        russ_upper = "АБВГДЕЁЖЗИКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ"
        words_statistics.add_word(russ_lower)
        words_statistics.add_word(russ_upper)

        result = words_statistics.get_statistics()
        assert len(result) == 1
        assert result[0] == WordCount("абвгдеёжзи", 2)

    @pytest.mark.timeout(2)
    def test_get_statistics_when_a_lot_of_same_words(self, words_statistics):
        word = "abcdefg"
        for i in range(40000):
            words_statistics.add_word(word)

        assert words_statistics.get_statistics() == [WordCount(word, 40000)]

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

    def test_get_statistics_result_changed_after_get_statistics(self, words_statistics):
        words_statistics.add_word("abc")
        assert words_statistics.get_statistics() == [WordCount("abc", 1)]
        words_statistics.add_word("def")
        assert words_statistics.get_statistics() == [WordCount("abc", 1), WordCount("def", 1)]
