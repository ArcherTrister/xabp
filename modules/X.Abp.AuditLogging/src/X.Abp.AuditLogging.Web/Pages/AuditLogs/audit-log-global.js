(function () {
    var l = abp.localization.getResource("AbpAuditLogging");

    abp.auditLogging = abp.auditLogging || {};

    var _entityHistoryModal = new abp.ModalManager(abp.appPath + 'AuditLogs/EntityHistory');
    _entityHistoryModal.onOpen(function () {
        var timeElements = _entityHistoryModal.getModal().find("time");
        if(timeElements.length > 0) {
            return new bootstrap.Popover(timeElements)
        }
    });

    abp.auditLogging.openEntityHistoryModal = function (entityTypeFullName, entityId) {
        _entityHistoryModal.open({
            entityId: entityId,
            entityTypeFullName: entityTypeFullName
        });
    };
    
    abp.auditLogging.entityHistoryAction = function (entityTypeFullName, entityIdField) {
        return {
            text: l('ChangeDetails'),
            icon: 'history',
            action: function (data) {
                abp.auditLogging.openEntityHistoryModal(
                    entityTypeFullName,
                    data.record[entityIdField]
                );
            }
        };
    };

})();