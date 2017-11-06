import Mocha from "mocha";
import path from "path";

import * as reporters from "./reporters";
import ReportDataCollector from "./reporters/failedTestsCollector";
import FirebaseTestResultsPoster from "./testResultsPoster";

import * as stringHelpers from "./stringHelpers";
import ConsoleWriter from "./consoleWriter";

import { AUTHOR } from "../yourName";

export default class Challenger {
    async run() {
        if (!stringHelpers.isDefinedString(AUTHOR) || stringHelpers.isWhitespace(AUTHOR)){
            ConsoleWriter.writeError("Enter your name at yourName.js");
            return;
        }

        const failedCount = await this.testCorrectImplementation();
        if (failedCount > 0) {
            return;
        }

        const incorrectImplementationTestResult = await this.testIncorrectImplementation();

        await this.postResults(incorrectImplementationTestResult);
    }

    testCorrectImplementation() {
        return new Promise((resolve) => {
            this.createTestsRunner(reporters.CorrectImplementationReporter, "./src/challenge/wordsStatistics.test.js")
                .run((failedCount) => resolve(failedCount));
        });
    }

    testIncorrectImplementation() {
        return new Promise((resolve) => {
            const collector = new ReportDataCollector();

            const reporter = reporters.IncorrectImplementationsReporter;
            const testFileName = "./src/challenge/incorrectImplementations/wordsStatistics.incorrect.test.js";
            const reporterOptions = { custom: ({ collector: collector }) };

            this.createTestsRunner(reporter, testFileName, reporterOptions)
                .run(() => resolve(collector.getStatistics()));
        });
    }

    createTestsRunner(reporter, testFileName, reporterOptions) {
        const fileName = path.resolve(testFileName);

        return new Mocha()
            .ui("bdd")
            .reporter(reporter, reporterOptions)
            .addFile(fileName);
    }

    async postResults(testResult) {
        const firebasePoster = new FirebaseTestResultsPoster();
        const values = testResult.reduce((r, v) => ({
            ...r,
            [v.implementationName]: v.failedTests.length
        }), {});

        try {
            await firebasePoster.writeAsync(AUTHOR, { data: values });
        } catch (error) {
            console.error(error)
        }
    }
}