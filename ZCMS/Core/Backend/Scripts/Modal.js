(function (window, $) {
    var Modal = function (elem, options) {
        this.elem = elem;
        this.$elem = $(elem);
        this.options = options;
    };

    Modal.prototype = {
        defaults: {
            message: 'Header'
        },
        init: function () {
            this.config = $.extend({}, this.defaults, this.options);
            this.buildmodal();
            this.displayModal();
            this.getdata(this.config.contentcontainer);
            var element = $(this.config.contentcontainer);
            this.config.closebutton = $("#modal-close").clone().appendTo(this.$elem).show();

            var closeHandler = null;
            if (this.config.closehandler)
                closeHandler = this.config.closehandler;
            this.config.closebutton.click(function () {
                if (closeHandler)
                    closeHandler();
                element.html('');
                element.parent().fadeOut('slow');
                $(".modalBack").fadeOut('fast');
                                
            });
            return this;
        },
        displayModal: function () {
            $(".modalBack").fadeIn('fast');

            this.$elem.css("top", ($(window).height() - 300) / 2 + $(window).scrollTop() + "px");
            this.$elem.css("left", ($(window).width() - 450) / 2 + $(window).scrollLeft() + "px");
            this.$elem.addClass('cmsModal').css('style', '').fadeIn('slow');


        },
        buildmodal: function () {
            this.$elem.after("<div class='modalBack'></div>");
            if (this.$elem.children(".modal-header").length < 1) {
                this.$elem.prepend("<div class='modal-header'>" + this.config.message + "</div>");
            }
        },
        getdata: function (element) {
            $.ajax({
                dataType: 'html',
                type: 'GET',
                url: this.config.dataurl,
                success: function (data) {
                    console.log('<span>' + data.result + '</span>');
                    element.append('<div>' + data + '</div>');
                },
                error: function (xhr, status, error) {
                    console.log('ajax error ' + xhr.status);
                }
            });
        }
    };

    Modal.defaults = Plugin.prototype.defaults;

    $.fn.modal = function (options) {
        return this.each(function () {
            new Modal(this, options).init();
        });
    };

    window.Modal = Modal;
})(window, jQuery);