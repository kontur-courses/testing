import FirebaseClient from "firebase-client";
import dateFormat from "dateformat"

import { toUriSafeString } from "./stringHelpers";

export default class FirebaseTestResultsPoster {
    constructor() {
        this.firebase = new FirebaseClient({
            url: "https://testing-challenge.firebaseio.com/"
        })
    }

    writeAsync(author, data) {
        const safeAuthor = toUriSafeString(author);

        const now = new Date();
        const postData = {
            time: now.toISOString(),
            ...data
        };

        return this.firebase.set(
            `word-statistics-js/${dateFormat(now, "yyyymmdd")}/${safeAuthor}`,
            postData
        );
    }
}