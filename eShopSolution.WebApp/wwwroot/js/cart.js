var CartController = function () {
    this.initialize = function () {
        loadData();
        registerEvent();
    }

    function registerEvent() {
        $('body').on('click', '.btn-plus', function (e) {
            e.preventDefault();
            const id = $(this).data('id');
            const quantity = parseInt($('#txt_quantity_' + id).val()) + 1;
            updateCart(id, quantity);
        });

        $('body').on('click', '.btn-minus', function (e) {
            e.preventDefault();
            const id = $(this).data('id');
            const quantity = parseInt($('#txt_quantity_' + id).val()) - 1;
            updateCart(id, quantity);
        });

        $('body').on('click', '.btn-remove', function (e) {
            e.preventDefault();
            const id = $(this).data('id');
            updateCart(id, 0);
        });
    }

    function updateCart(id, quantity) {
        const culture = $('#hiCulture').val();
        $.ajax({
            type: 'POST',
            url: '/' + culture + '/cart/UpdateCart',
            data: {
                id: id,
                quantity: quantity
            },
            success: function (res) {
                $('#lbl_number_of_items_header').text(res.length);
                loadData();
            },
            error: function (err) {
                console.log(err);
            }
        });
    }

    function loadData() {
        const baseimagepath = $('#hiImagePath').val();
        const culture = $('#hiCulture').val();
        $.ajax({
            type: 'GET',
            url: '/' + culture + '/cart/GetListItems',
            success: function (res) {
                if (res.length === 0) {
                    $('#tbl_cart').hide();
                }

                var html = '';
                var total = 0;

                $.each(res, function (i, item) {
                    var amount = item.price * item.quantity
                    html += "<tr>"
                        + "    <td> <img width=\"60\" src=\"" + baseimagepath + item.image + "\" alt=\"\" /></td>"
                        + "    <td>" + item.name + "<br />" + item.description + "</td>"
                        + "    <td>"
                        + "        <div class=\"input-append\">"
                        + "            <input class=\"span1\" style=\"max-width:34px;height:30px;\" placeholder=\"" + numberWithCommas(item.quantity) + "\" id=\"txt_quantity_" + item.productId + "\" value=\"" + item.quantity + "\" size=\"16\" type=\"text\">"
                        + "            <button class=\"btn btn-minus\" data-id=\"" + item.productId + "\" type=\"button\"><i class=\"icon-minus\"></i></button>"
                        + "            <button class=\"btn btn-plus\" data-id=\"" + item.productId + "\" type=\"button\"><i class=\"icon-plus\"></i></button>"
                        + "            <button class=\"btn btn-danger btn-remove\" data-id=\"" + item.productId + "\" type=\"button\"><i class=\"icon-remove icon-white\"></i></button>"
                        + "        </div>"
                        + "    </td>"
                        + "    <td>" + numberWithCommas(item.price) + "</td>"
                        + "    <td>" + numberWithCommas(amount) + "</td>"
                        + "</tr>"
                    total += amount;
                });
                $('#cartbody').html(html);
                $('#lbl_total').text(numberWithCommas(total));
                $('#lbl_number_of_items').text(res.length);
            },
            error: function (err) {
                console.log(err);
            }
        });
    };
}