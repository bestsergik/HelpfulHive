window.insertImageFromClipboard = async function (editor) {
    try {
        const clipboardItems = await navigator.clipboard.read();
        for (const clipboardItem of clipboardItems) {
            for (const type of clipboardItem.types) {
                if (type.startsWith('image/')) {
                    const blob = await clipboardItem.getType(type);
                    const reader = new FileReader();
                    reader.onload = function (e) {
                        const img = new Image();
                        img.onload = function () {
                            const maxScreenWidth = 1920 * 0.6;
                            const maxScreenHeight = 1080 * 0.6;

                            let width = img.width;
                            let height = img.height;

                            if (width > maxScreenWidth) {
                                height *= maxScreenWidth / width;
                                width = maxScreenWidth;
                            }
                            if (height > maxScreenHeight) {
                                width *= maxScreenHeight / height;
                                height = maxScreenHeight;
                            }

                            const resizedImg = document.createElement('img');
                            resizedImg.src = e.target.result;
                            resizedImg.style.maxWidth = '100%';
                            resizedImg.style.height = 'auto';
                            editor.appendChild(resizedImg);

                            const newLine = document.createElement('br');
                            editor.appendChild(newLine);

                            // После вставки изображения переместим курсор после него
                            const range = document.createRange();
                            range.setStartAfter(resizedImg);
                            range.collapse(true);
                            const selection = window.getSelection();
                            selection.removeAllRanges();
                            selection.addRange(range);
                        };
                        img.src = e.target.result;
                    };
                    reader.readAsDataURL(blob);
                } else if (type === 'text/plain') {
                    const textBlob = await clipboardItem.getType(type);
                    const text = await new Promise((resolve, reject) => {
                        const reader = new FileReader();
                        reader.onload = () => resolve(reader.result);
                        reader.onerror = reject;
                        reader.readAsText(textBlob);
                    });

                    if (document.activeElement === editor) {
                        const selection = window.getSelection();
                        if (selection.rangeCount > 0) {
                            const range = selection.getRangeAt(0);
                            range.deleteContents();
                            const textNode = document.createTextNode(text);
                            range.insertNode(textNode);

                            const newLine = document.createElement('br');
                            range.insertNode(newLine);

                            range.setStartAfter(textNode);
                            range.collapse(true);
                            selection.removeAllRanges();
                            selection.addRange(range);
                        }
                    } else {
                        editor.appendChild(document.createTextNode(text));
                        editor.appendChild(document.createElement('br'));
                    }
                }
            }
            editor.scrollTop = editor.scrollHeight;
        }
    } catch (err) {
        console.error('Failed to read from clipboard', err);
    }
};


function getContentText(element) {
    return element.innerHTML;
}


window.editorFunctions = {
    handlePaste: function (dotNetReference, editorId) {
        var editor = document.getElementById(editorId);
        if (editor) {
            // Удаляем предыдущий обработчик, если он был добавлен
            editor.removeEventListener('paste', this.pasteHandler);

            // Обработчик события вставки5
            this.pasteHandler = async function (event) {
                var pasteContent = (event.clipboardData || window.clipboardData).getData('text');
                await dotNetReference.invokeMethodAsync('HandlePaste', editorId, pasteContent);
            };

            // Добавляем обработчик события вставки
            editor.addEventListener('paste', this.pasteHandler);
        }
    }
};



document.addEventListener('keydown', function (e) {
    if (e.ctrlKey && e.key === 'f') {
        e.preventDefault();
        document.getElementById('searchInput').focus();
    }
});
