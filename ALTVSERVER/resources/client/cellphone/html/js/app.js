$(window).keyup((e) => {
  if (e.which === 27) {
    $('#smartphone').animate({ bottom: '-29vw', opacity: '0' }, 250);
    if ('alt' in window)
      alt.emit('smartphone:close');
  }
});

let isCharging = false;
let isFlightMode = false;
let audioVolume = 0.25;

let audioClick = new Audio('./audio/click.mp3');
audioClick.volume = audioVolume;

let noAppScreen = [
  'appAirplaneButton',
];

if ('alt' in window) {
  alt.emit('smartphone:ready');

  alt.on('smartphone:updateContacts', (cellphone, flightMode, contacts) => {
    $('#notch').text(cellphone);
    receiveContacts(contacts);
    isFlightMode = flightMode;
    if (isFlightMode)
      $('#signal').removeClass('fa-signal').addClass('fa-plane');
    else
      $('#signal').removeClass('fa-plane').addClass('fa-signal');
  });

  alt.on('smartphone:update', (currentTime, temp, weather) => {
    $('#clock').text(currentTime);
    $('#temperature').text(temp);
    $('#appWeatherButton > i').removeClass().addClass(`fas fa-${weather}`);
  });

  alt.on('smartphone:show', () => {
    $('#smartphone').animate({ bottom: '2vw', opacity: '1' }, 250);
  });
}

function toggleHomeScreen(screen = null) {
  if ($('#homeScreen').is(':visible')) {
    $('#homeScreen').hide();
    $('#appScreen').show();
    $('#appScreen > div').hide();
    $('#homeButton').show();
    if (screen)
      $((showScreen = '#' + screen.replace('Button', ''))).show();
  } else {
    $('#homeScreen').show();
    $('#appScreen').hide();
    $('#homeButton').hide();
  }
}

$('#appAirplaneButton').click(() => {
  if (isFlightMode) {
    isFlightMode = false;
    $('#signal').removeClass('fa-plane').addClass('fa-signal');
  } else {
    isFlightMode = true;
    $('#signal').removeClass('fa-signal').addClass('fa-plane');
  }

  if ('alt' in window)
    alt.emit('smartphone:flightmode', isFlightMode);
});

$('#homeButton').click(() => {
  audioClick.play();
  toggleHomeScreen();
});

$('.app').click(function () {
  audioClick.play();

  if (!noAppScreen.includes($(this).attr('id')))
    toggleHomeScreen($(this).attr('id'));
});

$('#appContactsSearch').on('keyup change', function () {
  let searchString = removerAcentos($(this).val().toLowerCase());
  if (searchString.length) {
    $('.contact').hide();
    $('.contact').each(function () {
      if (removerAcentos($(this).text()).toLowerCase().includes(searchString))
        $(this).show();
    });
  } else {
    $('.contact').show();
  }
});

$('#appContactsButton').click(() => {
  $('#appContactsEdit').hide();
  $('#appContactsOverview').show();
});

$('#appGpsButton').click(() => {
  $('#appGpsOverview').show();
});

$('#appWeatherButton').click(() => {
  $('#appWeatherOverview').show();
});

$(document).on('click', '.contact', function () {
  showEditContact($(this).attr('data-numero'), $(this).attr('data-nome'));
});

$('#editContactHead').click(() => {
  hideEditContact();
});

$('#editContactSaveButton').click(() => {
  if ('alt' in window)
    alt.emit('smartphone:contacts:update', Number($('#contactNewPhone').val()), $('#contactNewName').val());
  hideEditContact();
});

$('#editContactDeleteButton').click(() => {
  if ('alt' in window)
    alt.emit('smartphone:contacts:delete', Number($('#contactNewPhone').val()));
  hideEditContact();
});

$('#editContactLocationButton').click(() => {
  if ('alt' in window)
    alt.emit('smartphone:contacts:location', Number($('#contactNewPhone').val()));
});

$('#editContactCallButton').click(() => {
  if ('alt' in window)
    alt.emit('smartphone:contacts:call', Number($('#contactNewPhone').val()));
});

$('#rastrearPropriedade').click(() => {
  if ('alt' in window)
    alt.emit('smartphone:gps', Number($('#numeroPropriedade').val()));
});

$('#btnNovoContato').click(() => {
  showEditContact('', '');
});

function showEditContact(numero, nome) {
  $('#contactNewName').val(nome);
  $('#editContactLetter > span').text(nome[0] ?? '');
  $('#contactNewPhone').val(numero);
  $('#appContactsOverview').hide();
  $('#appContactsEdit').show();
}

function hideEditContact() {
  $('#contactNewName').val('');
  $('#contactNewPhone').val('');
  $('#appContactsOverview').show();
  $('#appContactsEdit').hide();
  $('#appContactsSearch').val('');
  $('.contact').show();
}

function receiveContacts(data) {
  let newHTML = '';
  data.forEach((d) => {
    newHTML += `
		<div class="contact" data-nome="${d.Nome}" data-numero="${d.Numero}">
			${d.Nome}
		</div>`;
  });
  $('#appContactsList').html(newHTML);
}

function removerAcentos(s) {
  return s.normalize('NFD').replace(/[\u0300-\u036f]/g, "");
}