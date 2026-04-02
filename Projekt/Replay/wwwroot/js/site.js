
/// <author>Daniel Feustel</author>
// NAVBAR JS
var navbarItems = document.getElementsByClassName("navbar-item");
console.log(navbarItems);

document.addEventListener("DOMContentLoaded", () => {
    let url = window.location;
    
    for (let i = 0; i < navbarItems.length; i++) {
        console.log(navbarItems[i].href, url);
        if (url.href === navbarItems[i].href) {
            
            navbarItems[i].className = "active-navbar-item";
        }
        
    }
    
});


