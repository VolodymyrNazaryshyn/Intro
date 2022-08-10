// Событие "DOMContentLoaded" подается тогда когда готов DOM
document.addEventListener("DOMContentLoaded", () => {
    // const app = document.getElementById("app"); // по ID <div id="app"></div>
    const app = document.querySelector("app"); // по имени тега <app><app>
    if (!app) throw "Forum script: APP not found";
    // app.innerHTML = "APP will be here";
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
    // elem.innerHTML = "topics will be here";
    for (let topic of j) {
        elem.innerHTML += `<div class='topic' data-id='${topic.id}'>
        <b>${topic.title}</b><i>${topic.description}</i></div>`;
    }
}
