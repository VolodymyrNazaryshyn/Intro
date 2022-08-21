document.addEventListener("DOMContentLoaded", function () {
    const buttonPublish = document.getElementById("button-publish");
    if (buttonPublish) buttonPublish.onclick = buttonPublishClick;
    loadArticles();
});

function buttonPublishClick(e) {
    const articleText = document.getElementById("article-text");
    if (!articleText) throw "article-text element not found";
    const picture = document.querySelector("input[name=picture]");
    if (!articleText) throw "picture element not found";

    const txt = articleText.value;
    const authorId = articleText.getAttribute("data-author-id");
    const topicId = articleText.getAttribute("data-topic-id");

    //console.log(txt, authorId, topicId, picture.files);
    const formData = new FormData();
    formData.append('TopicId', topicId);
    formData.append('AuthorId', authorId);
    formData.append('Text', txt);
    if (picture.files.length > 0) {
        formData.append('Picture', picture.files[0]);
    }
    fetch("/api/article", {
        method: "POST",
        body: formData
    }).then(r => r.json())
        .then(j => {
            if (j.status == "Ok") {
                articleText.value = ""; // Clear message text
                loadArticles();
            }
            else alert(j.message);
        });
}

function loadArticles() {
    const articles = document.querySelector("articles");
    if (!articles) throw "articles element not found";

    let tplPromise = fetch("/templates/article.html");

    const id = articles.getAttribute("topic-id");
    fetch(`/api/article/${id}`)
        .then(r => r.json())
        .then(async j => {
            console.log(j);
            var html = "";
            const tpl = await tplPromise.then(r=>r.text());
            for (let article of j) {
                const moment = new Date(article.createdDate);
                html += tpl
                    .replaceAll("{{author}}",
                        (article.author.id == article.topic.authorId ? `${article.author.realName} TC` : article.author.realName))
                    .replaceAll("{{text}}", article.text)
                    .replaceAll("{{avatar}}",
                        (article.author.avatar == null ? "no-avatar.png" : article.author.avatar))
                    .replaceAll("{{date}}", moment.toLocaleString("uk-UA"))
                    .replaceAll("{{articlePicture}}",
                        (article.pictureFile != null ? `<img src='/img/articleImg/${article.pictureFile}' style='height: 5em; width: 4em; display: block; float: left; margin: 4px 10px 2px 0px; border: 1px solid gray; padding: 3px'>` : ""))
                    .replaceAll("{{id}}", article.id);
            }
            articles.innerHTML = html;
            onArticlesLoaded();
        });
}

function onArticlesLoaded() {
    // span - reply
    for (let span of document.querySelectorAll(".article span")) {
        span.onclick = replyClick;
    }
}

function replyClick(e) {
    console.log(e.target);
}