import * as native from 'natives';

export async function drawText3d(
    msg,
    x,
    y,
    z,
    scale,
    fontType,
    r,
    g,
    b,
    a,
    useOutline = true,
    useDropShadow = true
) {
    native.setDrawOrigin(x, y, z, 0);
    native.beginTextCommandDisplayText('STRING');
    native.addTextComponentSubstringPlayerName(msg);
    native.setTextFont(fontType);
    native.setTextScale(1, scale);
    native.setTextProportional(true);
    native.setTextWrap(0.0, 1.0);
    native.setTextCentre(true);
    native.setTextColour(r, g, b, a);

    if (useOutline) 
        native.setTextOutline();

    if (useDropShadow) 
        native.setTextDropShadow();

    native.endTextCommandDisplayText(0, 0, 0);
    native.clearDrawOrigin();
}

export async function drawText2d(
    msg,
    x,
    y,
    scale,
    fontType,
    r,
    g,
    b,
    a,
    useOutline = true,
    useDropShadow = true,
    align = 0,
    useSafeZoneSize = true
) {
    let safeZoneSizeX = 0;
    let safeZoneSizeY = 0;

    if (useSafeZoneSize) {
        safeZoneSizeX = (1.0 - native.getSafeZoneSize()) * 0.5;
        safeZoneSizeY = safeZoneSizeX;

        const [z, x, y] = native.getActiveScreenResolution(0,0);
        if (x != 1920) {
            safeZoneSizeX += ((1920 - x) / 1920) / 10;
        }
    }

    native.beginTextCommandDisplayText('STRING');
    native.addTextComponentSubstringPlayerName(msg);
    native.setTextFont(fontType);
    native.setTextScale(scale, scale);
    native.setTextWrap(0.0, 1.0);
    native.setTextCentre(true);
    native.setTextColour(r, g, b, a);
    native.setTextJustification(align);

    if (useOutline) 
        native.setTextOutline();

    if (useDropShadow) 
        native.setTextDropShadow();

    native.endTextCommandDisplayText(x + safeZoneSizeX, y - safeZoneSizeY, 0);
}