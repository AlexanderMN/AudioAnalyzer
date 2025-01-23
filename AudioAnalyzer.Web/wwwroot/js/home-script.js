document.addEventListener("DOMContentLoaded", function () {
    const dropArea = document.getElementById("dropArea");
    const fileInput = document.getElementById("fileInput");
    dropArea.ondragover = dropArea.ondragenter = function (evt) {
        evt.preventDefault();
    };

    dropArea.ondrop = function (evt) {
        // pretty simple -- but not for IE :(
        fileInput.files = evt.dataTransfer.files;

        // If you want to use some of the dropped files
        const dT = new DataTransfer();

        for (const file of evt.dataTransfer.files) {
            dT.items.add(file);
        }
        fileInput.files = dT.files;

        evt.preventDefault();
    };
});