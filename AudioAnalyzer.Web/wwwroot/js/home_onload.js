//in-browser

var SingletonFactory = (function (){
    var instances = new Map()
    return {
        getInstance : function (variable){
            name = Object.keys(variable)[0];
            if (!instances.has(name)) {
                instances.set(name, {});
            }
            return instances.get(name);
        },
        setInstance : function (variable, value){
            name = Object.keys(variable)[0];
            instances.set(name, value);
        }
    }
})();

var audioContext = new AudioContext();
var audioFile;
var arrayBuffer;
var textForSearch = {r: [{
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

var transcribedTextContainer;
var textForSearchContainer;
var fileHubConnection;


$(document).ready(function () {
    $("#main").load("/Audio/Input");
    
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
