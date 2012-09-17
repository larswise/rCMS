
// Global service URLS
getPagesServiceUrl              = '/api/AjaxBackend/PostGetPages/';
uploadUnitServiceUrl            = '/File/RenderUnit/UploadUnit';
confirmDeleteServiceUrl         = '/File/RenderUnit/ConfirmDelete';
deleteAttachmentServiceUrl      = '/File/DeleteAttachments';
fileManagerListServiceUrl       = '/File/FileManagerList';

fileEditorEditServiceUrlSlug    = '/File/FileEditor/';
uploadAttachmentServiceUrlSlug  = '/File/UploadAttachment/';
applyImageEffectServiceUrlSlug  = '/File/ApplyImageEffect/';

allowedExtensions = ['jpg', 'jpeg', 'png', 'gif', 'tif', 'tiff', 'bmp', 'eps', 'vsd', 'txt', 'rtf', 'doc', 'docx', 'xls', 'xlsx', 'ppt', 'pptx', 'pdf', 'mp3', 'mpeg', 'mp4', 'avi', 'accdb'];
webImageUrlPrefix = '/File/GetCurrentImage?key=';


$(document).ready(function () {
    
    $(".mainmenuitem").click(function (e) {
        var clicked = $(this).attr('data-val');
        
        $(this).parent().find(".SubMenuItem").slideToggle('fast', 'easeInSine');

    });
});
