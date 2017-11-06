import Challenger from "./challenger";

(async function() {
    try {
        await new Challenger().run();
    } catch(error) {
        console.error(error);
    }
})();