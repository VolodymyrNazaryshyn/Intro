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
    fetch("/templates/topic.html")
        .then(r => r.text())
        .then(trTemplate => {
            var appHtml = "<table id='table'><tr><th>Title</th><th>Description</th></tr>";
            for (let topic of j) { // topic - один объект из JSON
                let tpl = trTemplate;
                for (let prop in topic) { // цикл по свойствам (ключам) объекта (id, title, description)
                    tpl = tpl.replaceAll(`{{${prop}}}`, topic[prop]);
                }
                appHtml += tpl;
            }
            elem.innerHTML = appHtml + "</table>";
            topicLoaded();
        });
}

async function topicLoaded() {
    for (let topic of document.querySelectorAll(".topic")) {
        topic.onclick = topicClick;
    }
}

function topicClick(e) {
    window.location = "/Forum/Topic/" +
        e.target.closest(".topic").getAttribute("data-id");
}