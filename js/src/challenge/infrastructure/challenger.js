import Mocha from "mocha";
import path from "path";

import * as reporters from "./reporters";
import ReportDataCollector from "./reporters/failedTestsCollector";
import FirebaseTestResultsPoster from "./testResultsPoster";

import * as stringHelpers from "../../lib/stringHelpers";
import ConsoleWriter from "./consoleWriter";

import { AUTHORS } from "../yourName";

export default class Challenger {
    async run() {
        if (!stringHelpers.isDefinedString(AUTHORS) || stringHelpers.isWhitespace(AUTHORS)){
            ConsoleWriter.writeError("Enter your surnames at yourName.js in AUTHORS constant");
        }

        ConsoleWriter.write("Check all tests pass with correct implementation...");
        const failedCount = await this.testCorrectImplementation();
        if (failedCount > 0) {
            return;
        }
        const incorrectImplementationTestResult = await this.testIncorrectImplementation();

        if (stringHelpers.isDefinedString(AUTHORS) || !stringHelpers.isWhitespace(AUTHORS)) {
            await this.postResults(incorrectImplementationTestResult);
        }
    }

    testCorrectImplementation() {
        return new Promise((resolve) => {
            const reporter = reporters.CorrectImplementationReporter;
            const testFileName = "./src/challenge/wordsStatistics.test.js";

            this.createTestsRunner(reporter, testFileName)
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
            await firebasePoster.writeAsync(AUTHORS, { data: values });
        } catch (error) {
            console.error(error)
        }
    }
}