import * as alt from 'alt-client';
import * as native from 'natives';
import { createPedEditCamera, destroyPedEditCamera, setFov, setZPos } from '/helpers/camera.js';
import { view, setView, toggleView, closeView } from '/helpers/cursor.js';
import { activateChat } from '/chat/index.js';

let sexo;
let oldData = {};
let barbearia;

alt.onServer('AbrirBarbearia', (sexo, personalizacao) => {
  native.displayHud(false);
  native.displayRadar(false);
  activateChat(false);
  alt.emit('character:Edit', sexo, JSON.parse(personalizacao), true);
});

alt.on('character:Edit', handleEdit);

async function handleEdit(_sexo, _oldData, _barbearia) {
  sexo = _sexo;
  oldData = _oldData;
  barbearia = _barbearia;

  setView(new alt.WebView('http://resource/charcreator/html/index.html'));
  view.on('load', handleReadyDone);
  view.on('character:Done', handleDone);
  view.on('character:Cancel', handleCancel);
  view.on('character:Sync', handleSync);
  view.focus();
  toggleView(true, false);

  if (!barbearia)
    return;

  configureCamera();
}

function configureCamera() {
  createPedEditCamera();
  setFov(50);
  setZPos(0.6);
}

alt.once('spawned', () => {
  native.setEntityHeading(alt.Player.local, 169.24);
  configureCamera();
});

function closeEditor() {
  closeView();
  destroyPedEditCamera();
}

function handleDone(newData) {
  alt.emitServer('ConfirmarPersonalizacao', JSON.stringify(newData), barbearia ? 1 : 0, true);
  closeEditor();
}

function handleCancel() {
  alt.emitServer('ConfirmarPersonalizacao', '', barbearia ? 1 : 0, false);
  closeEditor();
}

function handleReadyDone() {
  view.emit('character:SetData', sexo, oldData, barbearia);
}

function handleSync(data) {
  alt.emitServer('SetarPersonalizacao', JSON.stringify(data));
}