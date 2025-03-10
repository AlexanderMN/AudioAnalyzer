var audioBufferPromise = audioContext.decodeAudioData(arrayBuffer);
var fileName = document.getElementById("fileName");
var fileExtension = document.getElementById("fileExtension");
var duration = document.getElementById("duration");
var length = document.getElementById("length");
var sampleRate = document.getElementById("sampleRate");
var numberOfChannels = document.getElementById("numberOfChannels");

let fileParams = audioFile.name.split(".");
let extension = fileParams.pop();
let name = fileParams;

audioBufferPromise.then(
    audioBufer =>
    {
        fileName.innerHTML = name;
        fileExtension.innerHTML = extension;
        duration.innerHTML = audioBufer.duration;
        length.innerHTML = audioBufer.length;
        sampleRate.innerHTML = audioBufer.sampleRate;
        numberOfChannels.innerHTML = audioBufer.numberOfChannels;
    }
)