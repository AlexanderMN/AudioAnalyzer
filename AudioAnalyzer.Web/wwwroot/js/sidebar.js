const spectrogramButton = document.getElementById("button-spectrogram");
const basicInfoButton = document.getElementById("button-basicinfo");
const transcribeButton = document.getElementById("button-transcribe");
const summaryButton = document.getElementById("button-summary");
const searchButton = document.getElementById("button-search");
const main = document.getElementById("main");

function changeMainArea(newAreaHTMLFileName){
    $("#main").load(newAreaHTMLFileName);
}

function spectrogramButtonClick(){
    changeMainArea("spectrogram.html");
}

function basicinfoButtonClick(){
    changeMainArea("basicInfo.html");
}

function transcribeButtonClick(){

}

function summaryButtonClick(){
    changeMainArea("summary.html");
}

function searchButtonClick(){
    changeMainArea("search.html");
}


spectrogramButton.onclick = spectrogramButtonClick;
basicInfoButton.onclick = basicinfoButtonClick;
transcribeButton.onclick = transcribeButtonClick;
summaryButton.onclick = summaryButtonClick;
searchButton.onclick = searchButtonClick;

