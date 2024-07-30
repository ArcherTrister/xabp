var abp = abp || {};

abp.modals.directoryMoveModal = function () {
    var initModal = function (publicApi, args) {

        var l = abp.localization.getResource("FileManagement");

        var directoryDescriptorService = x.fileManagement.directories.directoryDescriptor;

        var $directoryContentTable = $('#MoveDirectory_DirectoryContentTable');
        var $directoryPathList = $('#DirectoryPathList');
        var id = $("#MoveDirectoryInput_Id").val();

        function getDirectoryId() {
            var directoryId = $directoryContentTable.attr("directory-id");
            return (directoryId === undefined || directoryId === "" || directoryId === "null" || directoryId === "00000000-0000-0000-0000-000000000000") ? null : directoryId;
        }

        function setDirectoryId(directoryId) {
            directoryId = directoryId === undefined || directoryId === "" || directoryId === "null" || directoryId === "00000000-0000-0000-0000-000000000000" ? null : directoryId;
            $directoryContentTable.attr("directory-id", directoryId);
            $('#MoveDirectoryInput_NewParentId').val(directoryId === null ? '' : directoryId);

            return directoryId;
        }

        function initDirectoryPathList() {
            $directoryPathList.html(getDirectoryPathItem(getDirectoryId(), addPathItem));
        }

        function getDirectoryPathItem(id, callback) {
            if(id === null){
                callback({ id: null, name: l('Root') });
            }else{
                directoryDescriptorService.get(id).then(function (result) {
                    callback(result);
                });
            }
        }

        function removePathItemsAfterIndex(index){
            var $children = $directoryPathList.children();

            for(i = index + 1; i < $children.length; i++){
                $children[i].remove();
            }
        }

        function addPathItem(directoryPathItem){
            $directoryPathList.append(createDirectoryPathItem(directoryPathItem));
        }

        function createDirectoryPathItem(directory){
            return '<li class="breadcrumb-item" directory-id="' + directory.id + '"><span>' + directory.name + '</span></li>';
        }

        var datatableConfigurations = abp.libs.datatables.normalizeConfiguration({
            processing: true,
            serverSide: false,
            paging: false,
            autoWidth: false,
            searching: false,
            scrollCollapse: true,
            ajax: abp.libs.datatables.createAjax(directoryDescriptorService.getList, getDirectoryId, (result)=>{
                result.items = result.items.filter(x => x.id !== id);
                return {
                    recordsTotal: result.totalCount,
                    recordsFiltered: result.totalCount,
                    data: result.items
                };
            }),
            columnDefs: [
                {
                    title: l('Name'),
                    data: "name",
                    render: function(data, type, row){
                        data = $.fn.dataTable.render.text().display(data);
                        return '<span class="directoryName" directory-id="' + row.id + '"><i class="directory-content-icon fa fa-folder text-primary text-center me-1"></i>' + data + '</span>';
                    }
                }
            ],
        });

        datatableConfigurations["language"] = {
            "emptyTable": l('MoveHere')
        };

        initDirectoryPathList();

        var dataTable = $directoryContentTable.DataTable(datatableConfigurations);

        $directoryContentTable.on('click', 'span.directoryName', function(){
            var selectedId = setDirectoryId($(this).attr('directory-id'));

            getDirectoryPathItem(selectedId, addPathItem);

            dataTable.ajax.reload();
        });

        $directoryPathList.on('click', 'li.breadcrumb-item', function () {
            var $clickedElem = $(this);

            setDirectoryId($clickedElem.attr('directory-id'));

            removePathItemsAfterIndex($clickedElem.index());

            dataTable.ajax.reload();
        });
    };

    return {
        initModal: initModal
    };
};
