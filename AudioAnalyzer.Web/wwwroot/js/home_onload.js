const audioContext = new AudioContext();
let audioFile;
let arrayBuffer;
let audio;
let uploadedFileId;
$(document).ready(function () {
    $("#main").load("/api/Home/Audio/Input");
    
    audio = document.querySelector("audio")

    // Example: Add custom play/pause buttons
    
});
