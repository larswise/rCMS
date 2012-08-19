$(document).ready(function () {
    
    $(".mainmenuitem").click(function (e) {
        var clicked = $(this).attr('data-val');
        console.log("im here ... " + $(this).find(".SubMenuItem"));
        $(this).parent().find(".SubMenuItem").slideToggle('fast', 'easeInSine');

    });

    $("#view-document-versions").click(function (e) {
        $.get("/Backend/RenderAllRevisions/" + $(this).attr('data-val'), function (data) {

            if (data) {
                $("#RevisionsContainer").hide().html(data).fadeIn('slow', 'easeInSine', function () { });
            } else {
                console.log(data);
                $("#RevisionsContainer").hide().html($(".norevs").text()).fadeIn('slow', 'easeInSine', function () { });
            }
            $("#close-revisions").click(function (e) {
                $("#RevisionsContainer").fadeOut('slow', 'easeOutSine', function () { });
            });
        });
    });

    $("#save-publish-page").click(function (e) {
        $("#page-editor-form").submit();
    });

    $("#save-draft-page").click(function (e) {
        $('<input>').attr({
            type: 'hidden',
            id: 'save-draft',
            name: 'save-draft',
            value: '1'
        }).appendTo('#page-editor-form');

        $("#page-editor-form").submit();
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
            dataurl: "/Backend/RenderUnit/ConfirmDelete",
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
            callbackclosebuttoninternal: "#confirm-edit",
            contentcontainer: $(".cmsModalInner"),
            dataurl: "/Backend/FileEditor/" + $(this).attr('data-val'),
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
        url: '/Backend/DeleteAttachments',
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
        action: '/Admin/UploadAttachment/' + currentPage,
        debug: true,
        allowedExtensions: ['jpg','jpeg','png','gif','tif','tiff','bmp','eps','vsd','txt','rtf','doc','docx','xls','xlsx','ppt','pptx','pdf','mp3','mpeg','mp4','avi','accdb']
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
        url: '/Backend/FileManagerList',
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
    // implement this!
}

function ApplyImageEffect(effect, image) {
    $(".file-progressbar").show();
    $.ajax({
        dataType: 'json',
        type: 'POST',
        url: '/Backend/ApplyImageEffect/' + effect + '/' + image.attr('data-val'),
        success: function (data) {
            image.attr('src', '/Backend/GetCurrentImage?key='+data+'&ts='+new Date().getMilliseconds());
            $(".file-progressbar").hide();
        },
        error: function () {
            console.log("something went wrong with ajax!");
            $(".file-progressbar").hide();
        },
        traditional: true
    });
}