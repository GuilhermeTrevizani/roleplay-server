import alt from 'alt-client';
import { view, setView, toggleView, closeView } from '/helpers/cursor.js';

alt.onServer('ACPShow', (flags, bans, sos, logTypes, staff) => {
  setView(new alt.WebView('http://resource/staff/acp/index.html'));
  view.on('load', () => {
    view.emit('updateFlags', JSON.parse(flags));
    view.emit('updateBans', JSON.parse(bans));
    view.emit('updateSOS', JSON.parse(sos));
    view.emit('updateLogTypes', JSON.parse(logTypes));
    view.emit('updateStaff', JSON.parse(staff));
  });
  view.on('closeView', closeView);
  view.on('unban', (id, total) => {
    alt.emitServer('StaffUnban', id, total);
  });
  view.on('searchLogs', (dataInicial, dataFinal, tipo, personagemOrigem, personagemDestino, descricao) => {
    alt.emitServer('StaffSearchLogs', dataInicial, dataFinal, tipo, personagemOrigem, personagemDestino, descricao);
  });
  view.on('searchUser', (search) => {
    alt.emitServer('StaffSearchUser', search);
  });
  view.on('searchCharacter', (search) => {
    alt.emitServer('StaffSearchCharacter', search);
  });
  view.on('banCharacter', (id, days, reason) => {
    alt.emitServer('StaffBanCharacter', id, days, reason);
  });
  view.on('saveUser', (userId, staff, flagsJSON) => {
    alt.emitServer('StaffSaveUser', userId, staff, flagsJSON);
  });
  view.on('ckCharacter', (id, reason) => {
    alt.emitServer('StaffCKCharacter', id, reason);
  });
  view.on('ckAvalationRemoveCharacter', (id) => {
    alt.emitServer('StaffCKAvalationRemoveCharacter', id);
  });
  view.on('ckAvalationCharacter', (id) => {
    alt.emitServer('StaffCKAvalationCharacter', id);
  });
  view.on('nameChangeStatusCharacter', (id) => {
    alt.emitServer('StaffNameChangeStatusCharacter', id);
  });
  view.on('removeForumNameChange', (id) => {
    alt.emitServer('StaffRemoveForumNameChangeUser', id);
  });
  view.on('removeJailCharacter', (id) => {
    alt.emitServer('StaffRemoveJailCharacter', id);
  });
  view.focus();
  toggleView(true);
});

alt.onServer('ACPUpdateBans', (bans) => {
  if (view?.url == 'http://resource/staff/acp/index.html')
    view.emit('updateBans', JSON.parse(bans));
});

alt.onServer('ACPUpdateSOS', (sos) => {
  if (view?.url == 'http://resource/staff/acp/index.html')
    view.emit('updateSOS', JSON.parse(sos));
});

alt.onServer('ACPUpdateLogs', (logs) => {
  if (view?.url == 'http://resource/staff/acp/index.html')
    view.emit('updateLogs', JSON.parse(logs));
});

alt.onServer('ACPUpdateUser', (html) => {
  if (view?.url == 'http://resource/staff/acp/index.html')
    view.emit('updateUser', html);
});

alt.onServer('ACPUpdateCharacter', (html) => {
  if (view?.url == 'http://resource/staff/acp/index.html')
    view.emit('updateCharacter', html);
});