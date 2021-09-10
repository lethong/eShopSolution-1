var SiteController = function () {
    this.initialize = function () {
        registerevent();
        loadCart();
    }
    function loadCart() {
        const culture = $('#hiCulture').val();
        $.ajax({
            type: 'GET',
            url: '/' + culture + '/cart/GetListItems',
            success: function (res) {
                $('#lbl_number_of_items_header').text(res.length);
            },
            error: function (err) {
                console.log(err);
            }
        });
    }
    function registerevent() {
        $('body').on('click', '.btn-add-cart', function (e) {
            e.preventDefault();
            const id = $(this).data('id');
            const culture = $('#hiCulture').val();
            $.ajax({
                type: 'POST',
                url: '/' + culture + '/cart/AddToCart',
                data: {
                    id: id,
                    languageId: culture
                },
                success: function (res) {
                    $('#lbl_number_of_items_header').text(res.length);
                },
                error: function (err) {
                    console.log(err);
                }
            });
        });
    }
}

function numberWithCommas(x) {
    return x.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
};