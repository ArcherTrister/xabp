(function () {

    var l = abp.localization.getResource('AbpAuditLogging');

    abp.widgets.AuditLoggingErrorRateWidget = function ($wrapper) {

        var _chart;

        var refresh = function (filters) {
            x.abp.auditLogging.auditLogs.getErrorRate({
                startDate: filters.startDate,
                endDate: filters.endDate
            })
                .then(function (statistic) {
                    _chart.data = {
                        labels: Object.keys(statistic.data),
                        datasets: [
                            {
                                data: Object.values(statistic.data),
                                backgroundColor: [
                                    'rgba(200, 50, 50, 0.7)',
                                    'rgba(50, 200, 50, 0.7)'
                                ]
                            }
                        ]
                    };
                    _chart.update();
                });
        };

        var init = function (filters) {
            _chart = new Chart($wrapper.find('.ErrorRateChart'),
                {
                    type: 'pie',
                    options: {
                        responsive: true,
                        maintainAspectRatio: false,
                    }
                });

            refresh(filters);
        };

        return {
            init: init,
            refresh: refresh
        };
    };
})();
