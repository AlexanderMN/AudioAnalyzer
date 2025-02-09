let connection = new signalR.HubConnectionBuilder()
    .withUrl("/hubs/fileUpload")
    .build();

let transcribedText = document.getElementById("transcribed-text");
connection.on("FileTranscribed", function (user, message) {
    transcribedText.innerHTML = message;
});
async function start() {
    try {
        await connection.start();
        console.log("SignalR Connected.");
    } catch (err) {
        console.log(err);
        setTimeout(start, 5000);
    }
}

connection.onclose(async () => {
    await start();
});

start();