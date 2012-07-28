$(document).ready(function () {
    $(".mainmenuitem").click(function (e) {
        $.get("/Backend/RenderLeftMenu/" + $(this).attr('data-val'), function (data) {

            $(".LeftAdminWelcome").hide().html(data).fadeIn('slow', 'easeInSine', function () { });
        });
    });

    $("#view-document-versions").click(function (e) {
        $.get("/Backend/RenderAllRevisions/" + $(this).attr('data-val'), function (data) {

            $("#RevisionsContainer").hide().html(data).fadeIn('slow', 'easeInSine', function () { });

            $("#close-revisions").click(function (e) {
                $("#RevisionsContainer").fadeOut('slow', 'easeOutSine', function () { });
            });
        });
    });

    $("#save-publish-page").click(function (e) {        
        $("#page-editor-form").submit();
    });


});