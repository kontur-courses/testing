export function isDefinedString(word) {
    return word !== undefined && word !== null && typeof word === "string";
}

export function isWhitespace(word) {
    return word.match(/^\s*$/) !== null;
}

// http://werxltd.com/wp/2010/05/13/javascript-implementation-of-javas-string-hashcode-method/
export function calculateHash(word) {
    let hash = 0, i, chr;
    if (word.length === 0) {
        return hash;
    }
    for (i = 0; i < word.length; i++) {
        chr = word.charCodeAt(i);
        hash = ((hash << 5) - hash) + chr;
        hash |= 0; // Convert to 32bit integer
    }
    return hash;
}

export function toUriSafeString(word) {
    return word.replace(/[.$#\[\]\/\u0000-\u0020 ]/g, "_");
}