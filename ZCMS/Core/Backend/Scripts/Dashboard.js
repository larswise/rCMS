
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
        if ($(this).val().length > 2) {
            
            $.ajax({
                type: 'GET',
                dataType: 'json',
                url: '/api/AjaxBackend/GetPages/' + $(this).val(),
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
    });
});
// MVVM Published Pages

var PublishedViewModel;

function PublishedPagesViewModel(arrayParam) {
    var self = this;
    self.pages = ko.observableArray(ko.utils.arrayMap(arrayParam, function (page) {
        return { PageName: page.PageName, PageId: page.PageId, Created: page.Created, CreatedBy: page.CreatedBy, LastModified: page.LastModified, LastModifiedBy: page.LastModifiedBy, Status: page.Status, StartPublish: page.StartPublish, EndPublish: page.EndPublish };
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
}