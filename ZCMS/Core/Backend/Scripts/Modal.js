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
            this.getdata(this.config.contentcontainer, this.config.closehandler, this.config.callbackclosebuttoninternal);
            var element = $(this.config.contentcontainer);
            this.config.closebutton = $("#modal-close").clone().appendTo(this.$elem).show();

            var closeHandler = null;

            if (this.config.closehandler && this.config.callbackclosebuttoninternal) {
                closeHandler = this.config.closehandler;
                $(element).find($(this.config.callbackclosebuttoninternal)).click(function () {
                    if (closeHandler)
                        closeHandler();
                    element.html('');
                    element.parent().fadeOut('fast');
                    $(".modalBack").fadeOut('fast');

                });
            }
            
            this.config.closebutton.click(function () {
                element.html('');
                element.parent().fadeOut('fast');
                $(".modalBack").fadeOut('fast');
            });

            return this;
        },
        displayModal: function () {
            $(".modalBack").fadeIn('fast');

            this.$elem.css("top", ($(window).height() - 300) / 2 + $(window).scrollTop() + "px");
            this.$elem.css("left", ($(window).width() - 450) / 2 + $(window).scrollLeft() + "px");
            this.$elem.addClass('cmsModal').css('style', '').fadeIn('fast');


        },
        buildmodal: function () {
            this.$elem.after("<div class='modalBack'></div>");
            if (this.$elem.children(".modal-header").length < 1) {
                this.$elem.prepend("<div class='modal-header'>" + this.config.message + "</div>");
            }
        },
        getdata: function (element, handler, handlertrigger) {
            $.ajax({
                dataType: 'html',
                type: 'GET',
                url: this.config.dataurl,
                success: function (data) {                    
                    element.append('<div>' + data + '</div>');


                    if (handler && handlertrigger) {
                        
                        $(element).find($(handlertrigger)).click(function () {
                            handler();
                            element.html('');
                            element.parent().fadeOut('fast');
                            $(".modalBack").fadeOut('fast');

                        });
                    }

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