
$(document).ready(function () {

    $("#TwitterLogin").click(function () {
        // first, get the signing key
        $.getJSON('/api/AjaxFrontend?consumerKey=' + twitterConsumerKey, function (data) {
            
        });      
        
    });
});