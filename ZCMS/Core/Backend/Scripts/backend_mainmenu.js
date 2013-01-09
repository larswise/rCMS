
// Global service URLS
getPagesServiceUrl              = '/api/AjaxBackend/PostGetPages/';
postTopicServiceUrl             = '/api/AjaxBackend/PostTopic';
postPageTopicServiceUrl         = '/api/AjaxBackend/PostPageTopic';
uploadUnitServiceUrl            = '/File/RenderUnit/UploadUnit';
confirmDeleteServiceUrl         = '/File/RenderUnit/ConfirmDelete';
deleteAttachmentServiceUrl      = '/File/DeleteAttachments';
fileManagerListServiceUrl       = '/File/FileManagerList';

fileEditorEditServiceUrlSlug    = '/File/FileEditor/';
uploadAttachmentServiceUrlSlug  = '/File/UploadAttachment/';
applyImageEffectServiceUrlSlug  = '/File/ApplyImageEffect/';

allowedExtensions = ['jpg', 'jpeg', 'png', 'gif', 'tif', 'tiff', 'bmp', 'eps', 'vsd', 'txt', 'rtf', 'doc', 'docx', 'xls', 'xlsx', 'ppt', 'pptx', 'pdf', 'mp3', 'mpeg', 'mp4', 'avi', 'accdb'];
webImageUrlPrefix = '/File/GetCurrentImage?key=';


$(function () {
    
    try {
        $(".mainmenuitem").click(function (e) {
            var clicked = $(this).attr('data-val');
            var menuItem = $(this).parent().find(".SubMenuItem");
            if(menuItem.is(":visible"))
                menuItem.slideUp('fast', 'easeInSine');
            else 
                menuItem.slideDown('fast', 'easeInSine');

        });
    } catch (e) {
        
    }
});


function FilterFileSelectorList(freeText) {
    $.ajax({
        type: 'GET',
        dataType: 'json',
        url: '/api/AjaxBackend/FileSelector/' + freeText,
        success: function (data) {
            $("#FileManagerSelectorListing .file-md-ir").remove();
            var builder = '';
            for (i = 0; i < data.length; i++) {
                builder += '' +
                '<div class="file-md-ir" data-val="' + data[i].FileKey + '"><div class="file-md-ic"><input type="checkbox" name="' + data[i].FileKey + '" id="' + data[i].FileKey + '"/></div>' +
                '<div class="file-md-ic"><img src="/Content/Backend/Images/Formats/' + data[i].Extension + '.png" alt="" /></div>' +
                '<div class="file-md-ic" title="' + data[i].FileName + '">' + data[i].FileName + '</div></div>'
            }

            $("#FileManagerSelectorListing .file-md-hr").after(builder).fadeIn('slow', 'easeInSine');
        },
        error: function () {
            
        },
        traditional: true
    });
}

IgnoreUntilBreak = {

    interval: 750,

    lastKeypress: null,

    interceptKeypress: function (cb, param) {
        this.lastKeypress = new Date().getTime();
        var that = this;
        setTimeout(function () {
            var currentTime = new Date().getTime();
            if (currentTime - that.lastKeypress > that.interval) {
                that.sendRequest(cb, param);
            }
        }, that.interval + 100);
    },

    sendRequest: function (cb, param) {
        cb(param);
    }

}

function isDark(color) {try {
    var match = /rgb\((\d+).*?(\d+).*?(\d+)\)/.exec(color);
    return parseFloat(match[1])
         + parseFloat(match[2])
         + parseFloat(match[3])
           < 3 * 200 / 2; // r+g+b should be less than half of max (3 * 256)
} catch (exp) { return false; }
}