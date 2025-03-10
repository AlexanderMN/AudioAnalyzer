var inputButton = document.getElementById("button-input");
var spectrogramButton = document.getElementById("button-spectrogram");
var basicInfoButton = document.getElementById("button-basicinfo");
var transcribeButton = document.getElementById("button-transcribe");
var summaryButton = document.getElementById("button-summary");
var searchButton = document.getElementById("button-search");
var main = document.getElementById("main");

function changeMainArea(newAreaHTMLFileName){
    $("#main").load("/api/Audio/" + newAreaHTMLFileName );
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
spectrogramButton.onclick = spectrogramButtonClick;
basicInfoButton.onclick = basicInfoButtonClick;
transcribeButton.onclick = transcribeButtonClick;
summaryButton.onclick = summaryButtonClick;
searchButton.onclick = searchButtonClick;

