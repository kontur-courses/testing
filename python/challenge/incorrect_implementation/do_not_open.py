from operator import attrgetter
from typing import Dict, List

from python.challenge.word_statistics import WordsStatistics, WordCount


class WordsStatisticsL2(WordsStatistics):

    def add_word(self, word: str) -> None:
        if word is None:
            raise ValueError("Word cannot be None")
        if word.isspace():
            return 
        word = word.lower()
        self.statistics[word] = self.statistics.get(word, 0) + 1


class WordsStatisticsL3(WordsStatistics):

    def add_word(self, word: str) -> None:
        if word is None:
            raise ValueError("Word cannot be None")
        if word.isspace():
            return
        if len(word) > 10:
            word = word[:10]
        elif len(word) > 5:
            word = word[:len(word) - 2]
        self.statistics[word] = self.statistics.get(word, 0) + 1


class WordsStatisticsL4(WordsStatistics):

    def add_word(self, word: str) -> None:
        if word is None:
            raise ValueError("Word cannot be None")
        if word.isspace():
            return 
        if len(word) - 1 > 10:
            word = word[:10]
        word = word.lower()
        self.statistics[word] = self.statistics.get(word, 0) + 1


class WordsStatisticsC(WordsStatistics):

    def add_word(self, word: str) -> None:
        if word is None:
            raise ValueError("Word cannot be None")
        if word.isspace():
            return 
        if len(word) > 10:
            word = word[:10]
        word = word.lower()
        if word not in self.statistics:
            self.statistics[word] = 0
        self.statistics[word] += 1


class WordsStatisticsE(WordsStatistics):

    def add_word(self, word: str) -> None:
        if not word or not word.strip():
            raise ValueError("Word cannot be None or empty")
        if len(word) > 10:
            word = word[:10]
        word = word.lower()
        self.statistics[word] = self.statistics.get(word, 0) + 1


class WordsStatisticsE2(WordsStatistics):

    def add_word(self, word: str) -> None:
        if word is None or not word.strip():
            return
        if len(word) > 10:
            word = word[:10]
        word = word.lower()
        self.statistics[word] = self.statistics.get(word, 0) + 1


class WordsStatisticsE3(WordsStatistics):
    def add_word(self, word: str) -> None:
        if word is None:
            raise ValueError("Word cannot be None")
        if len(word) > 10:
            word = word[:10]
        if not word.strip():
            return
        word = word.lower()
        self.statistics[word] += 1


class WordsStatisticsE4(WordsStatistics):
    def add_word(self, word: str) -> None:
        if len(word) == 0 or all(ch.isspace() for ch in word):
            return
        if len(word) > 10:
            word = word[:10]
        word = word.lower()
        self.statistics[word] = self.statistics.get(word, 0) + 1


class WordsStatisticsO1(WordsStatistics):
    def get_statistics(self) -> List[WordCount]:
        return [WordCount.create(item) for item in sorted(self.statistics.items(), key=lambda x: x[0])]


class WordsStatisticsO2(WordsStatistics):
    def get_statistics(self) -> List[WordCount]:
        return [WordCount.create(item) for item in sorted(super().get_statistics(), key=lambda x: x[1])]


class WordsStatisticsO3(WordsStatistics):
    def get_statistics(self) -> List[WordCount]:
        stat = super().get_statistics()
        stat.sort(key=lambda x: x.word)
        return stat


class WordsStatisticsO4(WordsStatistics):
    def get_statistics(self) -> List[WordCount]:
        return [WordCount.create(item) for item in sorted(self.statistics.items(), key=lambda x: (x[0], -x[1]))]


