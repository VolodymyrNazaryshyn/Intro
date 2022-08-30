document.addEventListener("DOMContentLoaded", function () {
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
    let replyId = articleText.getAttribute("data-reply-id");

    const formData = new FormData();
    formData.append('TopicId', topicId);
    formData.append('AuthorId', authorId);
    formData.append('Text', txt);
    formData.append('ReplyId', replyId);
    if (picture.files.length > 0) {
        formData.append('Picture', picture.files[0]);
    }

    const addArticleSpan = document.getElementById("addArticleSpan");
    const refuseReplyArticle = document.getElementById("refuseReplyArticle");

    fetch("/api/article", {
        method: "POST",
        body: formData
    }).then(r => r.json())
        .then(j => {
            if (j.status == "Ok") {
                articleText.value = ""; // Clear message field
                picture.value = '';
                articleText.setAttribute("data-reply-id", ""); // Clear reply-id
                addArticleSpan.innerText = "Add article:";
                refuseReplyArticle.style.display = "none";
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

    const articleText = document.getElementById("article-text");
    const authorId = (articleText) ? articleText.getAttribute("data-author-id") : "-";

    fetch(`/api/article/${id}`)
        .then(r => r.json())
        .then(async j => {
            const tpl = await tplPromise.then(r => r.text());
            var html = "";
            for (let article of j) {
                html += tpl
                    .replaceAll("{{id}}", article.id)
                    .replaceAll("{{reply-display}}",
                        (isAuthUser == "false" ? "none" : "inline-block"))
                    .replaceAll("{{del-display}}",
                        (article.authorId == authorId ? "inline-block" : "none"))
                    .replaceAll("{{edit-display}}",
                        (isDisplayEdit(j, article.authorId, article.replyId, article.id) ? "inline-block" : "none"))
                    .replaceAll("{{author}}",
                        (article.author.id == article.topic.authorId ? `${article.author.realName} TC` : article.author.realName))
                    .replaceAll("{{avatar}}",
                        (article.author.avatar == null ? "no-avatar.png" : article.author.avatar))
                    .replaceAll("{{date}}", formatDateIfDateToday(new Date(article.createdMoment)))
                    .replaceAll("{{tooltip}}",
                        (article.replyId == null ? "" : `Article: ${article.reply.text}\r\nAuthor: ${article.reply.author.realName}.\r\nCreated moment: ${formatDateIfDateToday(new Date(article.reply.createdMoment))}`))
                    .replaceAll("{{reply}}",
                        (article.replyId == null ? "" : `Reply to "${article.reply.text.substring(0, 15)}${article.reply.text.length > 15 ? "..." : ""}" by <b>${article.reply.author.realName}</b>`))
                    .replaceAll("{{articlePicture}}",
                        (article.pictureFile == null ? "" : `<img id="articlePicture" src='/img/articleImg/${article.pictureFile}'>`))
                    .replaceAll("{{text}}", article.text);
            }
            articles.innerHTML = html;
            onArticlesLoaded(j);
        });
}

function isDisplayEdit(j, artAuthorId, artReplyId, artId) {
    const articleText = document.getElementById("article-text");
    const authorId = (articleText) ? articleText.getAttribute("data-author-id") : "-";

    // если статья "своя" (её написал этот пользователь)
    if (artAuthorId == authorId) {
        // если на статью есть ответ
        for (let article of j) {
            if (artReplyId == article.id) {
                return true; // display: inline-block
            }
            if (artId == article.replyId) {
                return false; // display: none
            }
        }
        // если на статью нет ответов
        if (artReplyId == null) {
            return true; // display: inline-block
        }
    }
    // если статья "не своя" (её не писал этот пользователь)
    else {
        return false; // display: none
    }
}

function onArticlesLoaded(j) {
    const articleText = document.getElementById("article-text");
    if (!articleText) throw "article-text element not found";
    const refuseReplyArticle = document.getElementById("refuseReplyArticle");
    const addArticleSpan = document.getElementById("addArticleSpan");

    // reply-article button: element span
    for (let span of document.querySelectorAll(".article span")) {
        span.onclick = function(e) {
            const replyId = e.target.closest(".article").getAttribute("data-id");

            articleText.setAttribute("data-reply-id", replyId);
            articleText.focus(); // Set focus on a text-area
            
            refuseReplyArticle.style.display = "inline-block";

            for (let article of j) {
                if (article.id == replyId) {
                    addArticleSpan.innerText = `Reply to "${article.text.substring(0, 15)}${article.text.length > 15 ? "..." : ""}" by ${article.author.realName}:`;
                }
            }
        };
    }

    // delete-article button: element del
    for (let del of document.querySelectorAll(".article del")) {
        del.onclick = deleteClick; // Создаем обработчик нажатия на кнопку "удалить" публикацию
    }

    refuseReplyArticle.addEventListener("click", function() {
        articleText.setAttribute("data-reply-id", ""); // Clear reply-id
        addArticleSpan.innerText = "Add article:";
        refuseReplyArticle.style.display = "none";
    });
}

function deleteClick(e) {
    const id = e.target.closest(".article").getAttribute("data-id");
    if (confirm(`Delete article ${id}?`)) {
        //console.log(id); // Получаем и выводим на консоль id публикации (на удаление)
        fetch(`/api/article/${id}`, {
            method: "DELETE",
        }).then(r => r.json())
            .then(j => {
                if (j.status == "Ok") {
                    e.target.closest(".article").style.display = 'none';
                }
            });
    }
}