function copyTextToClipboard(text) {
    navigator.clipboard.writeText(text).then(function () {
        console.log('Text successfully copied to clipboard');
    })
        .catch(function (err) {
            console.log('Unable to copy text to clipboard', err);
        });
}

function getClipboardText() {
    return navigator.clipboard.readText().then(function (text) {
        console.log('Text successfully read from clipboard');
        return text;
    })
        .catch(function (err) {
            console.log('Unable to read text from clipboard', err);
            return '';
        });
}






