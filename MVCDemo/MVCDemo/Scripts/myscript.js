$(function () {
    $(document).on("click", "#contentPage a", function () {
        $.ajax({
            url: $(this).attr("href"),
            type: 'GET',
            cache: false,
            success: function (result) {
                $("#content").html(result);
            }
        });
        return false;
    });
});
$(function () {
    $('#all').on("click", function () {
        $('.list').prop("checked", $(this).prop("checked"));

    });
});
$(function () {
    $('.btn-modal').click(function (e) {
        e.preventDefault();
        var $modal = $('#myModal');
        var $modalDialog = $('.modal-dialog');
        var href = $(this).prop('href');
        // không cho phép tắt modal khi click bên ngoài modal
        var option = { backdrop: 'static' };
        // load modal
        $modalDialog.load(href, function () {
            $modal.modal(option, 'show');


        });
    });
});

