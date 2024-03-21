import * as alt from 'alt-client';
import * as native from 'natives';
import { createPedEditCamera, destroyPedEditCamera, setFov, setZPos } from '/helpers/camera.js';
import { view, setView, toggleView, closeView } from '/helpers/cursor.js';
import { activateChat } from '/chat/index.js';

let sexo = 0;
let personalization;
let estudio;

alt.onServer('OpenTattoo', (sex, _personalization) => {
  native.displayHud(false);
  native.displayRadar(false);
  activateChat(false);
  alt.emit('character:EditTattoos', sex, JSON.parse(_personalization), true);
});

alt.onServer('Character:CloseTattoos', handleClose);

alt.on('character:EditTattoos', handleEdit);

function handleEdit(_sexo, _personalization, _estudio = false) {
  sexo = _sexo;
  personalization = _personalization;
  estudio = _estudio;

  syncDecorations(personalization, false);
  setView(new alt.WebView('http://resource/tattoos/html/index.html'));
  view.on('load', () => {
    view.emit('character:SetData', sexo);
  });
  view.on('character:Done', handleDone);
  view.on('character:Cancel', handleCancel);
  view.on('character:Sync', handleSync);
  view.on('character:Everything', handleEverything);
  view.focus();

  toggleView(true, false);
  native.setEntityHeading(alt.Player.local, 169.24);
  createPedEditCamera(false);
  setFov(50);
  setZPos(0.6);
}

function handleClose() {
  closeView();
  destroyPedEditCamera();
}

function handleDone(personalization) {
  alt.emitServer('ConfirmTattoos', JSON.stringify(personalization), estudio, true);
}

function handleCancel() {
  alt.emitServer('ConfirmTattoos', '', estudio, false);
}

function handleSync(tattoo) {
  syncDecorations(personalization, false);

  addDecoration(tattoo.Collection, tattoo.Overlay);
}

function handleEverything(tattoos) {
  syncDecorations(personalization);

  tattoos.forEach(x => {
    addDecoration(x.Collection, x.Overlay);
  });
}

function addDecoration(collection, overlay) {
  alt.emitServer('AddDecoration', collection, overlay);
}

function syncDecorations(personalization, setTattoos = true) {
  alt.emitServer('ClearDecorations');

  if (personalization.HairOverlay && personalization.HairCollection)
    addDecoration(personalization.HairCollection, personalization.HairOverlay);

  if (setTattoos)
    personalization.Tattoos.forEach(x => {
      addDecoration(x.Collection, x.Overlay);
    });
}


alt.onServer('Character:ShowMessage', (message) => {
  view.emit('character:ShowMessage', message);
});