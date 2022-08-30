document.addEventListener("DOMContentLoaded", () => {
    const junk = document.querySelector("junk");
    if (!junk) throw "Container <junk> not found";
    junk.innerHTML = "Тут будут удаленные сообщения";

    fetch("/api/article?del=true")
        .then(r => r.json())
        .then(j => {
            // console.log(j);
            let html = "";
            const tpl = "<p>{{moment}} {{topic}} {{text}} <ins>&#x21ED;</ins></p>";
            for (let article of j) {
                html += tpl.replaceAll("{{moment}}", article.deleteMoment)
                    .replaceAll("{{topic}}", article.topic.title)
                    .replaceAll("{{text}}", article.text.substring(0, 15));
            }
            junk.innerHTML = html;
            onArticleLoaded();
        });
});

function onArticleLoaded() {
    for (let ins of document.querySelectorAll("p ins")) {
        ins.onclick = insClick;
    }
}

function insClick(e) {
    console.log(e.target);
}