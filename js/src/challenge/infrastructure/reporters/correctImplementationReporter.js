import mocha from "mocha";

import ConsoleWriter from "../consoleWriter";

export default function CorrectImplementationReporter(runner) {
    mocha.reporters.Base.call(this, runner);

    const failedTests = [];

    runner.on('fail', function(test, err){
        failedTests.push({name: test.titlePath().slice(-1), error: err});
    });

    runner.on('end', function() {
        if (failedTests.length > 0) {
            const failedTestNames = failedTests.map((t) => t.name).join(", ");
            ConsoleWriter.writeError(`Incorrect test${failedTests.length === 1 ? "" : "s"} detected: ${failedTestNames}`);
        } else {
            ConsoleWriter.writeSuccess("Tests are OK!");
        }
    });
}