function showPlayers(nomeServidor, x, rodape) {
    let players = JSON.parse(x);
    $('#title').text(`${nomeServidor} • Jogadores Online (${players.length})`);
    $('#bottom').text(rodape);
    $("#onlineplayers").html('');
    players.forEach(function(p) {
	    $("#onlineplayers").append(`<tr><th scope="row">${p.ID}</th><td>${p.Nome}</td><td>${p.Ping}</td></tr>`);
    });
}

if('alt' in window)
    alt.on('showPlayers', showPlayers);