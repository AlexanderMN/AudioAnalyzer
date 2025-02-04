const dropArea = document.getElementById("input-drop-area");
const fileInput = document.getElementById("input-file-input");
const userSubmitArea = document.getElementById("input-user-input");
const fileInputBuffer = null;

dropArea.ondragover = dropArea.ondragenter = function (evt) {
    evt.preventDefault();
};

function readInput(){
    var reader = new FileReader();
    reader.onload = function() {

        arrayBuffer = this.result;
    }
    reader.readAsArrayBuffer(fileInput.files[0]);
}

fileInput.addEventListener('change', readInput);

dropArea.ondrop = function (evt) {
    // pretty simple -- but not for IE :(
    fileInput.files = evt.dataTransfer.files;

    // If you want to use some of the dropped files
    const dT = new DataTransfer();

    for (const file of evt.dataTransfer.files) {
        dT.items.add(file);
    }

    readInput();
    evt.preventDefault();
};

userSubmitArea.onsubmit = function (evt) {
    audioFile = fileInput.files[0];
    
    let urlObj = URL.createObjectURL(fileInput.files[0]);
    audio.src = urlObj;

    evt.preventDefault();
    
}


