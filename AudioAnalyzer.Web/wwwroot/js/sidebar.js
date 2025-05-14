var main = document.getElementById("main");
function selectButton(button){
    button.style.transition = "marginLeft 0.3s ease";
    button.style.marginLeft = "5%";
    button.style.background = "#c5c5c5"
}

function unselectButton(button){
    button.style.marginLeft = "0";
    button.style.background = "#f5f5f5";
}

selectButton(document.getElementById("button-input"));
function changeMainArea(caller) {
    $("#main").load("/Audio/" + caller.target.name, 
        function (responseText, statusText) {
            if (statusText === "success") {
                document.querySelectorAll(".options-button").forEach(button => {
                    unselectButton(button);
                });
                selectButton(caller.target);
            }
        });
}

document.querySelectorAll(".options-button").forEach(button => {
    button.onclick = changeMainArea
})
