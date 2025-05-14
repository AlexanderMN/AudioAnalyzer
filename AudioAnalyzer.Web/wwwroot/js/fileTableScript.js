
document.querySelectorAll(".default-row").forEach((row, index) => {
    row.onclick = async () => {
        await fetch(`/Audio/AudioFile?fileId=${row.cells.item(0).innerHTML}`)
            .then(res => res.blob())
            .then(data => {
                audio.src = URL.createObjectURL(data);
                audio.load();
            });
    }
})
SingletonFactory.setInstance("checkedCount", 0);


document.querySelectorAll(".file-checkbox").forEach((checkbox, index) => {
    checkbox.onchange = () => {
        let checkedCount = SingletonFactory.getInstance("checkedCount");
        
        if (checkbox.checked) {
            checkedCount++;
            document.getElementById("file-count").innerHTML = `выбрано файлов: ${checkedCount}`;
            document.getElementById("popup").style.display = "block";
        }
        else {
            checkedCount--;
            document.getElementById("file-count").innerHTML = `выбрано файлов: ${checkedCount}`;
            if (checkedCount === 0) {
                document.getElementById("popup").style.display = "none";
            }
        }
        
        SingletonFactory.setInstance("checkedCount", checkedCount);
    }
});


document.getElementById("request-form").addEventListener("submit", async (e) => {
    e.preventDefault();

    let fileIds = [];

    document.querySelectorAll(".file-checkbox").forEach((checkbox, index) =>{
        if (checkbox.checked){
            var cells = document.querySelectorAll("#file-table tr").item(index +1 ).cells;
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
    function getResponse(){
        return $.ajax({
            url: `/Audio/${task}?`,
            type: "POST",
            data: {fileIds: fileIds},
            async: false
        });
    }
    
    async function processResponse(data, textStatus, jqXHR){
        if (textStatus === "success") {
            alert("Запрос создан")
        }
        else {
            alert("Ошибка создания запроса");
        }
    }
    
    getResponse().done(processResponse);
})