let perguntas;

function showHTML(x) {
  perguntas = JSON.parse(x);
  $("#perguntas").html('');
  perguntas.forEach(function (p) {
    $("#perguntas").append(`<p class='mar-top'><strong>${p.Name}</strong></p>`);
    p.Answers.forEach(function (r) {
      $("#perguntas").append(`<div class='radio'>
                <input type='radio' class='magic-radio' id='resposta${r.Id}' name='pergunta${p.Id}' value='${r.Id}'>
                <label for='resposta${r.Id}'>${r.Name}</label>
            </div>`);
    });
  });
}

function confirmar() {
  $('#btn-confirmar').LoadingOverlay('show');

  let temSemResposta = false;
  let acertos = 0;

  perguntas.forEach((p) => {
    const answer = parseInt($(`input[name='pergunta${p.Id}']:checked`).val());
    if (isNaN(answer))
      temSemResposta = true;
    else if (answer == p.CorrectQuestionAnswerId)
      acertos++;
  });

  if (temSemResposta) {
    mostrarErro("Verifique se todas as perguntas foram respondidas.", false);
    return;
  }

  if (acertos < perguntas.length) {
    mostrarErro(`Você não acertou todas as perguntas. Acertos: ${acertos} de ${perguntas.length}.`, true);
    return;
  }

  alt.emit('confirmar');
}

function copiar() {
  var copyText = document.getElementById("link");
  copyText.select();
  document.execCommand("copy");
}

function mostrarErro(erro, limpar = true) {
  $('#btn-confirmar').LoadingOverlay('hide');
  $.alert(erro);

  if (limpar)
    $('input').prop('checked', false);
}

if ('alt' in window) {
  alt.on('showHTML', showHTML);
  alt.on('mostrarErro', mostrarErro);
}