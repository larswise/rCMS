/* DASH */

function GetDashboardPages(param) {


    $.ajax({
        type: 'POST',
        dataType: 'json',
        contentType: 'application/json; charset=UTF-8',
        url: getPagesServiceUrl,
        data: JSON.stringify(param),
        success: function (data) {
            $("#dashboard-published-pages").css("background", "none");
            for (i = 0; i < data.length; i++) {
                PagesViewModel.addPage(new PageItem(data[i]));
            }
            MakeDragDrop();
        },
        error: function () {
            $("#dashboard-published-pages").css("background", "none");
        },
        traditional: true,
        async: true
    });
    
}
// MVVM Published Pages

var PublishedViewModel;

function PublishedPagesViewModel(arrayParam) {
    var self = this;
    self.pages = ko.observableArray(ko.utils.arrayMap(arrayParam, function (page) {
        return { PageName: page.PageName, PageId: page.PageId, Created: page.Created, CreatedBy: page.CreatedBy, LastModified: page.LastModified, LastModifiedBy: page.LastModifiedBy, Status: page.Status, StartPublish: page.StartPublish, EndPublish: page.EndPublish, PageType: page.PageType, EditUrl: page.EditUrl, ViewUrl: page.ViewUrl, TopicId: page.TopicId };
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
    self.ViewUrl = item.ViewUrl;
    self.TopicId = item.TopicId;
}


/* TOPICS */

ko.bindingHandlers.uniqueId = {
    init: function (element) {
        var mPrefix = $(element).attr('data-prefix').length > 0 ? $(element).attr('data-prefix') : ko.bindingHandlers.uniqueId.prefix;
        element.id = mPrefix + (++ko.bindingHandlers.uniqueId.counter);
    },
    counter: 0,
    prefix: "unique"
};

ko.bindingHandlers.uniqueFor = {
    init: function (element, valueAccessor) {
        var after = ko.bindingHandlers.uniqueId.counter - 1 + (ko.utils.unwrapObservable(valueAccessor()) === "after" ? 0 : 1);
        element.setAttribute("for", ko.bindingHandlers.uniqueId.prefix + after);
    }
};

function sortTiles(selector) {
    $(selector).sort(function (a, b) {
        $(b).css("color", isDark($(b).css("background-color")) ? 'white' : 'black');
        return $(a).height() * $(a).width() < $(b).height() * $(b).width() ? 1 : -1;
    }).appendTo('.topics-list');
}

function setTileSize() {
    $('.topics-list > .topic-item').each(function () {
        var totSize = $(this).html().length;

        if (totSize <= 700) {
            $(this).width(179).height(145);
        } else if (totSize > 700 && totSize <= 750) {
            $(this).width(179).height(145);
        } else if (totSize > 740 && totSize <= 825) {
            $(this).width(285).height(145);
        } else {
            $(this).width(605).height(145);
        }
    });
}

function PostNewTopic(topic) {

    $.ajax({
        type: 'POST',
        dataType: 'json',
        contentType: 'application/json; charset=UTF-8',
        url: postTopicServiceUrl,
        data: JSON.stringify(topic),
        success: function (data) {
            TopixViewModel.addTopix(topic);
            setTileSize();
            sortTiles($('.topics-list > .topic-item'));
        },
        error: function () {
            alert("error");
        },
        traditional: true,
        async: true
    });

}

function TopixViewModel(arrayParam) {
    var self = this;
    self.topix = ko.observableArray(ko.utils.arrayMap(arrayParam, function (topix) {
        return { Name: topix.Name, Description: topix.Description, Color: topix.Color, ShowInMenu: topix.ShowInMenu, TopicId: topix.TopicId };
    }));



    self.addTopix = function (topix) {
        self.topix.push(topix);
    };

    self.removeAll = function () {
        self.topix.removeAll();
    };
}


function TopixItem(item) {
    var self = this;
    self.TopicId = item.TopicId;
    self.Description = item.Description;
    self.Name = item.Name;
    self.Color = item.Color;
    self.ShowInMenu = item.ShowInMenu;
}