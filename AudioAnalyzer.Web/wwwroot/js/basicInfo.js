var audioBufferPromise = audioContext.decodeAudioData(arrayBuffer);

var fileName = document.getElementById("fileName");
var fileExtension = document.getElementById("fileExtension");
var duration = document.getElementById("duration");
var length = document.getElementById("length");
var sampleRate = document.getElementById("sampleRate");
var numberOfChannels = document.getElementById("numberOfChannels");

audioBufferPromise.then(
    audioBufer =>
    {
        duration.innerHTML = audioBufer.duration;
        length.innerHTML = audioBufer.length;
        sampleRate.innerHTML = audioBufer.sampleRate;
        numberOfChannels.innerHTML = audioBufer.numberOfChannels;
    }
)