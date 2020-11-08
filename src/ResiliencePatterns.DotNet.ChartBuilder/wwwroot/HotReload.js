window.Blazor.defaultReconnectionHandler.onConnectionDown = function () {
    setTimeout(function() {
        window.location.reload();
    }, 2000);
};