const alphabet = ["A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N",
    "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z"];
let acceptableFails = 0;
let clue = "";
let guessedClue = "";


function showClue() {
    document.getElementById("clue").innerHTML = guessedClue;
}

function start(clueForGuess) {
    clue = clueForGuess.toUpperCase();
    for (i = 0; i < clue.length; i++) {
        if (clue.charAt(i) === " ") guessedClue += " ";
        else guessedClue += "-";
    }
    let alphabetContent = "";
    for (i = 0; i < alphabet.length; i++) {
        let divId = "letter" + i;
        alphabetContent += `<div class="letter letterUnverified col-sm-2" onclick="checkLetter(${i})" id="${divId}">${alphabet[i]}</div>`;
    }
    document.getElementById("alphabet").innerHTML = `<div class=row>${alphabetContent}</div>`;
    showClue();
}

function checkLetter(nr) {
    let found = false;
    for (i = 0; i < clue.length; i++) {
        if (clue.charAt(i) === alphabet[nr]) {
            guessedClue = setChar(guessedClue, i, alphabet[nr]);
            found = true;
        }
    }
    let divId = "letter" + nr;
    if (found === true) {
        document.getElementById(divId).classList.remove("letterUnverified");
        document.getElementById(divId).classList.add("letterSuccess");
        showClue();
    } else {
        acceptableFails++;
        document.getElementById(divId).classList.remove("letterUnverified");
        document.getElementById(divId).classList.add("letterFail");
        document.getElementById(divId).setAttribute("onclick", ";");
        document.getElementById("hangingTree").innerHTML = `<img src="/hanging-tree/img/s${acceptableFails}.jpg" alt="tree" class="imageTree"/>`;
    }

    if (guessedClue === clue) {
        document.getElementById("alphabet").innerHTML = `Nice one! <br/>
        <span class="reset" onclick="location.reload()">Once again?</span>`;
    }

    if (acceptableFails >= 9) {
        document.getElementById("alphabet").innerHTML = `Sorry... <br/>
        <span class="reset" onclick="location.reload()">Once again?</span>`;
    }
}

function setChar(stringToChange, location, charToChange) {
    if (location > stringToChange.length - 1) return stringToChange.toString();
    else return stringToChange.substr(0, location) + charToChange + stringToChange.substr(location + 1);
}