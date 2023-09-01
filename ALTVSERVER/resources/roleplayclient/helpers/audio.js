import * as alt from 'alt-client'

let audioSpots = [];

alt.onServer('Audio:Setup', (id, position, source, dimension, volume, vehicleId, loop) => {
    const x = audioSpots.findIndex(x => x.id === id);
    if (x === -1) {
        const audio = new alt.Audio(source, volume);
        audio.looped = loop;

        if (vehicleId) {
            const vehicle = alt.Vehicle.getByRemoteID(vehicleId);
            audio.addOutput(new alt.AudioOutputAttached(vehicle));
        } else {
            audio.addOutput(new alt.AudioOutputWorld(position))
        }

        audio.play();

        audioSpots.push({ id, position, source, dimension, volume, vehicleId, loop, audio });
    } else {
        audioSpots[x].position = position;
        audioSpots[x].source = source;
        audioSpots[x].dimension = dimension;
        audioSpots[x].volume = volume;
        audioSpots[x].vehicleId = vehicleId;
        audioSpots[x].loop = loop;
    }
});

alt.onServer('Audio:Remove', (id) => {
    const x = audioSpots.findIndex(x => x.id === id);
    if (x === -1)
        return;

    audioSpots[x].audio.destroy();
    audioSpots.splice(x, 1);
});