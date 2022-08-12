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
    //// инициализируем непосредственно таблицу
    //let table = "<table id='topics'><tr><th>Title</th><th>Description</th></tr>";

    //// 1. Формируем строку из скольки угодно частей, но один раз - HTML
    //// 2. Желательно разделить разметку и данные, не смешивать
    ////    шаблон HTML и имена переменных -- отдельно HTML, отдельно данные
    //// 2.1. Это позволяет работать над разметкой отдельно (не заставляем дизайнеров "залезать" в код)
    //// 2.2. А также вынести шаблон из кода и загрузить его в AJAX
    //let trTemplate = "<tr><td>*title</td><td>*descr</td></tr>";

    //for (let topic of j) {
    //    // заполняем таблицу топиками
    //    table += trTemplate
    //        .replace("*title", topic.title)
    //        .replace("*descr", topic.description);
    //}

    //table += "</table>";
    //elem.innerHTML = table;

    // запрашиваем шаблон с сервера
    // ~ var trTemplate = `...`;

    fetch("/templates/topic.html")
        .then(r => r.text())
        .then(trTemplate => {
            var appHtml = "";
            for (let topic of j) {
                appHtml +=
                    trTemplate
                        .replace("{{title}}", topic.title)
                        .replace("{{description}}", topic.description)
                        .replace("{{id}}", topic.id);
            }

            elem.innerHTML = appHtml;

            let topicCollection = document.getElementsByClassName("topic"); // получаем все элементы по имени класса
            for (let i = 0; i < topicCollection.length; i++) {
                topicCollection[i].addEventListener('click', // для каждого топика устанавливаем обработчик клика
                    () => { alert(`${j[i].title} id: ${j[i].id}`); }); // по клику на топик в alert выводим его id
            }
        });

    
}
