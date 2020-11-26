window.captureAndUpload = (fileName) => {
    window.html2canvas(document.getElementById("chart"))
        .then(canvas => {
            var a = document.createElement('a');
            // toDataURL defaults to png, so we need to request a jpeg, then convert for file download.
            a.href = canvas.toDataURL("image/jpeg").replace("image/jpeg", "image/octet-stream");
            a.download = fileName + '.png';
            a.click();
        });
    
};

window.captureAndDisplay = () => {
    window.html2canvas({
        onrendered: function (canvas) {
            var myImage = canvas.toDataURL("image/png");
            window.open(myImage);
        }
    });
};
