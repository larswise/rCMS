
$(function () {

    $(".datefield").datepicker();

    $(".file-manager-upload").click(function () {
        $("#modal").modal({
            message: 'File Uploader',
            closebutton: $("#modal-close"),
            contentcontainer: $(".cmsModalInner"),
            dataurl: "/Backend/RenderUnit/UploadUnit"
        });
    });

    $(".file-selector").click(function () {
        $("#modal").modal({
            message: 'File Selector',
            closebutton: $("#modal-close"),
            contentcontainer: $(".cmsModalInner"),
            dataurl: "/Backend/FileSelector",
            closehandler: OnFileSelectorClose
        });
    });

    $(".unit-imagelist img").mouseover(function (e) {
        var posX = e.pageX;
        var posY = e.pageY;
        console.log(posX + " " + posY);
        var template = "<div class='fullsize' style='position:absolute;z-index:100; max-width: 400; max-height: 400; top:"+ posY +"px; left:"+posX+"px;background:#c0c0c0;padding:10px;border:1px solid black;'>" +
                       "<img style='max-width: 400px !important; vertical-align: middle;text-align:center' src='" + $(this).attr('src') + "' alt=' ' /></div>";

        $(this).after(template);

    }).mouseout(function () { console.log("mouseouting - " + $(this).html()); $(this).parent().find(".fullsize").remove(); });
});

function OnFileSelectorClose() {
    var i = 0;
    var attachFilesArray = new Object();
    attachFilesArray.Keys = new Array();
    attachFilesArray.Id = $("span#CurrentPage").text();
    $("#FileManagerSelectorListing .file-md-ir").each(function () {
        var inputParent = $(this).find(">:first-child");

        if ($(inputParent).find("input[type='checkbox']").is(":checked")) {
            attachFilesArray.Keys[i] = $(this).attr('data-val');
            i++;
        }
    });

    if (attachFilesArray.Keys.length > 0) {
        $.ajax({
            type: 'POST',
            dataType: 'json',
            contentType: 'application/json; charset=UTF-8',
            data: JSON.stringify(attachFilesArray),
            url: '/api/AjaxBackend/AttachFilesToPage',
            success: function (data) {
                for (i = 0; i < data.length; i++) {
                    console.log("A file was attached... : " + data[i].FileKey);
                }
            },
            error: function () {
                console.log("something went wrong with ajax!");
            },
            traditional: true
        });
    }
}

function FilterFileSelectorList(freeText) {
    $.ajax({
        type: 'GET',
        dataType: 'json',
        url: '/api/AjaxBackend/FileSelector/' + freeText,
        success: function (data) {
            $("#FileManagerSelectorListing .file-md-ir").remove();
            var builder = '';
            for (i = 0; i < data.length; i++) {
                builder += ''+
                '<div class="file-md-ir" data-val="'+ data[i].FileKey +'"><div class="file-md-ic"><input type="checkbox" name="' + data[i].FileKey + '" id="' + data[i].FileKey + '"/></div>' +
                '<div class="file-md-ic"><img src="/Content/Backend/Images/Formats/' + data[i].Extension + '.png" alt="" /></div>' +
                '<div class="file-md-ic" title="' + data[i].FileName + '">' + data[i].FileName + '</div></div>'
            }
            
            $("#FileManagerSelectorListing .file-md-hr").after(builder).fadeIn('slow', 'easeInSine');
        },
        error: function () {
            console.log("something went wrong with ajax!");
        },
        traditional: true
    });
}