import * as firebase from "firebase";

export default class ResultPoster {
    constructor() {
        const config = {
            databaseURL: "https://testing-challenge.firebaseio.com/"
        };

        firebase.initializeApp(config);
    }

    async writeAsync(author, data) {
        const safeAuthor = this.toUriSafeString(author);
        const path = `word-statistics/${this.buildDateKey()}/${safeAuthor}`
        const postData = {
            implementations: data.data,
            time: new Date().toISOString(),
            lang: 'js',
        };

        const result = await firebase.database().ref(path).set(
            postData,
            err => { if (err) console.log("Error while submitting " + err); }
        );
        firebase.database().goOffline();
        return result;
    }

    buildDateKey() {
        const now = new Date();
        const day = ("0" + now.getDate()).slice(-2);
        const month = ("0" + (now.getMonth() + 1)).slice(-2);
        const year = now.getFullYear();
        return year + month + day;
    }

    toUriSafeString(word) {
        return word.replace(/[.$#\[\]\/\u0000-\u0020 ]/g, "_");
    }
}
