document.addEventListener("DOMContentLoaded", () => {
    const junk = document.querySelector("junk");
    if (!junk) throw "Container <junk> not found";
    junk.innerHTML = "Тут будут удаленные сообщения";

    var tplPromise = fetch("/templates/junk.html");

    fetch("/api/article?del=true")
        .then(r => r.json())
        .then(async j => {
            const headerTable = "<tr><th>Deleted moment</th><th>Topic</th><th>Text</th><th>Restore</th></tr>";
            const tpl = await tplPromise.then(r => r.text());
            let table = "";
            for (let article of j) {
                junk.setAttribute("data-user-id", article.authorId);
                table += tpl
                    .replaceAll("{{moment}}", formatDateIfDateToday(new Date(article.deleteMoment)))
                    .replaceAll("{{topic}}", article.topic.title)
                    .replaceAll("{{text}}", `${article.text.substring(0, 15)}${(article.text.length > 15 ? "..." : "")}`)
                    .replaceAll("{{id}}", article.id);
            }
            junk.innerHTML = `<table class='junkTable'>${headerTable}${table}</table>`;
            if (typeof j[0] !== 'undefined') {
                junk.setAttribute("data-user-id", j[0].authorId);
            }
            onArticleLoaded();
        });
});

function onArticleLoaded() {
    for (let ins of document.querySelectorAll(".junkTable tr td:last-child")) {
        ins.onclick = insClick;
    }
}

function insClick(e) {
    const uid = e.target.closest('.junkTable tr').getAttribute('data-id');
    const junk = document.querySelector("junk");
    fetch("/api/article?uid=" + uid, {
        method: "PURGE",
        headers: {
            "User-Id": junk.getAttribute('data-user-id')
        }
    }).then(r => r.json())
        .then(j => {
            // console.log(j);
            if (j.message == "Ok") { // успешно восстановлена
                // а) обновить страницу либо б) удалить (скрыть) блок с публикацией
            }
            else { // ошибка восстановления (на бэке)
                alert(j.message);
            }
        });
}