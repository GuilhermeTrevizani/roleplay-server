$('#btn-confirm').click(() => {
    $(`#btn-confirm`).LoadingOverlay('show');

    const params = {
        VehicleParkValue: parseIntExt($('#VehicleParkValue').val()),
        HospitalValue: parseIntExt($('#HospitalValue').val()),
        BarberValue: parseIntExt($('#BarberValue').val()),
        ClothesValue: parseIntExt($('#ClothesValue').val()),
        DriverLicenseBuyValue: parseIntExt($('#DriverLicenseBuyValue').val()),
        Paycheck: parseIntExt($('#Paycheck').val()),
        DriverLicenseRenewValue: parseIntExt($('#DriverLicenseRenewValue').val()),
        AnnouncementValue: parseIntExt($('#AnnouncementValue').val()),
        ExtraPaymentGarbagemanValue: parseIntExt($('#ExtraPaymentGarbagemanValue').val()),
        Blackout: $('#Blackout').is(':checked'),
        KeyValue: parseIntExt($('#KeyValue').val()),
        LockValue: parseIntExt($('#LockValue').val()),
        IPLsJSON: JSON.stringify($('#IPLsJSON').val()),
        FuelValue: parseIntExt($('#FuelValue').val()),
        TattooValue: parseIntExt($('#TattooValue').val()),
        CooldownDismantleHours: parseIntExt($('#CooldownDismantleHours').val()),
        PropertyRobberyConnectedTime: parseIntExt($('#PropertyRobberyConnectedTime').val()),
        CooldownPropertyRobberyRobberHours: parseIntExt($('#CooldownPropertyRobberyRobberHours').val()),
        CooldownPropertyRobberyPropertyHours: parseIntExt($('#CooldownPropertyRobberyPropertyHours').val()),
        PoliceOfficersPropertyRobbery: parseIntExt($('#PoliceOfficersPropertyRobbery').val()),
        InitialTimeCrackDen: parseIntExt($('#InitialTimeCrackDen').val()),
        EndTimeCrackDen: parseIntExt($('#EndTimeCrackDen').val()),
        FirefightersBlockHeal: parseIntExt($('#FirefightersBlockHeal').val()),
    };

    alt.emit('save', JSON.stringify(params));
});

$('#IPLsJSON').select2({
    tags: true
});

function loaded(param) {
    $('#MaxCharactersOnline').val(param.MaxCharactersOnline);
    $('#VehicleParkValue').val(param.VehicleParkValue);
    $('#HospitalValue').val(param.HospitalValue);
    $('#BarberValue').val(param.BarberValue);
    $('#ClothesValue').val(param.ClothesValue);
    $('#DriverLicenseBuyValue').val(param.DriverLicenseBuyValue);
    $('#FuelValue').val(param.FuelValue);
    $('#Paycheck').val(param.Paycheck);
    $('#DriverLicenseRenewValue').val(param.DriverLicenseRenewValue);
    $('#AnnouncementValue').val(param.AnnouncementValue);
    $('#ExtraPaymentGarbagemanValue').val(param.ExtraPaymentGarbagemanValue);
    $('#KeyValue').val(param.KeyValue);
    $('#LockValue').val(param.LockValue);
    $('#TattooValue').val(param.TattooValue);
    $('#CooldownDismantleHours').val(param.CooldownDismantleHours);
    $('#PropertyRobberyConnectedTime').val(param.PropertyRobberyConnectedTime);
    $('#CooldownPropertyRobberyRobberHours').val(param.CooldownPropertyRobberyRobberHours);
    $('#CooldownPropertyRobberyPropertyHours').val(param.CooldownPropertyRobberyPropertyHours);
    $('#PoliceOfficersPropertyRobbery').val(param.PoliceOfficersPropertyRobbery);
    $('#InitialTimeCrackDen').val(param.InitialTimeCrackDen);
    $('#EndTimeCrackDen').val(param.EndTimeCrackDen);
    $('#FirefightersBlockHeal').val(param.FirefightersBlockHeal);

    if (param.Blackout)
        $('#Blackout').attr('checked', 'checked');

    const ipls = JSON.parse(param.IPLsJSON);
    ipls.forEach((x) => {
        $('#IPLsJSON').append(`<option value="${x}" selected>${x}</option>`);
    });
}

function mostrarMensagem(message) {
    $(`#btn-confirm`).LoadingOverlay('hide');
    $.alert(message);
}

if('alt' in window) {
    alt.on('loaded', loaded);
    alt.on('mostrarMensagem', mostrarMensagem);
}