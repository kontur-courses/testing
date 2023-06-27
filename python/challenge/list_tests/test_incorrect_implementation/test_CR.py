import pytest
import sys
from pathlib import Path
sys.path.append(str(Path(__file__).parents[4]))
from python.challenge.incorrect_implementation.do_not_open import WordsStatisticsCR


@pytest.fixture
def words_statistics():
    return WordsStatisticsCR()


from python.challenge.test_words_statistics import TestWordsStatistics
