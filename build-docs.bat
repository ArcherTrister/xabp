:: docfx init -q
:: docfx metadata ../X.Abp.sln
:: apicleaner.exe _api
:: docfx docfx.json
:: docfx serve _site
docfx ..\docfx.json --serve