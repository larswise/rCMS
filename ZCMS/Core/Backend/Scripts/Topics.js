
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
