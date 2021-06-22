setTimeout(SetHref, 1000);

//Write a function that allows me to link back to the Blog from a custom image
function SetHref() {
    let anchor = document.querySelector(".topbar-wrapper a");
    anchor.href = "/Home/Index";
}

//Set favicon
const favicon = document.querySelectorAll('[rel="icon"]')[0];
favicon.setAttribute('href', '/swagger/favicon-32x32.png');