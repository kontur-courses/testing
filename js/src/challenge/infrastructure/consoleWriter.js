import colors from "colors";

class ConsoleWriter {
    static writeSuccess(message) {
        ConsoleWriter.write(message.green)
    }

    static writeError(message) {
        ConsoleWriter.write(message.red)
    }

    static write(message) {
        console.log(message);
    }
}

export default ConsoleWriter;