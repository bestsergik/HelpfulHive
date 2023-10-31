window.insertImage = function (editor, url) {
    var img = document.createElement("img");
    img.src = url;
    editor.appendChild(img);
};

window.getContent = function (editor) {
    return editor.innerHTML;
};