class WordsStatistics123:
    MAX_SIZE = 1237

    def __init__(self):
        self.statistics = [0] * WordsStatistics123.MAX_SIZE
        self.words = [''] * WordsStatistics123.MAX_SIZE

    def add_word(self, word: str) -> None:
        if word is None:
            raise ValueError("Word cannot be None")
        if word.isspace():
            return 
        if len(word) > 10:
            word = word[:10]
        index = abs(hash(word.lower())) % WordsStatistics123.MAX_SIZE
        self.statistics[index] += 1
        self.words[index] = word.lower()

    def get_statistics(self) -> List[WordCount]:
        return [WordCount(self.words[i], self.statistics[i]) for i in range(WordsStatistics123.MAX_SIZE) if
                self.statistics[i] > 0]


class WordsStatisticsQWE(WordsStatistics):

    def add_word(self, word: str) -> None:
        if word is None:
            raise ValueError("Word cannot be None")
        if word.isspace():
            return 
        if len(word) > 10:
            word = word[:10]
        word = self._to_lower(word)
        count = self.statistics[word]
        self.statistics[word] = count + 1

    def get_statistics(self) -> List[WordCount]:
        return [WordCount.create(item) for item in
                sorted(self.statistics.items(), key=attrgetter('count', 'word'), reverse=True)]

    def _to_lower(self, s: str) -> str:
        return s.translate(str.maketrans('QWERTYUIOPLJKHGFDSAZXCVBNMЙЦУКЕНГШЩЗФЫВАПРОЛДЯЧСМИТЬ',
                                         'qwertyuioplkjhgfdsazxcvbnmйцукенгшщзфывапролдячсмить'))


class WordsStatistics998(WordsStatistics):
    def __init__(self):
        self.statistics: List[WordCount] = []

    def add_word(self, word: str) -> None:
        if word is None:
            raise ValueError("Word cannot be None")
        if word.isspace():
            return 
        if len(word) > 10:
            word = word[:10]
        lower_case_word = word.lower()
        word_count = next((w for w in self.statistics if w.word == lower_case_word), None)
        if word_count is not None:
            self.statistics.remove(word_count)
        else:
            word_count = WordCount(lower_case_word, 0)
        self.statistics.append(WordCount(word_count.word, word_count.count - 1))
        self.statistics.sort(key=attrgetter('count', 'word'))

    def get_statistics(self) -> List[WordCount]:
        return [WordCount(w.word, -w.count) for w in super().get_statistics()]


class WordsStatistics999(WordsStatistics):
    def __init__(self):
        self.used_words: set[str] = set()
        self.statistics: List[WordCount] = []

    def add_word(self, word: str) -> None:
        if word is None:
            raise ValueError("Word cannot be None")
        if word.isspace():
            return 
        if len(word) > 10:
            word = word[:10]
        word = word.lower()
        if word in self.used_words:
            stat = next((w for w in self.statistics if w.word == word), None)
            if stat is not None:
                self.statistics.remove(stat)
                self.statistics.append(WordCount(stat.word, stat.count + 1))
        else:
            self.statistics.append(WordCount(word, 1))
            self.used_words.add(word)

    def get_statistics(self) -> List[WordCount]:
        return sorted(super().get_statistics(), key=attrgetter('count', 'word'), reverse=True)


class WordsStatisticsEN1(WordsStatistics):
    def __init__(self):
        self.statistics: Dict[str, int] = {}

    def add_word(self, word: str) -> None:
        if word is None:
            raise ValueError("Word cannot be None")
        if word.isspace():
            return 
        if len(word) > 10:
            word = word[:10]
        word = word.lower()
        self.statistics[word] = self.statistics.get(word, 0) + 1

    def get_statistics(self) -> List[WordCount]:
        result = [WordCount.create(item) for item in
                  sorted(self.statistics.items(), key=lambda x: (x[1], x[0]), reverse=True)]
        self.statistics.clear()
        return result


class WordsStatisticsEN2(WordsStatistics):
    def __init__(self):
        super().__init__()
        self.result: List[WordCount] = None

    def get_statistics(self) -> List[WordCount]:
        if self.result is None:
            self.result = super().get_statistics()
        return self.result
