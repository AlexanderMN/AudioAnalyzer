let rows = document.querySelectorAll("#file-table tr");
let checkboxes = document.querySelectorAll(".file-checkbox");
const popup = document.getElementById("popup");
const requestForm = document.getElementById("request-form")
const fileCount = document.getElementById("file-count");

let checkedCount = 0;

checkboxes.forEach((checkbox, index) => {
    checkbox.onchange = () => {
        if (checkbox.checked) {
            checkedCount++;
            fileCount.innerHTML = `выбрано файлов: ${checkedCount}`;
            popup.style.display = "block";
        }
        else {
            checkedCount--;
            fileCount.innerHTML = `выбрано файлов: ${checkedCount}`;
            if (checkedCount === 0) {
                popup.style.display = "none";
            }
        }
    }
});

requestForm.addEventListener("submit", async (e) => {
    e.preventDefault();

    var fileIds = [];

    checkboxes.forEach((checkbox, index) =>{
        if (checkbox.checked){
            var cells = rows.item(index +1 ).cells;
            var fileId = cells.item(0).innerHTML;
            fileIds.push(fileId)
        }
    });

    let task

    switch(e.submitter.defaultValue){
        case "Транскрибация":
            task = 'Transcribe';
            break;
        case "Поиск":
            task = 'Search';
            break;
        case "Суммаризация":
            task = "Summary";
            break;

    }
    let arrStr = fileIds.join(',');
    let request = `/Audio/${task}?fileIds=${arrStr}`;
    var response = await fetch(request);
    
    if (response.ok) {
        alert("Запрос создан")
    }
    else {
        alert(response.message);
    }
})