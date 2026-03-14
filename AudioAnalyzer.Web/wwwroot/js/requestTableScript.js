SingletonFactory.setInstance("hiddenRows", document.querySelectorAll(".hidden-row"));

document.querySelectorAll(".default-row").forEach((displayedRow, index) => {
    var hiddenRows = SingletonFactory.getInstance("hiddenRows");
    displayedRow.onclick = () => {
        if (hiddenRows.item(index).style.display === "none" || hiddenRows.item(index).style.display === "") {
            hiddenRows.item(index).style.display = "table-row"
            hiddenRows.item(index).style.opacity = '0';
            hiddenRows.item(index).style.transition = 'opacity 0.3s ease';

            setTimeout(() => {
                hiddenRows.item(index).style.opacity = '1';
            }, 10);
        }
        else{
            hiddenRows.item(index).style.display = "none";
        }

        SingletonFactory.setInstance("hiddenRows", hiddenRows);
    }
});

function getRequestResult(requestPath){
    $("#main").load("/Audio/" + requestPath)
}

document.querySelectorAll(".expand-button").forEach((expandButton) => {
    expandButton.onclick = async () => {
        let expandedRow = expandButton.closest("tr")
        let expandedTable = expandButton.closest(".expanded-table")
        let cells = expandedTable.parentNode.parentNode.previousElementSibling.cells;
        switch (cells.item(1).innerHTML){
            case "Транскрибация":
                await getRequestResult(`Transcribe?requestId=${cells.item(0).innerHTML}&fileId=${expandedRow.cells.item(0).innerHTML}`);
                break;
            case "Суммаризация":
                await getRequestResult(`Summary?requestId=${cells.item(0).innerHTML}&fileId=${expandedRow.cells.item(0).innerHTML}`);
                break;
            case "Поиск":
                await getRequestResult(`Search?requestId=${cells.item(0).innerHTML}&fileId=${expandedRow.cells.item(0).innerHTML}`);
                break;
            case "Классификация":
                await getRequestResult(`Classify?requestId=${cells.item(0).innerHTML}&fileId=${expandedRow.cells.item(0).innerHTML}`);
                break;
        } 
    }
})


