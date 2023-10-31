window.insertImageFromClipboard = async function (editor, event) {
    try {
        event.preventDefault();

        const clipboardItems = await navigator.clipboard.read();
        for (const clipboardItem of clipboardItems) {
            for (const type of clipboardItem.types) {
                if (type.startsWith('image/')) {
                    const blob = await clipboardItem.getType(type);
                    const reader = new FileReader();
                    reader.onload = function (e) {
                        const img = document.createElement('img');
                        img.src = e.target.result;
                        editor.appendChild(img);
                    };
                    reader.readAsDataURL(blob);
                    return;
                }
            }
        }
    } catch (err) {
        console.error('Failed to read from clipboard', err);
    }
};

function getContentText(element) {
    return element.innerHTML;
}
