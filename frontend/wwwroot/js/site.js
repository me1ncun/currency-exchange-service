﻿document.addEventListener("DOMContentLoaded", function (event) {
    const host = "https://localhost:7234"

    // Fetch the list of currencies and populate the select element
    function requestCurrencies() {
        $.ajax({
            url: `${host}/currencies`,
            type: "GET",
            dataType: "json",
            success: function (data) {
                const tbody = $('.currencies-table tbody');
                tbody.empty();
                $.each(data, function (index, currency) {
                    const row = $('<tr></tr>');
                    row.append($('<td></td>').text(currency.code));
                    row.append($('<td></td>').text(currency.name));
                    row.append($('<td></td>').text(currency.sign));
                    tbody.append(row);
                });

                //

                const newRateBaseCurrency = $("#new-rate-base-currency");
                newRateBaseCurrency.empty();

                // populate the base currency select element with the list of currencies
                $.each(data, function (index, currency) {
                    newRateBaseCurrency.append(`<option value="${currency.code}">${currency.code}</option>`);
                });

                //

                const newRateTargetCurrency = $("#new-rate-target-currency");
                newRateTargetCurrency.empty();

                // populate the target currency select element with the list of currencies
                $.each(data, function (index, currency) {
                    newRateTargetCurrency.append(`<option value="${currency.code}">${currency.code}</option>`);
                });

                //

                const convertBaseCurrency = $("#convert-base-currency");
                convertBaseCurrency.empty();

                // populate the base currency select element with the list of currencies
                $.each(data, function (index, currency) {
                    convertBaseCurrency.append(`<option value="${currency.code}">${currency.code}</option>`);
                });

                const convertTargetCurrency = $("#convert-target-currency");
                convertTargetCurrency.empty();

                // populate the base currency select element with the list of currencies
                $.each(data, function (index, currency) {
                    convertTargetCurrency.append(`<option value="${currency.code}">${currency.code}</option>`);
                });
            },
            error: function (jqXHR, textStatus, errorThrown) {
                const error = JSON.parse(jqXHR.responseText);
                const toast = $('#api-error-toast');

                $(toast).find('.toast-body').text(error.message);
                toast.toast("show");
            }
        });
    }

    requestCurrencies();

    var Cur = {};
    function AddCurrency() {

        Cur.name = $("#add-currency-name").val();
        Cur.sign = $("#add-currency-sign").val();
        Cur.code = $("#add-currency-code").val();

        $.ajax({
            url: `${host}/currencies`,
            type: "POST",
            contentType: "application/json;charset=utf-8",
            data: JSON.stringify(Cur),
            dataType: "json",
            success: function (data) {
                requestCurrencies();
            },

            error: function (jqXHR, textStatus, errorThrown) {
                const error = JSON.parse(jqXHR.responseText);
                const toast = $('#api-error-toast');

                $(toast).find('.toast-body').text(error.message);
                toast.toast("show");
            }
        });
    }

    $(document).ready(function () {
        $("#btnSave").click(function (e) {

            AddCurrency();
            e.preventDefault();
            console.log(Cur)
        });
    }); 

    function requestExchangeRates() {
        $.ajax({
            url: `${host}/exchangeRates`,
            type: "GET",
            dataType: "json",
            success: function (response) {
                const tbody = $('.exchange-rates-table tbody');
                tbody.empty();
                $.each(response, function (index, rate) {
                    const row = $('<tr></tr>');
                    const currency = rate.baseCurrency.code + rate.targetCurrency.code;
                    const exchangeRate = rate.rate;
                    row.append($('<td></td>').text(currency));
                    row.append($('<td></td>').text(exchangeRate));
                    row.append($('<td></td>').html(
                        '<button class="btn btn-secondary btn-sm exchange-rate-edit"' +
                        'data-bs-toggle="modal" data-bs-target="#edit-exchange-rate-modal">Edit</button>'
                    ));
                    tbody.append(row);
                });
            },
            error: function () {
                const error = JSON.parse(jqXHR.responseText);
                const toast = $('#api-error-toast');

                $(toast).find('.toast-body').text(error.message);
                toast.toast("show");
            }
        });
    }

    requestExchangeRates();

    $(document).delegate('.exchange-rate-edit', 'click', function () {
        // Get the currency and exchange rate from the row
        const pair = $(this).closest('tr').find('td:first').text();
        const exchangeRate = $(this).closest('tr').find('td:eq(1)').text();

        // insert values into the modal
        $('#edit-exchange-rate-modal .modal-title').text(`Edit ${pair} Exchange Rate`);
        $('#edit-exchange-rate-modal #exchange-rate-input').val(exchangeRate);
    });

    // add event handler for edit exchange rate modal "Save" button
    $('#edit-exchange-rate-modal .btn-primary').click(function () {
        // get the currency pair and exchange rate from the modal
        const pair = $('#edit-exchange-rate-modal .modal-title').text().replace('Edit ', '').replace(' Exchange Rate', '');
        const exchangeRate = $('#edit-exchange-rate-modal #exchange-rate-input').val();

        // set changed values to the table row
        const row = $(`tr:contains(${pair})`);
        row.find('td:eq(1)').text(exchangeRate);

        // send values to the server with a patch request
        $.ajax({
            url: `${host}/exchangeRate/${pair}`,
            type: "PATCH",
            contentType: "application/json;charset=utf-8",
            data: `rate=${exchangeRate}`,
            success: function () {

            },
            error: function (jqXHR, textStatus, errorThrown) {
                const error = JSON.parse(jqXHR.responseText);
                const toast = $('#api-error-toast');

                $(toast).find('.toast-body').text(error.message);
                toast.toast("show");
            }
        });

        // close the modal
        $('#edit-exchange-rate-modal').modal('hide');
    });

    var ExchangeRate = {};
    function AddExchangeRate() {

        ExchangeRate.baseCurrencyCode = $("#new-rate-base-currency").val();
        ExchangeRate.targetCurrencyCode = $("#new-rate-target-currency").val();
        ExchangeRate.rate = $("#exchange-rate").val();

        $.ajax({
            url: `${host}/exchangeRates`,
            type: "POST",
            contentType: "application/json;charset=utf-8",
            data: JSON.stringify(ExchangeRate),
            dataType: "json",
            success: function (data) {
                requestExchangeRates();
            },

            error: function (jqXHR, textStatus, errorThrown) {
                const error = JSON.parse(jqXHR.responseText);
                const toast = $('#api-error-toast');

                $(toast).find('.toast-body').text(error.message);
                toast.toast("show");
            }
        });
    }

    $(document).ready(function () {
        $("#btnSaveExchange").click(function (e) {
            console.log(ExchangeRate)
            AddExchangeRate();
            e.preventDefault();
        });
    }); 

    $("#convert").submit(function (e) {
        e.preventDefault();

        const baseCurrency = $("#convert-base-currency").val();
        const targetCurrency = $("#convert-target-currency").val();
        const amount = $("#convert-amount").val();

        $.ajax({
            url: `${host}/exchange/from=${baseCurrency}&to=${targetCurrency}&amount=${amount}`,
            type: "GET",
            // data: "$("#add-exchange-rate").serialize()",
            success: function (data) {
                $("#convert-converted-amount").val(data.convertedAmount);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                const error = JSON.parse(jqXHR.responseText);
                const toast = $('#api-error-toast');

                $(toast).find('.toast-body').text(error.message);
                toast.toast("show");
            }
        });

        return false;
    });
});
