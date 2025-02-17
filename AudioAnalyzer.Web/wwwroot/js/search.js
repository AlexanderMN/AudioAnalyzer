let userTextInput = document.getElementById("search-container-input");
let searchResultTable = document.getElementById("search-result-table");

function arrayContainsWords(array, words, startIndex){

    for (let i = 0; i < words.length; i++){
        if (array[i + startIndex] !== words[i]){
            return false;
        }
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
            i+= searchWords.length;
        }
    }
    
    return indices;
}

userTextInput.onsubmit = async function (evt) {
    evt.preventDefault();
    
    let inputText = userTextInput.value;
    
    let response = textForSearch.r[0].response[0];
    let words = response.words.map(p => p.word);

    
    
    let searchWords = inputText.trim().toLowerCase().split(" ");
    let indexes = getIndicesOf(searchWords, words);
    
    for (let i = 0; i < indexes.length; i++){
        let tableRow = document.createElement("tr");
        
        tableRow.insertCell(0);
        tableRow.insertCell(1);
        
        let numberOfExtraWords = 5;
        
        let highLightStartIndex = indexes[i]
        let highLightEndIndex = indexes[i] + searchWords.length;
        
        let extraWordsAtStart = 
            highLightStartIndex > numberOfExtraWords ?
            highLightStartIndex - numberOfExtraWords : 0;
        
        let extraWordsAtEnd =
            highLightEndIndex < indexes.length - 1 - numberOfExtraWords ? 
            highLightEndIndex + numberOfExtraWords : indexes.length - 1;
        
        let startIndex = highLightStartIndex + extraWordsAtStart;
        let endIndex = highLightEndIndex + extraWordsAtEnd;
                
        let wordsSlice = response.words.slice(startIndex, endIndex);
        let startTime = wordsSlice[0].start;
        let endTime = wordsSlice.at(-1).end;
        
        let textToShow = words.slice(startIndex, endIndex).join(" ");
        
        tableRow.cells[0].innerHTML = "начало: " + startTime + "\n" + "конец: " + endTime;
        tableRow.cells[1].innerHTML = textToShow;
        
        
        searchResultTable.appendChild(tableRow);
    }
    
    console.log(indexes);
}

