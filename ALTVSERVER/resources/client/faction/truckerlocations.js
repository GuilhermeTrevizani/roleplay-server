function loaded(truckerLocations) {
  $('#tbody-truckerlocations').html(truckerLocations);
}

function track(posX, posY) {
  alt.emit('track', posX, posY);
}

if ('alt' in window)
  alt.on('loaded', loaded);