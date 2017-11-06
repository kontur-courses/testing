import mocha from "mocha";

import ConsoleWriter from "../consoleWriter";

export default function IncorrectImplementationsReporter(runner, options) {
    mocha.reporters.Base.call(this, runner);

    const testSuitesResults = [];
    let failedTests = [];

    const { collector } = options.reporterOptions.custom;

    runner.on('suite', function() {
        failedTests = [];
    });

    runner.on('fail', function(test, error) {
        failedTests.push({
            name: test.titlePath().slice(-1)[0],
            error: error
        });
    });

    runner.on('suite end', function(suite) {
        if (suite.root === false) {
            const { title: implementationName } = suite;

            const failedTestNames = failedTests.map((t) => t.name);
            const failedTestNamesString = failedTestNames.join(", ");
            testSuitesResults.push({
                failed: failedTests.length > 0,
                failedTestNames: failedTestNamesString,
                implementationName: implementationName
            });

            collector.append(implementationName, failedTestNames);
        }
    });

    runner.on('end', function() {
        testSuitesResults.forEach((r) => {
            if (r.failed === true) {
                ConsoleWriter.writeSuccess(`${r.implementationName}\tfails on: ${r.failedTestNames}`);
            } else {
                ConsoleWriter.writeError(`${r.implementationName}\twrite tests to kill it`);
            }
        })
    })
}