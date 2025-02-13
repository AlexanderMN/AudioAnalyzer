//in-browser
const audioContext = new AudioContext();
let audioFile;
let arrayBuffer;
let audio;


let uploadedFileId;


let transcribedText;
let textForSearch;

let transcribedTextContainer;
let textForSearchContainer;




let fileHubConnection;


$(document).ready(function () {
    $("#main").load("/api/Home/Audio/Input");
    
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
        .withUrl("/hubs/fileUpload?fileId=" + uploadedFileId)
        .build();

    fileHubConnection.on("FileText", function (user, message) {
        if (transcribedTextContainer instanceof HTMLElement) {
            transcribedTextContainer.innerHTML = message;
        }
    });
    
    fileHubConnection.on("FileTextForSearch", function (user, message) {
        textForSearch = message;
    })

    fileHubConnection.onclose(async () => {
        await startFileHubConnection();
    });
}
