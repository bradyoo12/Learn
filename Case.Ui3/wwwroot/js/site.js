window.scrollTextAreaToBottom = function (textAreaElement) {
    if (textAreaElement) {
        textAreaElement.scrollTop = textAreaElement.scrollHeight;
    }
};
