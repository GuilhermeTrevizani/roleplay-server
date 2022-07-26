$(window).keyup((e) => {
    if (e.which === 27)
        closeView();
});

function showHTML(title, items) {
    $('#h3-title').html(title);
    items.forEach(function(p) {
	    $("#tbody-itens").append(`<tr>
            <td>${p.Name}</td> 
            <td>${p.Value}</td> 
            <td class="text-center">
                <button class="btn btn-xs btn-primary" type="button" onclick="buy('${p.Name}')">Comprar</button>
            </td>
        </tr>`);
    });
}

function buy(item) {
    alt.emit("buy", item);
}

function closeView() {
    alt.emit('closeView');
}

if('alt' in window) 
    alt.on('showHTML', showHTML);