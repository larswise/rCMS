
$(function () {

    $(".datefield").datepicker();

    $(".file-manager-upload").click(function () {
        $("#modal").modal({
            message: 'File Uploader',
            closebutton: $("#modal-close"),
            contentcontainer: $(".cmsModalInner"),
            dataurl: "/Backend/RenderUnit/ImageListProperty"
        });
    });

    $(".file-selector").click(function () {
        $("#modal").modal({
            message: 'File Selector',
            closebutton: $("#modal-close"),
            contentcontainer: $(".cmsModalInner"),
            dataurl: "/Backend/FileSelector"
        });
    });
});

function FilterFileSelectorList(freeText) {
    $.ajax({
        type: 'GET',
        dataType: 'html',
        url: '/Backend/FileSelector/' + freeText,
        success: function (data) {
            $("#FileManagerFileList").hide().html(data).fadeIn('slow', 'easeInSine');
        },
        error: function () {
            console.log("something went wrong with ajax!");
        },
        traditional: true
    });
}