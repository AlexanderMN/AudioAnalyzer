var inputButton = document.getElementById("button-input");
var filesButton = document.getElementById("button-files");
var requestsButton = document.getElementById("button-requests");
let optionsButtons = document.querySelectorAll(".options-button");
var main = document.getElementById("main");

function selectButton(button){
    button.style.transition = "marginLeft 0.3s ease";
    button.style.marginLeft = "5%";
    button.style.background = "#c5c5c5"
}

function unselectButton(button){
    button.style.marginLeft = "0";
    button.style.background = "#f5f5f5";
}

selectButton(inputButton);
function changeMainArea(caller) {
    $("#main").load("/Audio/" + caller.target.name, 
        function (responseText, statusText, req) {
            if (statusText === "success") {
                optionsButtons.forEach(button => {
                    unselectButton(button);
                });
                selectButton(caller.target);
            }
        });
}

function filesButtonClick() {
    changeMainArea("Files")
    filesButton.style.marginLeft = "5%";
}

function requestsButtonClick(caller) {
    changeMainArea("Requests")
}

function spectrogramButtonClick(){
    changeMainArea("Spectrogram");
}

function basicInfoButtonClick(){
    changeMainArea("BasicInfo");
}

function transcribeButtonClick(){
    changeMainArea("Transcribe");
}

function summaryButtonClick(){
    changeMainArea("Summary");
}

function searchButtonClick(){
    changeMainArea("Search");
}

function inputButtonClick(caller){
    changeMainArea("Input");
}

optionsButtons.forEach(button => {
    button.onclick = changeMainArea
})
