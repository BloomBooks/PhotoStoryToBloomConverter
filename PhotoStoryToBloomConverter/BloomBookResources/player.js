function attach(){
    
    var link = document.createElement("link");
    link.href = "../playerStyles.css";
    link.rel = "stylesheet";
    link.type = "text/css"; // no need for HTML5
    document.head.appendChild(link);

    var animationStyle = document.createElement("style");
    animationStyle.id = "bloom-animationSheet";
    document.body.appendChild(animationStyle);
    
    var toolbar = document.createElement("div");
    document.body.appendChild(toolbar);
    toolbar.outerHTML = "<div id='playerToolbar'>Home</div>";
    
    //show the first page
    document.body.getElementsByClassName("bloom-page")[0].classList.add("currentPage");
            
    document.getElementById("playerToolbar").onclick = function(event){
        event.stopPropagation();
        [].forEach.call(document.body.querySelectorAll(".bloom-page"), function(page){
            page.classList.remove("currentPage");
        });
        document.body.getElementsByClassName("bloom-page")[0].classList.add("currentPage");
    }
    
    document.body.onclick = function(event){
        var current = document.body.getElementsByClassName("currentPage")[0];
        if(current){
            current.classList.remove("currentPage");
            current = current.nextElementSibling;
            if(current){
                current.classList.add("currentPage");

                var imageContainer = current.getElementsByClassName("bloom-imageContainer")[0];
                var imgHtml = imageContainer.innerHTML;
                var wrappedHtml = "<div id=\"bloom-imageView\">" + imgHtml + "</div>";
                imageContainer.innerHTML = wrappedHtml;

                var stylesheet = document.getElementById("bloom-animationSheet").sheet;
                stylesheet.deleteRule(0);
                stylesheet.deleteRule(0);

                var animationView = document.getElementById("bloom-imageView");
                var initialTransform = "scale(" + animationView.dataset.initialScaleWidth + ", " + animationView.dataset.initialScaleHeight+ ") translate("+ animationView.dataset.initialOffsetX +"px, " + animationView.dataset.initialOffsetY + "px)";
                var finalTransform = "scale(" + animationView.dataset.finalScaleWidth + ", " + animationView.dataset.finalScaleHeight+ ") translate("+ animationView.dataset.finalOffsetX +"px, " + animationView.dataset.finalOffsetY + "px)";
                stylesheet.insertRule("@keyframes movepic {\
                    from{\
                        transform-origin: 0px 0px;\
                        transform: "+initialTransform+";\
                    }\
                    to{\
                        transform-origin: 0px 0px;\
                        transform: "+finalTransform+";\
                    }\
                }", 0);

                stylesheet.insertRule("#bloom-imageView {\
                    transform-origin: 0px 0px;\
                    transform: " + initialTransform + ";\
                    animation-name: movepic;\
                    animation-duration: 5s;\
                    animation-fill-mode: forwards;\
                }", 1);
            }            
        }
    }
    resize();   
}

function resize(){
    var pageWidth = document.querySelectorAll(".bloom-page")[0].scrollWidth;
    var pageHeight = document.querySelectorAll(".bloom-page")[0].scrollHeight;
    //var scale = window.scrollWidth / pageWidth;
    var scale = Math.min(window.innerWidth/pageWidth,window.innerHeight/pageHeight);
//   var scale = window.innerWidth/pageWidth;
    document.body.style.transform = "scale("+scale+")";
}

document.addEventListener("DOMContentLoaded", attach, false);

window.addEventListener("resize", function(){resize()}, false);
