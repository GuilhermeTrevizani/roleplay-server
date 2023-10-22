$(window).keyup((e) => {
    if (e.which === 27)
        closeView();
});

function loaded(truckerLocations) {
    $('#tbody-truckerlocations').html(truckerLocations);
}

function track(posX, posY) {
    alt.emit('track', posX, posY);
}

function closeView() {
    alt.emit('closeView');
}

if('alt' in window)
    alt.on('loaded', loaded);