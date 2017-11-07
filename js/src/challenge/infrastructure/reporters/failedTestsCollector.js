export default class FailedTestsCollector {
    constructor() {
        this.statistics = new Map();
    }

    append(implementationName, failedTests) {
        this.statistics.set(implementationName, failedTests);
    }

    getStatistics() {
        return Array.from(this.statistics)
            .map((keyValue) => ({
                failedTests: keyValue[1],
                implementationName: keyValue[0]
            }))
            .sort((a, b) => {
                const compare = a.implementationName.localeCompare(b.implementationName);
                return compare !== 0 ? compare : a.failedTests - b.failedTests;
            });
    }
}