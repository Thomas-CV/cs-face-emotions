function setAsFormatedJson(data, destination) {
    if (data) document.getElementById(destination).innerHTML = JSON.stringify(data, null, 4);
}

function attachLoaderOnClick(imageIndex, destination, loader) {
    document.getElementById(destination).addEventListener("click", function () {
        document.getElementById(loader).style.visibility = "visible";
        window.location.href = '/' + imageIndex;
    });
}

var isDrown = 0;
function attachShowBoundingBoxes(data, button, destination) {
    document.getElementById(button).addEventListener("click", function () {
        var can = document.getElementById(destination);
        if (can.style.visibility === "visible") can.style.visibility = "hidden";
        else {
            can.style.visibility = "visible";
            if (isDrown === 1) return;

            var ctx = can.getContext('2d');
            ctx.strokeStyle = "rgba(255,0,0,0.9)";
            ctx.fillStyle = "rgba(255,0,0,0.3)";

            var blk = data.split(',');
            for (var i = 0; i < blk.length; i += 4) {
                ctx.rect(blk[i + 1], blk[i], blk[i + 2], blk[i + 3]);
                ctx.stroke();
                ctx.fill();
            }
            isDrown = 1;
        }
    });
}