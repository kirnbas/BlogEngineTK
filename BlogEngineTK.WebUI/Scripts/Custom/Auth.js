window.onload = function ()
{
    var refreshCaptcha = document.getElementById("refreshCaptcha");
    refreshCaptcha.onclick = reloadCaptcha;
};

function reloadCaptcha()
{
    var myImageElement = document.getElementById('imgCaptcha');
    myImageElement.src = '../captcha.axd?rand=' + Math.random();
}