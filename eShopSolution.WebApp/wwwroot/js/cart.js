var CartController = function () {
    this.initialize = function () {
        loadData();
    }
    function loadData() {
        const baseimagepath = $('#hiImagePath').val();
        const culture = $('#hiCulture').val();
        $.ajax({
            type: 'GET',
            url: '/' + culture + '/cart/GetListItems',
            success: function (res) {
                var html = '';
                var total = 0;
                $.each(res, function (i, item) {
                    var amount = item.price * item.quantity
                    html += "<tr>"
                        + "    <td> <img width=\"60\" src=\"" + baseimagepath + item.image + "\" alt=\"\" /></td>"
                        + "    <td>MASSA AST<br />" + item.description + "</td>"
                        + "    <td>"
                        + "        <div class=\"input-append\">"
                        + "            <input class=\"span1\" style=\"max-width:34px;height:30px;\" placeholder=\"" + numberWithCommas(item.quantity) + "\" id=\"appendedInputButtons\" size=\"16\" type=\"text\">"
                        + "            <button class=\"btn\" type=\"button\"><i class=\"icon-minus\"></i></button><button class=\"btn\" type=\"button\">"
                        + "                <i class=\"icon-plus\"></i>"
                        + "            </button>"
                        + "            <button class=\"btn btn-danger\" type=\"button\">"
                        + "                <i class=\"icon-remove icon-white\"></i>"
                        + "            </button>"
                        + "        </div>"
                        + "    </td>"
                        + "    <td>" + numberWithCommas(item.price) + "</td>"
                        + "    <td>" + numberWithCommas(amount) + "</td>"
                        + "</tr>"
                    total += amount;
                    $('#cartbody').html(html);
                    $('#lbl_total').text(numberWithCommas(total));
                    $('#lbl_number_of_items').text(res.length);
                });
            },
            error: function (err) {
                console.log(err);
            }
        });
    };
}