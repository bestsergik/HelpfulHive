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

                            const canvas = document.createElement('canvas');
                            canvas.width = width;
                            canvas.height = height;
                            const ctx = canvas.getContext('2d');
                            ctx.drawImage(img, 0, 0, width, height);

                            const base64Image = canvas.toDataURL('image/png');
                            const resizedImg = document.createElement('img');
                            resizedImg.src = base64Image;

                            if (document.activeElement === editor) {
                                const selection = window.getSelection();
                                if (selection.rangeCount > 0) {
                                    const range = selection.getRangeAt(0);
                                    range.deleteContents();
                                    range.insertNode(resizedImg);


                                    const newLine = document.createElement('br');
                                    range.insertNode(newLine);

                                    range.setStartAfter(resizedImg);
                                    range.collapse(true);
                                    selection.removeAllRanges();
                                    selection.addRange(range);
                                }
                            } else {
                                editor.appendChild(resizedImg);
                                editor.appendChild(document.createElement('br'));
                            }
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

