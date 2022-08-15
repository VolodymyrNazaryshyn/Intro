document.addEventListener("DOMContentLoaded", function () {
    const buttonPublish = document.getElementById("button-publish");
    if (!buttonPublish) throw "button-publish element not found";
    buttonPublish.onclick = buttonPublishClick;
});

function buttonPublishClick(e) {
    const articleText = document.getElementById("article-text");

    if (!articleText) throw "article-text element not found";

    const txt = articleText.value;

    // Собираем и выводим данные из атрибутов data в консоль
    console.log(`Author ID: ${articleText.dataset.authorId}`);
    console.log(`Topic ID: ${articleText.dataset.topicId}`);
    console.log(`Text: ${txt}`);
    console.log(`Creation date: ${articleText.dataset.datetime}`);
}