﻿document.addEventListener("DOMContentLoaded", function () {
    const buttonPublish = document.getElementById("button-publish-article");
    if (buttonPublish) buttonPublish.onclick = buttonPublishClick;

    loadArticles();
});

function buttonPublishClick(e) {
    const articleText = document.getElementById("article-text");
    if (!articleText) throw "article-text element not found";
    const picture = document.querySelector("input[name=picture]");
    if (!picture) throw "picture element not found";

    const txt = articleText.value;
    const authorId = articleText.getAttribute("data-author-id");
    const topicId = articleText.getAttribute("data-topic-id");

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
                articleText.value = "";
                picture.value = '';
                loadArticles();
            }
            else alert(j.message);
        });
}

function loadArticles() {
    const articles = document.querySelector("articles");
    if (!articles) throw "articles element not found";
    // Variable, responsible for user registration
    const isAuthUser = articles.getAttribute("data-auth-user");
    const id = articles.getAttribute("topic-id");
    var tplPromise = fetch("/templates/article.html");

    fetch(`/api/article/${id}`)
        .then(r => r.json())
        .then(async j => {
            const tpl = await tplPromise.then(r => r.text());
            var html = "";
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
                        (article.pictureFile == null ? "" : `<img id="articlePicture" src='/img/articleImg/${article.pictureFile}'>`))
                    .replaceAll("{{id}}", article.id)
                    .replaceAll("{{articleReply}}",
                        (isAuthUser == "false" ? "" : "<span>&#x2936;</span>"))
                    .replaceAll("{{reply}}",
                        (article.replyId == null ? "" : article.replyId));
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

    // By blur event change the innerText of the element
    document.getElementById("article-text").addEventListener('blur', function () {
        document.getElementById("addArticleSpan").innerText = "Add article:";
    });
}

function replyClick(e) {
    const id = e.target.closest(".article").getAttribute("data-id");
    // Show id article to console
    console.log(id);
    // Set focus on a text-area
    document.getElementById("article-text").focus();
    // Change innerText element
    document.getElementById("addArticleSpan").innerText = `Reply to the article ${id}:`;
}