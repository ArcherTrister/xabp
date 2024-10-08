/* This file is automatically generated by ABP framework to use MVC Controllers from javascript. */


// module languageManagement

(function(){

  // controller x.abp.languageManagement.language

  (function(){

    abp.utils.createNamespace(window, 'x.abp.languageManagement.language');

    x.abp.languageManagement.language.getAllList = function(ajaxParams) {
      return abp.ajax($.extend(true, {
        url: abp.appPath + 'api/language-management/languages/all',
        type: 'GET'
      }, ajaxParams));
    };

    x.abp.languageManagement.language.getList = function(input, ajaxParams) {
      return abp.ajax($.extend(true, {
        url: abp.appPath + 'api/language-management/languages' + abp.utils.buildQueryString([{ name: 'filter', value: input.filter }, { name: 'resourceName', value: input.resourceName }, { name: 'baseCultureName', value: input.baseCultureName }, { name: 'targetCultureName', value: input.targetCultureName }, { name: 'getOnlyEmptyValues', value: input.getOnlyEmptyValues }, { name: 'sorting', value: input.sorting }, { name: 'skipCount', value: input.skipCount }, { name: 'maxResultCount', value: input.maxResultCount }]) + '',
        type: 'GET'
      }, ajaxParams));
    };

    x.abp.languageManagement.language.get = function(id, ajaxParams) {
      return abp.ajax($.extend(true, {
        url: abp.appPath + 'api/language-management/languages/' + id + '',
        type: 'GET'
      }, ajaxParams));
    };

    x.abp.languageManagement.language.create = function(input, ajaxParams) {
      return abp.ajax($.extend(true, {
        url: abp.appPath + 'api/language-management/languages',
        type: 'POST',
        data: JSON.stringify(input)
      }, ajaxParams));
    };

    x.abp.languageManagement.language.update = function(id, input, ajaxParams) {
      return abp.ajax($.extend(true, {
        url: abp.appPath + 'api/language-management/languages/' + id + '',
        type: 'PUT',
        data: JSON.stringify(input)
      }, ajaxParams));
    };

    x.abp.languageManagement.language['delete'] = function(id, ajaxParams) {
      return abp.ajax($.extend(true, {
        url: abp.appPath + 'api/language-management/languages/' + id + '',
        type: 'DELETE',
        dataType: null
      }, ajaxParams));
    };

    x.abp.languageManagement.language.setAsDefault = function(id, ajaxParams) {
      return abp.ajax($.extend(true, {
        url: abp.appPath + 'api/language-management/languages/' + id + '/set-as-default',
        type: 'PUT',
        dataType: null
      }, ajaxParams));
    };

    x.abp.languageManagement.language.getResources = function(ajaxParams) {
      return abp.ajax($.extend(true, {
        url: abp.appPath + 'api/language-management/languages/resources',
        type: 'GET'
      }, ajaxParams));
    };

    x.abp.languageManagement.language.getCulturelist = function(ajaxParams) {
      return abp.ajax($.extend(true, {
        url: abp.appPath + 'api/language-management/languages/culture-list',
        type: 'GET'
      }, ajaxParams));
    };

    x.abp.languageManagement.language.getFlagList = function(ajaxParams) {
      return abp.ajax($.extend(true, {
        url: abp.appPath + 'api/language-management/languages/flag-list',
        type: 'GET'
      }, ajaxParams));
    };

  })();

  // controller x.abp.languageManagement.languageText

  (function(){

    abp.utils.createNamespace(window, 'x.abp.languageManagement.languageText');

    x.abp.languageManagement.languageText.getList = function(input, ajaxParams) {
      return abp.ajax($.extend(true, {
        url: abp.appPath + 'api/language-management/language-texts' + abp.utils.buildQueryString([{ name: 'filter', value: input.filter }, { name: 'resourceName', value: input.resourceName }, { name: 'baseCultureName', value: input.baseCultureName }, { name: 'targetCultureName', value: input.targetCultureName }, { name: 'getOnlyEmptyValues', value: input.getOnlyEmptyValues }, { name: 'sorting', value: input.sorting }, { name: 'skipCount', value: input.skipCount }, { name: 'maxResultCount', value: input.maxResultCount }]) + '',
        type: 'GET'
      }, ajaxParams));
    };

    x.abp.languageManagement.languageText.get = function(resourceName, cultureName, name, baseCultureName, ajaxParams) {
      return abp.ajax($.extend(true, {
        url: abp.appPath + 'api/language-management/language-texts/' + resourceName + '/' + cultureName + '/' + name + '' + abp.utils.buildQueryString([{ name: 'baseCultureName', value: baseCultureName }]) + '',
        type: 'GET'
      }, ajaxParams));
    };

    x.abp.languageManagement.languageText.update = function(resourceName, cultureName, name, value, ajaxParams) {
      return abp.ajax($.extend(true, {
        url: abp.appPath + 'api/language-management/language-texts/' + resourceName + '/' + cultureName + '/' + name + '' + abp.utils.buildQueryString([{ name: 'value', value: value }]) + '',
        type: 'PUT',
        dataType: null
      }, ajaxParams));
    };

    x.abp.languageManagement.languageText.restoreToDefault = function(resourceName, cultureName, name, ajaxParams) {
      return abp.ajax($.extend(true, {
        url: abp.appPath + 'api/language-management/language-texts/' + resourceName + '/' + cultureName + '/' + name + '/restore',
        type: 'PUT',
        dataType: null
      }, ajaxParams));
    };

  })();

})();


