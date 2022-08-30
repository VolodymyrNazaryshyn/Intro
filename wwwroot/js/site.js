// форматирование даты (если сегодня - то выводим время, иначе дату и время)
function formatDateIfDateToday(someDate) {
    const today = new Date();
    // если дата сегодняшняя - true
    const isDateToday = someDate.getDate() == today.getDate()
        && someDate.getMonth() == today.getMonth()
        && someDate.getFullYear() == today.getFullYear();

    return isDateToday ? someDate.toLocaleTimeString("uk-UA")
        : someDate.toLocaleString("uk-UA");
}