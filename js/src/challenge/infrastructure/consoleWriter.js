const terminalCodes = {
    green: '\x1b[32m',
    red: '\x1b[31m',
    reset: '\x1b[0m'
};

class ConsoleWriter {
    static writeSuccess(message) {
        console.log(terminalCodes.green, message, terminalCodes.reset);
    }

    static writeError(message) {
        console.log(terminalCodes.red, message, terminalCodes.reset);
    }

    static write(message) {
        console.log(message);
    }
}

export default ConsoleWriter;