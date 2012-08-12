$(function () {
    $(".file-manager-upload").click(function () {
        $("#modal").modal({
            message: 'File Uploader',
            closebutton: $("#modal-close"),
            contentcontainer: $(".cmsModalInner"),
            dataurl: "/Backend/RenderUnit/UploadUnit"
        });
    });


});


