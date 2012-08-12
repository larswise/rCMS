$(document).ready(function () {
    
    $(".mainmenuitem").click(function (e) {
        var clicked = $(this).attr('data-val');
        $.get("/Backend/RenderLeftMenu/" + clicked, function (data) {

            $(".LeftAdminWelcome").hide().html(data).fadeIn('slow', 'easeInSine', function () {
            });
        });
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


});

function FileUploader() {
    var uploader = new qq.FileUploader({
        element: document.getElementById('file-uploader'),
        action: '/Admin/UploadAttachment/' + $("#CurrentPage").text(),
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
        return $(item).attr('data-val');
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
        },
        error: function () {
            console.log("something went wrong with ajax!");
        },
        traditional: true
    });
}


