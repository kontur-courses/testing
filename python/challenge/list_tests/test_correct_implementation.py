import pytest
import sys
from pathlib import Path
sys.path.append(str(Path(__file__).parents[3]))
from python.challenge.word_statistics import WordsStatistics


@pytest.fixture
def words_statistics():
    return WordsStatistics()


from python.challenge.test_words_statistics import TestWordsStatistics
