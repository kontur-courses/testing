export default function zip(firstArray, secondArray, selector) {
    const resultArray = [];

    if (firstArray.length === 0) {
        return resultArray;
    }

    firstArray.forEach((value, index) => {
        if (secondArray[index] === undefined) {
            return;
        }

        resultArray.push(selector(value, secondArray[index]))
    });

    return resultArray;
};