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






function copyHtmlToClipboard(htmlContent) {
    var tempDiv = document.createElement('div');
    tempDiv.style.position = 'absolute';
    tempDiv.style.left = '-9999px';
    tempDiv.innerHTML = htmlContent;
    document.body.appendChild(tempDiv);

    if (document.selection) {
        var range = document.body.createTextRange();
        range.moveToElementText(tempDiv);
        range.select();
    } else if (window.getSelection) {
        var range = document.createRange();
        range.selectNode(tempDiv);
        window.getSelection().removeAllRanges();
        window.getSelection().addRange(range);
    }

    document.execCommand('copy');
    document.body.removeChild(tempDiv);
}
