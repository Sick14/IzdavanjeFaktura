$(document).ready(function () {
    var orderItems = [];
    $('#Quantity').val(0);
    $("#InvoiceIssueDate, #InvoiceDueDate").change(function () {
            var start_date = $("#InvoiceIssueDate").val();
        var end_date = $("#InvoiceDueDate").val();
        if ((end_date < start_date) && end_date != '' && start_date != '') {
            $("#DueDateError").removeClass("hidden");
        }
        else {
            $("#DueDateError").addClass("hidden");
        }
    });

    // For Edit view
    if ($("#InvoiceID").val() !== undefined) {
        $.ajax({
            type: "POST",
            url: '/Invoices/GetInvoiceItemsByInvoiceID',
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify({ id: $("#InvoiceID").val() }),
            dataType: "json",
            success: function (result) {
                $.each(result, function (index, item) {
                    orderItems.push({
                        ProductID: item.ProductID,
                        Quantity: parseInt(item.Quantity),
                        PriceWithoutVAT: Number(item.PriceWithoutVAT),
                        TotalPriceWithoutVAT: Number(item.TotalPriceWithoutVAT)
                    });
                });
            }
        });
    }

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
        if (price === undefined) {
            price = $(this).parents('tr').find("[name='item.TotalPriceWithoutVAT']").val();
        }
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
            CountryID: $('#CountryID').val().trim(),
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
            if ($("#DueDateError").hasClass("hidden")) {
                if (orderItems.length > 0) {
                    var message = "Invoice created successfully!"
                    $(location).prop('href', '../Invoices/Index?message=' + message);
                }
                else {
                    $('.alert-danger').removeClass("hidden");
                    setTimeout(function () {
                        $('.alert-danger').addClass("hidden");
                    }, 3000);
                }
            }
        }
        e.preventDefault();
    }); 

    $('#submitEdit').click(function (e) {
        var data = {
            InvoiceNumber: $('#InvoiceNumber').val().trim(),
            InvoiceIssueDate: $('#InvoiceIssueDate').val().trim(),
            InvoiceDueDate: $('#InvoiceDueDate').val().trim(),
            TotalPriceWithoutVAT: $('#TotalPriceWithoutVAT').val().trim(),
            TotalPriceWithVAT: $('#TotalPriceWithVAT').val().trim(),
            Customer: $('#Customer').val().trim(),
            CountryID: $('#CountryID').val().trim(),
            InvoiceID: $('#InvoiceID').val(),
            InvoiceItems: orderItems
        }

        $("#myFormEdit").validate(); // this will validate the form and show the validation messages
        if ($("#myFormEdit").valid()) {
            $.ajax({
                url: '/Invoices/Edit',
                type: "POST",
                data: JSON.stringify(data),
                dataType: "JSON",
                contentType: "application/json"
            });
            if ($("#DueDateError").hasClass("hidden")) {
                if (orderItems.length > 0) {
                    var message = "Invoice edited successfully!"
                    // Timeout set so Edit can finish before relocation
                    setTimeout(function () {
                        $(location).prop('href', '../Index?message=' + message);
                    }, 30);
                    
                }
                else {
                    $('.alert-danger').removeClass("hidden");
                    setTimeout(function () {
                        $('.alert-danger').addClass("hidden");
                    }, 3000);
                }
            }
        }
        e.preventDefault();
    });
});