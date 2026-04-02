/// <author>Daniel Feustel</author>
var btns = document.getElementsByClassName("faq-btn");

for (let i = 0; i < btns.length; i++) {
    let sibling = btns[i].nextElementSibling;
    sibling.style.display = "none";
    btns[i].addEventListener("click", () => {
        
        sibling.style.display = (sibling.style.display == 'none') ? 'flex' : 'none';
    });
}

var dbArrowDown = document.getElementsByClassName("dashboard-down");

for (let i = 0; i < dbArrowDown.length; i++) {
    dbArrowDown[i].parentNode.addEventListener("click", () => {
        dbArrowDown[i].style.transform += "rotate(180deg)";
    });
}



