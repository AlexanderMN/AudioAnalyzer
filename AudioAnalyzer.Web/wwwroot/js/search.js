var searchSubmitForm = document.getElementById("search-submit-form")
var userTextInput = document.getElementById("search-container-input");
var searchResultTable = document.getElementById("search-result-table");
var searchTbody = document.getElementById("search-tbody");
function arrayContainsWords(array, words, startIndex){

    if (words.length === 1){
        return array[startIndex].includes(words[0])
    }

    for (let i = 0; i < words.length; i++){
        if (array[i+ startIndex] === words[i])
            continue;

        if (!array[i + startIndex].includes(words[i])){
            return false;
        }

        if (i !== 0 && i !== words.length - 1)
            return false;

    }

    return true;
}


function getIndicesOf(searchWords, listOfWords) {
    if (searchWords.length === 0) {
        return [];
    }

    if (searchWords.length > searchWords.length) {
        return [];
    }

    let startIndex = 0, index, indices = [];

    for (let i = 0; i < listOfWords.length; i++){

        if (arrayContainsWords(listOfWords, searchWords, i)){
            indices.push(i);
        }
    }

    return indices;
}

searchSubmitForm.onsubmit = async function (evt) {
    evt.preventDefault();
    
    
    
    let inputText = userTextInput.value;

    let response = textForSearch.r[0].response[0];
    let words = response.words.map(p => p.word);

    let searchWords = inputText.toLowerCase().split(/\s+/);
    let indexes = getIndicesOf(searchWords, words);

    for (let i = 0; i < indexes.length; i++){
        let tableRow = document.createElement("tr");
        tableRow.insertCell(0);
        tableRow.insertCell(1);
        
        tableRow.cells[0].setAttribute("style", "width:20%");
        tableRow.cells[1].setAttribute("style", "width:80%");
        
        let numberOfExtraWords = 5;

        let IndexOfFirstWordContainingOccurence = indexes[i]
        let IndexOfLastWordContainingOccurence = indexes[i] + searchWords.length;

        let extraWordsAtStart =
            IndexOfFirstWordContainingOccurence> numberOfExtraWords ?
                numberOfExtraWords : IndexOfFirstWordContainingOccurence;

        let extraWordsAtEnd =
            IndexOfLastWordContainingOccurence + numberOfExtraWords < words.length - 1 ?
                numberOfExtraWords : words.length - 1 - IndexOfLastWordContainingOccurence;

        let startIndex = IndexOfFirstWordContainingOccurence - extraWordsAtStart;
        let endIndex = IndexOfLastWordContainingOccurence + extraWordsAtEnd;

        let firstWord = response.words[IndexOfFirstWordContainingOccurence]
        let startTime = firstWord.start;

        let stringContainingOccurence = words.slice(IndexOfFirstWordContainingOccurence, IndexOfLastWordContainingOccurence).join(" ");

        let correctedInput = searchWords.join(" ");
        let indexOfOccurence = stringContainingOccurence.indexOf(correctedInput);
        let indexOfLastOccurence = indexOfOccurence + correctedInput.length;

        let wordsBeforeHighlight = words.slice(startIndex, IndexOfFirstWordContainingOccurence).join(" ");

        let wordsContainingHighlight = stringContainingOccurence.slice(0, indexOfOccurence)
            + "<mark>"
            + stringContainingOccurence.slice(indexOfOccurence, indexOfLastOccurence)
            + "</mark>"
            + stringContainingOccurence.slice(indexOfLastOccurence);

        let wordsAfterHighlight = words.slice(IndexOfLastWordContainingOccurence, endIndex).join(" ");

        let textToShow = wordsBeforeHighlight.concat(" ", wordsContainingHighlight, " ", wordsAfterHighlight);
        
        tableRow.cells[0].innerHTML = String("<a href = \"javascript:setAudioTime(" + startTime + ");\"> " +
            startTime + "</a>")
        tableRow.cells[1].innerHTML = textToShow;


        searchTbody.appendChild(tableRow);
    }

    console.log(indexes);
}

