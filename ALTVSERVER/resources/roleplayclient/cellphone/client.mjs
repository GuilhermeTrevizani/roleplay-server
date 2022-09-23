import * as alt from 'alt-client';
import { showCursor } from '/helpers/cursor.js';
import { toggleInput } from '/chat/index.mjs';

let webView = null;
export let isCellphoneOpened = false;
let cellphone = 0;
let temp = 0;
let weather = '';
let time = '';
let contacts = [];
let flightMode = false;

alt.onServer('ToggleViewCellphone', (_cellphone, _flightMode) => {
	cellphone = _cellphone;
	flightMode = _flightMode;

	if ((webView && cellphone != 0) || (!webView && cellphone == 0))
		return;

	if (cellphone != 0) {
		webView = new alt.WebView('http://resource/cellphone/html/smartphone.html');

		webView.on('smartphone:ready', () => {
			updateData();
			updateContacts();
		});

		webView.on('smartphone:close', () => {
			toggleView(false);
			webView.unfocus();
			isCellphoneOpened = false;
		});
		webView.on('smartphone:contacts:update', (numero, nome) => {
			const found = contacts.find(x => x.Numero === numero);
			if (found) {
				found.Nome = nome;
				found.Numero = numero;
			} else {
				contacts.push({
					Numero: numero,
					Nome: nome
				});
			}
			contacts = contacts.sort((a,b) => (a.Nome > b.Nome) ? 1 : ((b.Nome > a.Nome) ? -1 : 0));
			alt.emitServer('AdicionarContatoCelular', numero, nome);
			updateContacts();
		});

		webView.on('smartphone:contacts:delete', (numero) => {
			const found = contacts.find(x => x.Numero === numero);
			if (found)
				contacts.splice(contacts.indexOf(found), 1);
			alt.emitServer('RemoverContatoCelular', numero);
			updateContacts();
		});

		webView.on('smartphone:contacts:call', (number) => {
			alt.emitServer('LigarContatoCelular', number);
		});

		webView.on('smartphone:contacts:location', (number) => {
			alt.emitServer('EnviarLocalizacaoContatoCelular', number);
		});

		webView.on('smartphone:flightmode', (isFlightMode) => {
			alt.emitServer('ModoAviaoCelular', isFlightMode);
		});

		webView.on('smartphone:gps', (property) => {
			alt.emitServer('RastrearPropriedade', property);
		});
	} else {
		webView.destroy();
		webView = null;
	}
});

alt.onServer('OpenCellphone', (_flightMode, _contacts) => {
	isCellphoneOpened = true;
	webView.emit('smartphone:show');
	alt.toggleGameControls(false);
	toggleView(true);
	webView.focus();
	flightMode = _flightMode;
	contacts = JSON.parse(_contacts);
	time = new Date().toLocaleTimeString('en-US', { hour12: false, hour: "numeric", minute: "numeric"});
	temp = alt.getSyncedMeta('Temperature');
	weather = alt.getSyncedMeta('WeatherType');
	updateContacts();
	updateData();
});

function toggleView(show) {
    showCursor(show);
    alt.toggleGameControls(!show);
    toggleInput(!show);
}

export function updateCellphone() {
	time = new Date().toLocaleTimeString('en-US', { hour12: false, hour: "numeric", minute: "numeric"});
	temp = alt.getSyncedMeta('Temperature');
	weather = alt.getSyncedMeta('WeatherType');
	updateData();
}

function updateContacts() {
	if (!webView)
		return;

	webView.emit('smartphone:updateContacts', cellphone, flightMode, contacts);
}

function updateData() {
	if (!webView)
		return;

	let weatherType = 'sun';
	if (weather == 'CLEARING')
		weatherType = 'cloud-sun-rain';
	else if (weather == 'CLOUDS')
		weatherType = 'cloud';
	else if (weather == 'RAIN')
		weatherType = 'cloud-rain';
	else if (weather == 'THUNDER' || weather == 'OVERCAST')
		weatherType = 'bolt';
	else if (weather == 'FOGGY')
		weatherType = 'wind';
	else if (weather == 'SMOG')
		weatherType = 'smog';
	else if (weather == 'SNOW' || weather == 'BLIZZARD')
		weatherType = 'snowflake';

	webView.emit('smartphone:update', time, temp, weatherType);
}