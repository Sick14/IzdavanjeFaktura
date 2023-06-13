$(document).ready(function () {
    var orderItems = [];
    $('#add').click(function () {
        var $newRow = $('#tableRow').clone().removeAttr('id');

        $('#add', $newRow).addClass('remove').val('Remove').removeClass('btn-success').addClass('btn-danger');

        $('#ProductID', $newRow).replaceWith('<input type="text" id="ProductID" name="ProductID" class="form-control">');
        $('#ProductID', $newRow).val($('#ProductID').find(":selected").text());

        $('#Quantity, #ProductID', $newRow).attr("disabled", true);

        $('#ProductID, #Quantity, #PriceWithoutVATPerItem, #TotalPriceWithoutVATPerItem, #add', $newRow).removeAttr('id');

        $('#ordertable').append($newRow);

        calculatePrice($('#TotalPriceWithoutVATPerItem').val());

        orderItems.push({
            ProductID: $('#ProductID').val().trim(),
            Quantity: parseInt($('#Quantity').val().trim()),
            PriceWithoutVAT: Number($('#PriceWithoutVATPerItem').val()),
            TotalPriceWithoutVAT: Number($('#TotalPriceWithoutVATPerItem').val())
        });

        $('#ProductID').val('');
        $('#Quantity').val(0);
        $('#PriceWithoutVATPerItem, #TotalPriceWithoutVATPerItem').val('');
        $('#add').attr("disabled", true);
    });

    $('#ordertable').on('click', '.remove', function () {
        var rowNumber = $(this).closest("tr").index();
        var price = $(this).parents('tr').find("[name='TotalPriceWithoutVATPerItem']").val();
        calculatePriceSubstract(price);
        $(this).parents('tr').remove();
        orderItems.splice(rowNumber, 1); 
    });

    function calculatePrice(totalPrice) {
        var totalPriceWithoutVAT = Number($('#TotalPriceWithoutVAT').val());
        var totalPriceConvert = Number(totalPrice);
        var sum = totalPriceWithoutVAT + totalPriceConvert;
        $('#TotalPriceWithoutVAT').val(sum.toFixed(2));

        if ($('#CountryID').val() != '' && $('#TotalPriceWithoutVAT').val() != '') {
            $.ajax({
                type: "GET",
                url: '/VAT/CalculateVAT',
                data: { countryId: $('#CountryID').val(), price: $('#TotalPriceWithoutVAT').val() },
                success: function (vatAmount) {
                    $('#TotalPriceWithVAT').val(vatAmount.toFixed(2));
                }
            });
        }
    }

    function calculatePriceSubstract(totalPrice) {
        var totalPriceWithoutVAT = Number($('#TotalPriceWithoutVAT').val());
        var totalPriceConvert = Number(totalPrice);
        var total = totalPriceWithoutVAT - totalPriceConvert;
        $('#TotalPriceWithoutVAT').val(total.toFixed(2));

        if ($('#CountryID').val() != '' && $('#TotalPriceWithoutVAT').val() != '') {
            $.ajax({
                type: "GET",
                url: '/VAT/CalculateVAT',
                data: { countryId: $('#CountryID').val(), price: $('#TotalPriceWithoutVAT').val() },
                success: function (vatAmount) {
                    $('#TotalPriceWithVAT').val(vatAmount.toFixed(2));
                }
            });
        }
    }
    $('#submit').click(function (e) {
        var data = {
            InvoiceNumber: $('#InvoiceNumber').val().trim(),
            InvoiceIssueDate: $('#InvoiceIssueDate').val().trim(),
            InvoiceDueDate: $('#InvoiceDueDate').val().trim(),
            TotalPriceWithoutVAT: $('#TotalPriceWithoutVAT').val().trim(),
            TotalPriceWithVAT: $('#TotalPriceWithVAT').val().trim(),
            Customer: $('#Customer').val().trim(),
            InvoiceItems: orderItems
        }

        $("#myForm").validate(); // this will validate the form and show the validation messages
        if ($("#myForm").valid()) {
            $.ajax({
                url: '/Invoices/Create',
                type: "POST",
                data: JSON.stringify(data),
                dataType: "JSON",
                contentType: "application/json"
            });
            if (orderItems.length > 0) {
                $(location).prop('href', '../Invoices/Index');
            }
            else {
                alert("error");
            }
        }
        e.preventDefault();
    }); 
});