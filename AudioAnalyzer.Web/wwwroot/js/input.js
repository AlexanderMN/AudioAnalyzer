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
    evt.preventDefault();
    
    // pretty simple -- but not for IE :(
    fileInput.files = evt.dataTransfer.files;

    // If you want to use some of the dropped files
    const dT = new DataTransfer();

    for (const file of evt.dataTransfer.files) {
        dT.items.add(file);
    }

    readInput();
};

userSubmitArea.onsubmit = async function (evt) {
    evt.preventDefault();
    let formData = new FormData();
    formData.append("file", fileInput.files[0]);

     let response = await fetch('/api/Home/Audio/Input', {method: "POST", body: formData});
     if (response.ok) {
         
         response.text().then(data => {
             uploadedFileId = data;
         })
         audioFile = fileInput.files[0];
         audio.src = URL.createObjectURL(fileInput.files[0]);
     }
}
