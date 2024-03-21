import alt from 'alt-client';
import { view, setView, toggleView, closeView } from '/helpers/cursor.js';

alt.onServer('StaffShowFactionRanks', (update, ranksHtml, factionId, factionName) => {
  if (update) {
    if (view?.url == 'http://resource/staff/factionrank/index.html')
      view.emit('loaded', ranksHtml);

    return;
  }

  setView(new alt.WebView('http://resource/staff/factionrank/index.html'));
  view.on('load', () => {
    view.emit('loaded', ranksHtml);
    view.emit('init', factionName);
  });
  view.on('closeView', closeView);
  view.on('save', (factionRankId, name, salary) => {
    alt.emitServer('StaffFactionRankSave', factionId, factionRankId, name, salary);
  });
  view.on('remove', (factionRankId) => {
    alt.emitServer('StaffFactionRankRemove', factionRankId);
  });
  view.on('order', (ranksJSON) => {
    alt.emitServer('StaffFactionRankOrder', factionId, ranksJSON);
  });
  view.focus();
  toggleView(true);
});