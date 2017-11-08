import Challenger from "./infrastructure/challenger";

(async function() {
    try {
        await new Challenger().run();
    } catch(error) {
        console.error(error);
    }
})();