export default function ArgumentNullError(message) {
    this.name = 'ArgumentNullError';
    this.message = message || "";
    this.stack = (new Error()).stack;
}
ArgumentNullError.prototype = Object.create(Error.prototype);
ArgumentNullError.prototype.constructor = ArgumentNullError;
