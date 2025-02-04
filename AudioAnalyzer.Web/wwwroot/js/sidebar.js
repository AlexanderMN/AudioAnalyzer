const spectrogramButton = document.getElementById("button-spectrogram");
const basicInfoButton = document.getElementById("button-basicinfo");
const transcribeButton = document.getElementById("button-transcribe");
const summaryButton = document.getElementById("button-summary");
const searchButton = document.getElementById("button-search");
const main = document.getElementById("main");

function changeMainArea(newAreaHTMLFileName){
    $("#main").load("/api/Home/" + newAreaHTMLFileName);
}

function spectrogramButtonClick(){
    changeMainArea("Spectrogram");
}

function basicInfoButtonClick(){
    changeMainArea("BasicInfo");
}

function transcribeButtonClick(){

}

function summaryButtonClick(){
    changeMainArea("Summary");
}

function searchButtonClick(){
    changeMainArea("Search");
}


spectrogramButton.onclick = spectrogramButtonClick;
basicInfoButton.onclick = basicInfoButtonClick;
transcribeButton.onclick = transcribeButtonClick;
summaryButton.onclick = summaryButtonClick;
searchButton.onclick = searchButtonClick;

