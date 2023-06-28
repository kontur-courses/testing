import pytest
import sys
from pathlib import Path
sys.path.append(str(Path(__file__).parents[4]))
from python.challenge.incorrect_implementation.do_not_open import WordsStatisticsL3


@pytest.fixture
def words_statistics():
    return WordsStatisticsL3()


from python.challenge.test_words_statistics import TestWordsStatistics
