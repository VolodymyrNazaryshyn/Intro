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
    // формируем шапку таблицы
    let headerTable = "<tr><th>Title</th><th>Description</th></tr>";
    // инициализируем непосредственно таблицу
    let table = "";

    for (let topic of j) {
        // заполняем таблицу топиками
        table += `<tr data-id='${topic.id}'><td>${topic.title}</td><td>${topic.description}</td></tr>`;
    }
    // складываем елементы таблицы воедино
    elem.innerHTML = `<table id='topics'>${headerTable}${table}</table>`;
}
