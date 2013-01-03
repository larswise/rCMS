
$(function () {

    // bind obs arr

    var TopixArray = new Array();
    var counter = 0;
    var topix = {};
    if (topics.length > 0) {
        topix = JSON.parse(topics);
    }
    $.each(topix, function () {
        TopixArray[counter] = new TopixItem(this);
        counter++;
    });
    TopixViewModel = new TopixViewModel(TopixArray);
    ko.applyBindings(TopixViewModel);
    // post new topic
    
    $("#newtopic-button").click(function (e) {
        e.preventDefault();

        var topic = new TopixItem({ Name: $("#TopicName").val(), Description: $("#TopicDescription").val(), Color: $(".colorPicker-picker").css('background-color'), ShowInMenu: $("#TopicShowInMenu").val(), TopicId: 0 });
        PostNewTopic(topic);
    });

    //
    setTileSize();
    sortTiles();
    

});

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
        var after = ko.bindingHandlers.uniqueId.counter-1 + (ko.utils.unwrapObservable(valueAccessor()) === "after" ? 0 : 1);
        element.setAttribute("for", ko.bindingHandlers.uniqueId.prefix + after);
    }
};

function sortTiles() {
    $('.topics-list > .topic-item').sort(function (a, b) {
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
            sortTiles();            
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