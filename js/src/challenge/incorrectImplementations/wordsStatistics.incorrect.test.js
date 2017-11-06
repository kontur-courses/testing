import * as incorrectImplementations from "./doNotOpen";

import tests from "../wordsStatistics.test";

const implementations = [
    incorrectImplementations.WordsStatisticsL2,
    incorrectImplementations.WordsStatisticsL3,
    incorrectImplementations.WordsStatisticsL4,
    incorrectImplementations.WordsStatisticsC,
    incorrectImplementations.WordsStatisticsE,
    incorrectImplementations.WordsStatisticsE2,
    incorrectImplementations.WordsStatisticsE3,
    incorrectImplementations.WordsStatisticsE4,
    incorrectImplementations.WordsStatisticsO1,
    incorrectImplementations.WordsStatisticsO2,
    incorrectImplementations.WordsStatisticsO3,
    incorrectImplementations.WordsStatisticsO4,
    incorrectImplementations.WordsStatisticsCR,
    incorrectImplementations.WordsStatisticsSTA,
    incorrectImplementations.WordsStatistics123,
    incorrectImplementations.WordsStatisticsQWE,
    incorrectImplementations.WordsStatistics998,
    incorrectImplementations.WordsStatistics999,
    incorrectImplementations.WordsStatisticsEN1,
    incorrectImplementations.WordsStatisticsEN2,
];

for (const implementation of implementations) {
    describe(implementation.name, tests(() => new implementation()));
}