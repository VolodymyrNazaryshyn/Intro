// Событие "DOMContentLoaded" подается тогда когда готов DOM
document.addEventListener("DOMContentLoaded", () => {
    // const app = document.getElementById("app"); // по ID <div id="app"></div>
    const app = document.querySelector("app"); // по имени тега <app><app>
    if (!app) throw "Forum script: APP not found";
    loadTopics(app);
});

function loadTopics(elem) {
    fetch("/api/topic", // API topics - get all
        {
            method: "GET",
            headers: {
                "User-Id": "",
                "Culture": ""
            },
            body: null
        })
        .then(r => r.json())
        .then(j => {
            if (j instanceof Array) {
                showTopics(elem, j);
            }
            else {
                throw "showTopics: Backend data invalid";
            }
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