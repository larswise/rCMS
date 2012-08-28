$(document).ready(function () {

    $(".file-manager-upload").click(function () {
        $("#modal").modal({
            message: 'File Uploader',
            closebutton: $("#modal-close"),
            contentcontainer: $(".cmsModalInner"),
            dataurl: uploadUnitServiceUrl
        });
    });

    $(".file-extension").click(FilterFileList);
    $("#filemanager-filter-input").keyup(function () {
        if($(this).val().length>1 || $(this).val().length==0)
            FilterFileList();
    });
    $("#file-manager-refresh").click(function () {
        $("#filemanager-filter-input").val('');
        FilterFileList();
    });

    FileMarkDeleteInit();
    FileEditInit();
});

function FileMarkDeleteInit() {
    $(".file-delete").click(function () {
        $("#modal").modal({
            message: 'Confirm Delete',
            silentclosebutton: $("#modal-close"),
            callbackclosebuttoninternal: "#confirm-deletion-final",
            contentcontainer: $(".cmsModalInner"),
            dataurl: confirmDeleteServiceUrl,
            closehandler: FileConfirmDelete
        });
    });

    $(".file-md-ir").click(function () {
        if (!$(this).hasClass('file-md-ir-active'))
            $(this).addClass('file-md-ir-active');
        else
            $(this).removeClass('file-md-ir-active');
    });
}

function FileEditInit() {
    $(".file-edit").click(function () {
        $("#modal").modal({
            message: 'Edit File',
            silentclosebutton: $("#modal-close"),
            callbackclosebuttoninternal: "#filemanager-edit-ok",
            contentcontainer: $(".cmsModalInner"),
            dataurl: fileEditorEditServiceUrlSlug + $(this).attr('data-val'),
            closehandler: FileConfirmChanges
        });
    });
}

function FileConfirmDelete() {
    console.log("go delete it...");
    var deleteArray = $.map($(".file-md-ir-active"), function (item, index) {
        return $(item).find('img[data-operation="delete"]').attr('data-val');
    });

    $.ajax({
        type: 'POST',
        data: JSON.stringify(deleteArray),
        url: deleteAttachmentServiceUrl,
        contentType: 'application/json; charset=UTF-8',
        success: function (data) {
            $(".file-md-ir-active").remove();
            $(".deleted-status").html(data).fadeIn('slow', 'easeInSine');
        },
        error: function () {
            console.log("something went wrong with ajax!");
        },
        traditional: true
    });

}

function FileUploader(currentPage) {    
    var uploader = new qq.FileUploader({
        element: document.getElementById('file-uploader'),
        action: uploadAttachmentServiceUrlSlug + currentPage,
        debug: true,
        allowedExtensions: allowedExtensions
    });

    if ($("label[for='New']").text() == "New") {
        $("div .qq-upload-button").attr('display', 'none');
        $("input[name='file']").attr('disabled', 'disabled');
        $("div .qq-upload-list").text($(".noupload").text());
    }
}

function FilterFileList() {
    $("#FileManagerFileList").html('');
    var value = $(this).attr('data-val');
    if ($(this).children("img").hasClass("deselected")) {
        $(this).children("img").removeClass("deselected")
    } else {
        $(this).children("img").addClass("deselected")
    }

    var arr = $.map($(".file-extension img:not('[class=deselected]')"), function (item, index) {
        return $(item).attr('data-val').toLowerCase();
    });
    var freeText = $(".file-manager-filter-input").val();
    var postData = { extensionFilter: arr, filterFreeText: freeText };

    $.ajax({
        dataType: 'html',
        type: 'POST',
        data: postData,
        url: fileManagerListServiceUrl,
        success: function (data) {
            $("#FileManagerFileList").hide().html(data).fadeIn('slow', 'easeInSine');
            FileMarkDeleteInit();
            FileEditInit();
        },
        error: function () {
            console.log("something went wrong with ajax!");
        },
        traditional: true
    });
}


function FileConfirmChanges() {
    var itemToEditKey = $(".file-edit-wrap").find("img").attr('data-val');
    var newName = $("#FileName").val();
    var newDescription = $("#Description").val();
    var newContentType = $("#file-content-type").val();

    console.log(itemToEditKey + " - " + newName + " - " + newDescription + " - " + newContentType);
}

function ApplyImageEffect(effect, image) {
    image.attr('src', $('.file-progressbar > img').attr('src'));
    $.ajax({
        dataType: 'json',
        type: 'POST',
        url: applyImageEffectServiceUrlSlug + effect + '/' + image.attr('data-val'),
        success: function (data) {
            image.attr('src', webImageUrlPrefix + data + '&ts=' + new Date().getMilliseconds());
        },
        error: function () {
            console.log("something went wrong with ajax!");
            $(".file-progressbar").hide();
        },
        traditional: true
    });
}