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

    const id = articles.getAttribute("topic-id");
    fetch(`/api/article/${id}`)
        .then(r => r.json())
        .then(j => {
            console.log(j);
            var html = "";
            const tpl = `<div style='border: 2px solid lightgray; box-shadow: 0 0 4px 5px lightgray; margin-bottom: 1em;
                         padding: 10px; border-radius: 5%; overflow: auto; background-color:antiquewhite'>
                             <div style='display: block; float: left'>
                                 <img src='/img/userImg/{{avatar}}' style='height:8ch; width:8ch; 
                                 border:1px solid grey; border-radius: 50%; background: white' />
                             </div>
                             <div style='float: none; overflow: auto; padding-left: 10px'>
                                 <b>{{author}}</b><br/> {{date}}
                                 <hr style='border-color:grey'>
                             </div>
                             {{articlePicture}}
                             <div style='float: none; overflow: auto'>
                                 <p>{{text}}</p>
                             </div>
                         </div>`;
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
                        (article.pictureFile != null ? `<img src='/img/articleImg/${article.pictureFile}' style='height:14ch; width:14ch; display: block; float: left'>` : ""));
            }
            articles.innerHTML = html;
        });
}
