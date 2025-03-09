//in-browser
const audioContext = new AudioContext();
let audioFile;
let arrayBuffer;
let audio;

let textForSearch = {r: [{
    response: [{
        text: String,
        words: [{
            word: String,
            start: Number,
            end: Number
        }]
    }]
}
]}

let transcribedTextContainer;
let textForSearchContainer;



let fileHubConnection;


$(document).ready(function () {
    $("#main").load("/api/Audio/Input");
    
    audio = document.querySelector("audio");

    // Example: Add custom play/pause buttons
});

async function startFileHubConnection() {
    try {
        await fileHubConnection.start();
        console.log("SignalR Connected.");
    } catch (err) {
        console.log(err);
        setTimeout(startFileHubConnection, 5000);
    }
}
function initUploadHubConnection() {
    fileHubConnection = new signalR.HubConnectionBuilder()
        .withUrl("/hubs/fileUpload")
        .build();

    fileHubConnection.on("TranscribedText", function (message) {
        console.log("entered TranscribedText");
        if (transcribedTextContainer instanceof HTMLElement) {
            transcribedTextContainer.innerHTML = message;
        }
    });
    
    fileHubConnection.on("TranscribedTextForSearch", function (message) {
        textForSearch = JSON.parse(message);
        console.log("entered TranscribedTextForSearch");
        
    })

    fileHubConnection.onclose(async () => {
        await startFileHubConnection();
    });
}

function setAudioTime(timestamp) {
    audio.currentTime = timestamp;
}
