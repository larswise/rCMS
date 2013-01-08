
$(function () {


    $(".content-group").click(function () { alert("I am just a mock-up, yet!"); });
    
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
        PagesViewModel.removeAll();
        $("#dashboard-published-pages").css("background", "url('/content/backend/images/bar_loader.png') no-repeat scroll 50% 50% transparent");
        IgnoreUntilBreak.interceptKeypress(GetDashboardPages, simpleParameter);
    });

    $(".dashboard-current-page").draggable({ revert: "invalid" });

    $(".content-group").droppable({
        /*activeClass: "ui-state-hover",*/
        hoverClass: "ui-state-active",
        drop: function (event, ui) {
            $(ui.draggable).remove();
        }
    });

    ko.applyBindings(PagesViewModel);
    sortTiles($("#dashboard-groups > .content-group"));
});

