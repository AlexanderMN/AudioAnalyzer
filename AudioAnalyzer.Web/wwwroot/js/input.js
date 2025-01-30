const dropArea = document.getElementById("input-drop-area");
const fileInput = document.getElementById("input-file-input");
const optionButtons = document.getElementsByClassName("options-button");
const fileInputBuffer = null;

dropArea.ondragover = dropArea.ondragenter = function (evt) {
    evt.preventDefault();
};

function _arrayBufferToBase64( buffer ) {
    var binary = '';
    var bytes = new Uint8Array( buffer );
    var len = bytes.byteLength;
    for (var i = 0; i < len; i++) {
        binary += String.fromCharCode( bytes[ i ] );
    }
    return window.btoa( binary );
}

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

    for (button of optionButtons) {
        button.disabled = false;
    }

    readInput();
    evt.preventDefault();
};


