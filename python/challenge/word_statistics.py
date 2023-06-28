from typing import List, Tuple
from collections import OrderedDict


class WordCount:
    def __init__(self, word: str, count: int) -> None:
        self.word = word
        self.count = count

    @staticmethod
    def create(pair: Tuple[str, int]) -> 'WordCount':
        return WordCount(pair[0], pair[1])

    def __eq__(self, other: str) -> bool:
        if not isinstance(other, WordCount):
            return False
        return (self.word, self.count) == (other.word, other.count)


class WordsStatistics:
    def __init__(self):
        self.statistics = OrderedDict()

    def add_word(self, word: str) -> None:
        if word is None:
            raise ValueError("Word cannot be None")
        if not word.strip():
            return
        if len(word) > 10:
            word = word[:10]
        word = word.lower()
        self.statistics[word] = self.statistics.get(word, 0) + 1

    def get_statistics(self) -> List[WordCount]:
        return [WordCount.create(item) for item in sorted(self.statistics.items(), key=lambda x: (-x[1], x[0]))]
