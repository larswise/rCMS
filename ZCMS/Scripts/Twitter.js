
$(document).ready(function () {

    $("#TwitterLogin").click(function () {
        // first, get the signing key
        $.getJSON('/api/AjaxFrontend?consumerKey=' + twitterConsumerKey, function (data) {
            console.log("succe" + data);
            $.ajax({
                type: 'POST',
                dataType: 'json',
                contentType: 'application/json; charset=UTF-8',
                data: {
                    oauth_callback: window.location,
                    oauth_signature_method: 'HMAC-SHA1',
                    oauth_consumer_key: twitterConsumerKey,
                    oauth_signature: data
                },
                url: 'https://api.twitter.com/oauth/request_token',
                success: function (data) {
                    console.log(data);
                },
                error: function () {
                    console.log("error");
                }
            });
        });      
        
    });
});