document.addEventListener("DOMContentLoaded", function () {
    const buttonPublish = document.getElementById("button-publish");
    if (!buttonPublish) throw "button-publish element not found";
    buttonPublish.onclick = buttonPublishClick;
});

function buttonPublishClick(e) {
    const articleText = document.getElementById("article-text");
    if (!articleText) throw "article-text element not found";
    const txt = articleText.value;
    console.log(txt);
}