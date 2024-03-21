import alt from 'alt-client';
import { view, setView, toggleView, closeView } from '/helpers/cursor.js';

alt.onServer('StaffShowFactionMembers', (update, membersHtml, factionId, factionName, government, ranksJSON, flagsJSON) => {
  if (update) {
    if (view?.url == 'http://resource/staff/factionmember/index.html')
      view.emit('loaded', membersHtml);

    return;
  }

  setView(new alt.WebView('http://resource/staff/factionmember/index.html'));
  view.on('load', () => {
    view.emit('init', factionName, JSON.parse(ranksJSON), JSON.parse(flagsJSON), government);
    view.emit('loaded', membersHtml);
  });
  view.on('closeView', closeView);
  view.on('invite', (characterId) => {
    alt.emitServer('StaffFactionMemberInvite', factionId, characterId);
  });
  view.on('save', (characterId, factionRankId, badge, flags) => {
    alt.emitServer('StaffFactionMemberSave', factionId, characterId, factionRankId, badge, flags);
  });
  view.on('remove', (characterId) => {
    alt.emitServer('StaffFactionMemberRemove', factionId, characterId);
  });
  view.focus();
  toggleView(true);
});