
$(function () {

    $(".file-selector").click(function () {
        $("#modal").modal({
            message: 'File Selector',
            silentclosebutton: $("#modal-close"),
            callbackclosebuttoninternal: ".file-picker-save-selected",
            contentcontainer: $(".cmsModalInner"),
            dataurl: "/Backend/FileSelector",
            closehandler: OnFileSelectorClose
        });
    });

    $("#save-site-description").click(function (e) {

        $("#site-description-form").submit();
    });

});

function OnFileSelectorClose() {
    
}