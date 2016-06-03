function attach() {

    var link = document.createElement("link");
    link.href = "../playerStyles.css";
    link.rel = "stylesheet";
    link.type = "text/css"; // no need for HTML5
    document.head.appendChild(link);


    var toolbar = document.createElement("div");
    document.body.appendChild(toolbar);
    toolbar.outerHTML = "<div id='playerToolbar'>Home</div>";

    //show the first page
    var current = document.body.getElementsByClassName("bloom-page")[0];
    current.classList.add("currentPage");

    var animationStyle = document.createElement("style");
    animationStyle.id = "bloom-animationSheet";
    animationStyle.type = "text/css";
    document.body.appendChild(animationStyle);

    var stylesheet = document.getElementById("bloom-animationSheet").sheet;

    var animationView = current.getElementsByClassName("bloom-imageView")[0];

    var initialRect = animationView.dataset.initialrect.split(" ");
    var initialScaleWidth = 922 / initialRect[2];
    var initialScaleHeight = 677 / initialRect[3];

    var finalRect = animationView.dataset.finalrect.split(" ");
    var finalScaleWidth = 922 / finalRect[2];
    var finalScaleHeight = 677 / finalRect[3];

    var initialTransform = "scale(" + initialScaleWidth + ", " + initialScaleHeight + ") translate(" + initialRect[0] + "px, " + initialRect[1] + "px)";
    var finalTransform = "scale(" + finalScaleWidth + ", " + finalScaleHeight + ") translate(" + finalRect[0] + "px, " + finalRect[1] + "px)";

    stylesheet.insertRule("@keyframes movepic {\
                    from{\
                        transform-origin: 0px 0px;\
                        transform: "+ initialTransform + ";\
                    }\
                    to{\
                        transform-origin: 0px 0px;\
                        transform: "+ finalTransform + ";\
                    }\
                }", 0);

    stylesheet.insertRule(".bloom-imageView {\
                    transform-origin: 0px 0px;\
                    transform: " + initialTransform + ";\
                    animation-name: movepic;\
                    animation-duration: "+ animationView.dataset.duration + "s;\
                    animation-fill-mode: forwards;\
                }", 1);

    stylesheet.insertRule(".bloom-imageContainer {\
                    width: 922px !important;\
                    height: 677px !important;\
                    overflow: hidden !important;\
                }", 2);

    stylesheet.insertRule(".marginBox IMG {\
                    max-width: none !important;\
                }", 3);

    document.getElementById("playerToolbar").onclick = function (event) {
        event.stopPropagation();
        [].forEach.call(document.body.querySelectorAll(".bloom-page"), function (page) {
            page.classList.remove("currentPage");
        });
        document.body.getElementsByClassName("bloom-page")[0].classList.add("currentPage");
    }

    document.body.onclick = function (event) {
        current = document.body.getElementsByClassName("currentPage")[0];
        if (current) {
            current.classList.remove("currentPage");
            current = current.nextElementSibling;
            if (current) {
                current.classList.add("currentPage");

                stylesheet.deleteRule(0);
                stylesheet.deleteRule(0);

                animationView = current.getElementsByClassName("bloom-imageView")[0];

                initialRect = animationView.dataset.initialrect.split(" ");
                initialScaleWidth = 922 / initialRect[2];
                initialScaleHeight = 677 / initialRect[3];

                finalRect = animationView.dataset.finalrect.split(" ");
                finalScaleWidth = 922 / finalRect[2];
                finalScaleHeight = 677 / finalRect[3];

                initialTransform = "scale(" + initialScaleWidth + ", " + initialScaleHeight + ") translate(" + initialRect[0] + "px, " + initialRect[1] + "px)";
                finalTransform = "scale(" + finalScaleWidth + ", " + finalScaleHeight + ") translate(" + finalRect[0] + "px, " + finalRect[1] + "px)";

                stylesheet.insertRule("@keyframes movepic {\
                    from{\
                        transform-origin: 0px 0px;\
                        transform: "+ initialTransform + ";\
                    }\
                    to{\
                        transform-origin: 0px 0px;\
                        transform: "+ finalTransform + ";\
                    }\
                }", 0);

                stylesheet.insertRule(".bloom-imageView {\
                    transform-origin: 0px 0px;\
                    transform: " + initialTransform + ";\
                    animation-name: movepic;\
                    animation-duration: "+ animationView.dataset.duration + "s;\
                    animation-fill-mode: forwards;\
                }", 1);
            }
        }
    }
    resize();
}

function resize() {
    var pageWidth = document.querySelectorAll(".bloom-page")[0].scrollWidth;
    var pageHeight = document.querySelectorAll(".bloom-page")[0].scrollHeight;
    //var scale = window.scrollWidth / pageWidth;
    var scale = Math.min(window.innerWidth / pageWidth, window.innerHeight / pageHeight);
    //   var scale = window.innerWidth/pageWidth;
    document.body.style.transform = "scale(" + scale + ")";
}

document.addEventListener("DOMContentLoaded", attach, false);

window.addEventListener("resize", function () { resize() }, false);
