﻿
$(function () {
    $(".datefield").datepicker();

    $("#zcms-tabs").tabs({ idPrefix: "#zcms-tab-unit" });

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

    $("#Instance_PageName").keyup(function () {
        $("#url-slug").text($(this).val().replace(new RegExp("[^a-zA-Z0-9\_]+", "g"), "-"));
        $("#url-slug").parent().find(".actual-property-value").val($("#url-slug").text());
    });

    // ImageList mvvm
    if (typeof item==='undefined') {}
    else {
        var imagesArray = new Array();
        var counter = 0;
        $.each(item, function () {
            imagesArray[counter] = new ImageItem(this);
            counter++;
        });
        ImageListViewModel = new ImagesViewModel(imagesArray);
        ko.applyBindings(ImageListViewModel);
    }

    $(".unit-imagelist img").mouseover(function (e) {
        var posX = e.pageX;
        var posY = e.pageY;
        
        var template = "<div class='fullsize' style='position:absolute;z-index:10000; max-width: 400; max-height: 400; top:40%; left:50%;overflow:hidden;background:#c0c0c0;padding:10px;border:1px solid black;'>" +
                       "<img style='max-width: 400px; max-height: 400px; !important; vertical-align: middle;text-align:center' src='" + $(this).attr('src') + "' alt=' ' /></div>";

        $('.editor-image-container').before(template);

    }).mouseout(function () { $('.editor-image-container').parent().find(".fullsize").remove(); });
    

    $("#delete-document").click(function (e) {
        e.preventDefault();
        $("#modal").modal({
            message: 'Confirm Delete',
            silentclosebutton: $("#modal-close"),
            callbackclosebuttoninternal: "#confirm-deletion-final",
            contentcontainer: $(".cmsModalInner"),
            dataurl: "/File/RenderUnit/ConfirmDelete",
            closehandler: OnPageConfirmDelete
        });
    });
});

function OnPageConfirmDelete() {
    $("#page-editor-form").append("<input type='hidden' name='delete-page' value='true' />");
    $("#page-editor-form").submit();
}

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
                    var newImgItem = new ImageItem("/File/GetCurrentImage?key=" + data[i].FileKey);
                    ImageListViewModel.addImage(newImgItem);
                }
            },
            error: function () {
                console.log("something went wrong with ajax!");
            },
            traditional: true
        });
    }
}



// MVVM ImageList

var ImageListViewModel;

function ImagesViewModel(arrayParam) {
    var self = this;
    self.images = ko.observableArray(ko.utils.arrayMap(arrayParam, function (image) {
        return { imagePath: image.imagePath, fileKey: image.fileKey };
    }));

    self.removeImage = function (image) {
        var detachFile = new Object();
        detachFile.Param1 = image.fileKey;
        detachFile.Param2 = $("span#CurrentPage").text();
        //console.log(detachFile.Param1);

        // ajax remove image...
        
        $.ajax({
            type: 'POST',
            dataType: 'json',
            contentType: 'application/json; charset=UTF-8',
            data: JSON.stringify(detachFile),
            url: '/api/AjaxBackend/RemoveAttachmentFromPage/',
            success: function (data) {
                //console.log(data);
                self.images.remove(image);
            },
            error: function () {
                console.log("something went wrong with ajax!");
            },
            traditional: true
        });
    };

    self.addImage = function (image) {
        self.images.push(image);
    };
}

function ImageItem(key) {
    var self = this;
    self.imagePath = key;
    self.fileKey = key;
}