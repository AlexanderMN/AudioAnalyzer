var inputButton = document.getElementById("button-input");
var filesButton = document.getElementById("button-files");
var requestsButton = document.getElementById("button-requests");
var main = document.getElementById("main");

function changeMainArea(newAreaHTMLFileName){
    $("#main").load("/Audio/" + newAreaHTMLFileName );
}

function filesButtonClick() {
    changeMainArea("Files")
}

function requestsButtonClick() {
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

function inputButtonClick(){
    changeMainArea("Input");
}

inputButton.onclick = inputButtonClick;
filesButton.onclick = filesButtonClick;
requestsButton.onclick = requestsButtonClick;

