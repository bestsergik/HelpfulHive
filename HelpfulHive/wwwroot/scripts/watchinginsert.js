function handlePaste(editor, dotnetReference) {
    editor.onDidPaste(() => {
        dotnetReference.invokeMethodAsync('FormatXml');
    });
}
