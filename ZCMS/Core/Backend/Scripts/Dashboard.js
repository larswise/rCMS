
$(function () {
        
    var PagesArray = new Array();
    var counter = 0;
    $.each(pageItems, function () {
        PagesArray[counter] = new PageItem(this);
        counter++;
    });
    PagesViewModel = new PublishedPagesViewModel(PagesArray);
    

    $("#dashboard-text-filter").keyup(function () {
        
        if ($(this).val().length > 2 || $(this).val().length == 0) {
            simpleParameter = new Object();
            simpleParameter.Param1 = $(this).val();
            simpleParameter.Param2 = "";
            simpleParameter.Param3 = $("#selected-topic").val();
            PagesViewModel.removeAll();
            $("#dashboard-published-pages").css("background", "url('/content/backend/images/bar_loader.png') no-repeat scroll 50% 50% transparent");
            IgnoreUntilBreak.interceptKeypress(GetDashboardPages, simpleParameter);
            //GetDashboardPages(simpleParameter);
        }
    });

    $(".dashboard-filter-link").click(function () {
        simpleParameter = new Object();
        simpleParameter.Param1 = "";
        simpleParameter.Param2 = $(this).attr('data-val');

        if (simpleParameter.Param2 == "Any") {
            $("#selected-topic").val('');
        }
        simpleParameter.Param3 = $("#selected-topic").val();

        PagesViewModel.removeAll();
        $("#dashboard-published-pages").css("background", "url('/content/backend/images/bar_loader.png') no-repeat scroll 50% 50% transparent");
        IgnoreUntilBreak.interceptKeypress(GetDashboardPages, simpleParameter);
    });

    $(".content-group").click(function () {
        $("#selected-topic").val(this.id);

        simpleParameter = new Object();
        simpleParameter.Param1 = "";
        simpleParameter.Param2 = "";
        simpleParameter.Param3 = $("#selected-topic").val();

        PagesViewModel.removeAll();
        $("#dashboard-published-pages").css("background", "url('/content/backend/images/bar_loader.png') no-repeat scroll 50% 50% transparent");
        IgnoreUntilBreak.interceptKeypress(GetDashboardPages, simpleParameter);
    });
    
    ko.applyBindings(PagesViewModel);
    sortTiles($("#dashboard-groups > .content-group"));

    MakeDragDrop();
});

function MakeDragDrop() {
    $(".dashboard-current-page").draggable({ revert: "invalid" });

    $(".content-group").droppable({
        /*activeClass: "ui-state-hover",*/
        hoverClass: "ui-state-active",
        drop: function (event, ui) {
            PostPageTopic($(ui.draggable).find(".current-page-head span.page-label-id").text(), $(ui.draggable).find(".current-page-head input[type='hidden']").val(), this.id);
            $(ui.draggable).remove();
        }
    });
}

function PostPageTopic(pageId, oldTopicId, topicId) {
    
    var multiSimpleParam = new Object();
    multiSimpleParam.Param1 = pageId;
    multiSimpleParam.Param2 = topicId;
    $.ajax({
        type: 'POST',
        dataType: 'json',
        contentType: 'application/json; charset=UTF-8',
        url: postPageTopicServiceUrl,
        data: JSON.stringify(multiSimpleParam),
        success: function (data) {
            if (data) {
                $("#" + topicId + " span:first").fadeOut('slow', function () {
                    $("#" + topicId + " span:first").text(parseInt($("#" + topicId + " span:first").text()) + 1);
                });

                $("#" + topicId + " span:first").fadeIn('slow');

                if (oldTopicId != null && oldTopicId.length > 0) {
                    $("#" + oldTopicId + " span:first").fadeOut('slow', function () {
                        $("#" + oldTopicId + " span:first").text(parseInt($("#" + oldTopicId + " span:first").text()) - 1);
                    });

                    $("#" + oldTopicId + " span:first").fadeIn('slow');
                }
            }
        },
        error: function () {
            alert("error");
        },
        traditional: true,
        async: true
    });

}