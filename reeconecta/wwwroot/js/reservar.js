$(document).ready(function () {
    $(document).on('click', '.btn-reservar', function () {
        var button = $(this);
        var id = button.data('id');
        var feedbackDiv = button.closest('.produto-card').find('.reserva-feedback');

        feedbackDiv.html('');

        button.prop('disabled', true).text('Reservando...');

        $.ajax({
            url: '/Produtos/Reservar',
            type: 'POST',
            data: { id: id },
            success: function (response) {
                if (response.success) {
                    button.removeClass('btn-primary').addClass('btn-secondary').text('Reservado');

                    var card = $('<div class="card text-white bg-success mb-2"><div class="card-body p-2">' + response.message + '</div></div>');
                    feedbackDiv.append(card);
                    card.fadeIn().delay(3000).fadeOut(500, function () { $(this).remove(); });
                } else {
                    button.prop('disabled', false).text('Tenho interesse');

                    var card = $('<div class="card text-white bg-danger mb-2"><div class="card-body p-2">' + response.message + '</div></div>');
                    feedbackDiv.append(card);
                    card.fadeIn().delay(3000).fadeOut(500, function () { $(this).remove(); });
                }
            },
            error: function () {
                button.prop('disabled', false).text('Tenho interesse');

                var card = $('<div class="card text-white bg-danger mb-2"><div class="card-body p-2">Erro ao tentar reservar o produto.</div></div>');
                feedbackDiv.append(card);
                card.fadeIn().delay(3000).fadeOut(500, function () { $(this).remove(); });
            }
        });
    });
});
