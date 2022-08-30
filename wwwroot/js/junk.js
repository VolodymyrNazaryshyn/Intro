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
                table += tpl
                    .replaceAll("{{moment}}", formatDateIfDateToday(new Date(article.deleteMoment)))
                    .replaceAll("{{topic}}", article.topic.title)
                    .replaceAll("{{text}}", `${article.text.substring(0, 15)}${(article.text.length > 15 ? "..." : "")}`);
            }
            junk.innerHTML = `<table class='junkTable'>${headerTable}${table}</table>`;
            onArticleLoaded();
        });
});

function onArticleLoaded() {
    for (let ins of document.querySelectorAll(".junkTable tr td:last-child")) {
        ins.onclick = insClick;
    }
}

function insClick(e) {
    console.log(e.target);
}