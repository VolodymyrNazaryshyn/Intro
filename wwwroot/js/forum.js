document.addEventListener("DOMContentLoaded", () => {
    // const app = document.getElementById("app"); // по ID <div id="app"></div>
    const app = document.querySelector("app"); // по имени тега <app><app>
    if (!app) throw "Forum script: APP not found";
    const buttonPublish = document.getElementById("button-publish-topic");
    if (buttonPublish) buttonPublish.onclick = buttonPublishClick;

    loadTopics(app);
});

function buttonPublishClick(e) {
    const titleText = document.querySelector("input[name=title]");
    if (!titleText) throw "titleText element not found";
    const descriptionText = document.getElementById("topic-text");
    if (!descriptionText) throw "topic-text element not found";

    const txt = descriptionText.value;
    const authorId = descriptionText.getAttribute("data-author-id");

    const formData = new FormData();
    formData.append('Title', titleText.value);
    formData.append('Description', txt);
    formData.append('AuthorId', authorId);

    fetch("/api/topic", {
        method: "POST",
        body: formData
    }).then(r => r.json())
        .then(j => {
            if (j.status == "Ok") {
                titleText.value = "";
                descriptionText.value = "";
                loadTopics(document.querySelector("app"));
            }
            else alert(j.message);
        });
}

function loadTopics(elem) {
    fetch("/api/topic")
        .then(r => r.json())
        .then(j => {
            showTopics(elem, j);
        });
}

function showTopics(elem, j) {
    let headerTable = "<tr><th>Title</th><th>Description</th><th>Author</th></tr>";
    let table = "";
    for (let topic of j) {
        elem.innerHTML += `<div data-id='${topic.id}'>
        <b>${topic.title}</b><i>${topic.description}</i></div>`;
        table += `<tr class='topic' data-id='${topic.id}'><td>${topic.title}</td><td>${topic.description}</td><td>${topic.author.realName}</td></tr>`;
    }
    elem.innerHTML = `<table id='topicTable'>${headerTable}${table}</table>`;
    topicLoaded();
}

async function topicLoaded() {
    for (let topic of document.querySelectorAll(".topic")) {
        topic.onclick = topicClick;
    }
}

function topicClick(e) {
    window.location = `/Forum/Topic/${e.target.closest(".topic").getAttribute("data-id")}`;
}