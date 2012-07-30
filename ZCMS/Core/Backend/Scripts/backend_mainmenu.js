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


});