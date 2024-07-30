function checkDisabled() {
    let requiresLoginInput = document.getElementById("FormSettings_RequiresLogin");
    let hasLimitPerUserInput = document.getElementById("FormSettings_HasLimitOneResponsePerUser");
    if (requiresLoginInput.checked === true) {
        hasLimitPerUserInput.disabled = false;
    } else {
        hasLimitPerUserInput.disabled = true;
        hasLimitPerUserInput.checked = false;
    }
}