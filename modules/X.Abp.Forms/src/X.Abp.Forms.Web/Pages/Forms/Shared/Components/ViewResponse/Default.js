function setFormResponseData(){
    const formResponseId = document.getElementById("edit-div").getAttribute("data-id");
    localStorage.setItem("formResponseId", formResponseId);
}

function clearStorage(){
    localStorage.clear();
}