(function () {
    const _sendModal = new abp.ModalManager(abp.appPath + 'Forms/SendModal');

    $(document).on("click", "#SendModalBtn", function (el) {
        _sendModal.open({
            id: $("#formInfo").val()
        });
    });
    $(document).on("click", "#Responses-tab", function (el) {
        const id = $("#formInfo").val();
        location.href = `${window.origin}/Forms/${id}/Responses`;
        return false;
    });
    $(document).on("click", "#Questions-tab", function (el) {
        const id = $("#formInfo").val();
        
        location.href = `${window.origin}/Forms/${id}/Questions`;
        
        return false;
    });
})();
