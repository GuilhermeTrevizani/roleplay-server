import * as alt from 'alt-client';
import * as native from 'natives';
import { createPedEditCamera, destroyPedEditCamera, setFov, setZPos } from '/helpers/camera.js';
import { view, setView, toggleView, closeView } from '/helpers/cursor.js';
import { activateChat } from '/chat/index.js';

let sexo;
let oldData = {};
let barbearia;
let registerInterval;

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

  if (!barbearia) {
    native.freezeEntityPosition(alt.Player.local, true);

    if (registerInterval) {
      alt.clearInterval(registerInterval);
      registerInterval = null;
    }

    registerInterval = alt.setInterval(() => {
      if (alt.Player.local.isSpawned) {
        native.freezeEntityPosition(alt.Player.local, false);
        native.setEntityHeading(alt.Player.local, 169.24);
        configureCamera();
        alt.clearInterval(registerInterval);
        registerInterval = null;
      }
    }, 500);

    return;
  }

  configureCamera();
}

function configureCamera() {
  createPedEditCamera();
  setFov(50);
  setZPos(-0.38);
}

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
