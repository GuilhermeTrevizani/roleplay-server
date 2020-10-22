toastr.options = {
    "closeButton": false,
    "debug": false,
    "newestOnTop": false,
    "progressBar": false,
    "positionClass": "toast-top-right",
    "preventDuplicates": false,
    "onclick": null,
    "showDuration": "300",
    "hideDuration": "1000",
    "timeOut": "5000",
    "extendedTimeOut": "1000",
    "showEasing": "swing",
    "hideEasing": "linear",
    "showMethod": "fadeIn",
    "hideMethod": "fadeOut"
};

function iniciarLoadingOverlay(obj = null) {
    if (obj != null)
        $(obj).LoadingOverlay("show", { imageColor: "#AE6AB2" });
    else
        $.LoadingOverlay("show", { imageColor: "#AE6AB2" });
}

function finalizarLoadingOverlay(obj = null) {
    if (obj != null)
        $.LoadingOverlay("hide");
    else
        $.LoadingOverlay("hide");
}