
$(function () {
    var PagesArray = new Array();
    var counter = 0;
    $.each(pageItems, function () {
        PagesArray[counter] = new PageItem(this);
        counter++;
    });
    PagesViewModel = new PublishedPagesViewModel(PagesArray);
    ko.applyBindings(PagesViewModel);

    $("#dashboard-text-filter").keyup(function () {
        if ($(this).val().length > 2 || $(this).val().length == 0) {
            simpleParameter = new Object();
            simpleParameter.Param1 = $(this).val();
            simpleParameter.Param2 = "";
            GetDashboardPages(simpleParameter);
        }
    });

    $(".dashboard-filter-link").click(function () {
        simpleParameter = new Object();
        simpleParameter.Param1 = "";
        simpleParameter.Param2 = $(this).attr('data-val');
        GetDashboardPages(simpleParameter);
    });

});

function GetDashboardPages(param) {
    $.ajax({
        type: 'POST',
        dataType: 'json',
        contentType: 'application/json; charset=UTF-8',
        url: getPagesServiceUrl,
        data: JSON.stringify(param),
        success: function (data) {
            PagesViewModel.removeAll();
            for (i = 0; i < data.length; i++) {
                PagesViewModel.addPage(new PageItem(data[i]));
            }
        },
        error: function () {
            console.log("something went wrong with ajax!");
        },
        traditional: true
    });
}
// MVVM Published Pages

var PublishedViewModel;

function PublishedPagesViewModel(arrayParam) {
    var self = this;
    self.pages = ko.observableArray(ko.utils.arrayMap(arrayParam, function (page) {
        return { PageName: page.PageName, PageId: page.PageId, Created: page.Created, CreatedBy: page.CreatedBy, LastModified: page.LastModified, LastModifiedBy: page.LastModifiedBy, Status: page.Status, StartPublish: page.StartPublish, EndPublish: page.EndPublish, PageType: page.PageType, EditUrl: page.EditUrl };
    }));



    self.addPage = function (page) {
        self.pages.push(page);
    };

    self.removeAll = function () {
        self.pages.removeAll();
    };
}

function PageItem(item) {
    var self = this;
    self.PageName = item.PageName;
    self.PageId = item.PageId;
    self.Created = item.Created;
    self.CreatedBy = item.CreatedBy;
    self.LastModified = item.LastModified;
    self.LastModifiedBy = item.LastModifiedBy;
    self.Status = item.Status;
    self.StartPublish = item.StartPublish;
    self.EndPublish = item.EndPublish;
    self.PageType = item.PageType;
    self.EditUrl = item.EditUrl;
}