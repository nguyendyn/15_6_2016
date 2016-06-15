(function (b, a, c) {
    var d = b();
    b.fn.dropdownHover = function (e) {
        if ("ontouchstart" in document) {
            return this
        }
        d = d.add(this.parent());
        return this.each(function () {
            var m = b(this),
                l = m.parent(),
                k = {
                    delay: 500,
                    instantlyCloseOthers: true
                },
                i = {
                    delay: b(this).data("delay"),
                    instantlyCloseOthers: b(this).data("close-others")
                },
                f = "show.bs.dropdown",
                j = "hide.bs.dropdown",
                g = b.extend(true, {}, k, e, i),
                h;
            l.hover(function (n) {
                if (!l.hasClass("open") && !m.is(n.target)) {
                    return true
                }
                d.find(":focus").blur();
                if (g.instantlyCloseOthers === true) {
                    d.removeClass("open")
                }
                a.clearTimeout(h);
                l.addClass("open");
                m.trigger(f)
            }, function () {
                h = a.setTimeout(function () {
                    l.removeClass("open");
                    m.trigger(j)
                }, g.delay)
            });
            m.hover(function () {
                d.find(":focus").blur();
                if (g.instantlyCloseOthers === true) {
                    d.removeClass("open")
                }
                a.clearTimeout(h);
                l.addClass("open");
                m.trigger(f)
            });
            l.find(".dropdown-submenu").each(function () {
                var o = b(this);
                var n;
                o.hover(function () {
                    a.clearTimeout(n);
                    o.children(".dropdown-menu").show();
                    o.siblings().children(".dropdown-menu").hide()
                }, function () {
                    var p = o.children(".dropdown-menu");
                    n = a.setTimeout(function () {
                        p.hide()
                    }, g.delay)
                })
            })
        })
    };
    b(document).ready(function () {
        b('[data-hover="dropdown"]').dropdownHover()
    })
})(jQuery, this);

//$('.page-footer').css('margin-top', $(document).height() - ($('.page-header-menu').height() + $('.page-container').height()) - $('.page-footer').height());



$(function () {
    $("#nav > li").each(function () {
        var $href = location.pathname;
        if ($href == null) {
            $("#nav li:first-child").addClass("active");

        }
        else {
            var $attribute = $("a", this).attr('href');
            if ($href == $attribute) {
                $(this).addClass('active');
            }
        }
    });
});

jQuery(document).ready(function () {
    //Check to see if the window is top if not then display button
    jQuery(window).scroll(function () {
        if ($(this).scrollTop() > 10) {
            $('.page-header-menu').addClass("scroll-menu");
        } else {
            $('.page-header-menu').removeClass("scroll-menu");
        }
    });

    //Click event to scroll to top
    jQuery('.scroll-menu').click(function () {
        $('html, body').animate({
            scrollTop: 0
        }, 300);
        return false;
    });

});

