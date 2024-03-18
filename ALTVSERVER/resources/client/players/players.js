$(window).keyup((e) => {
  if (e.which === 27 || e.which === 9)
    alt.emit('close');
});

function showPlayers(nomeServidor, players, rodape) {
  $('#title').text(`${nomeServidor} • Jogadores Online (${players.length})`);
  $('#bottom').text(rodape);
  $("#onlineplayers").html('');
  players.forEach(function (p) {
    $("#onlineplayers").append(`<tr>
            <th scope="row">${p.SessionId}</th>
            <td>${p.ICName}</td>
            <td>${p.Ping}</td>
        </tr>`);
  });
}

if ('alt' in window)
  alt.on('showPlayers', showPlayers);