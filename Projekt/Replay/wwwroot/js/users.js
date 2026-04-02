/// <author>Daniel Feustel</author>

// # Zur Information
// ich hatte paar probleme, dass die rollen und abteilungen mit leerzeichen
// gespeichert wurden, was dann probleme mit den String datenbank umwandeln gemacht hat
// ich hab dann innerHtml mit textcontent ausgetauscht  und noch paar trim() hinzugefügt
// die zeilen hier kannst du löschen, wenn dus gelesen hast
// ich habe noch entdeckt, dass ich die Rollen und ABteilungen mit den alten werten aus dem Http Get initalisieren muss 


let btnsDptm = document.getElementsByClassName("select-btn-dptm");
let btnsRoles = document.getElementsByClassName("select-btn-role");
let depContainer = document.getElementById("department-container");
let roleContainer = document.getElementById("role-container");
let container = document.getElementById("additional-data");
let locked = document.getElementById("lock-toggle");

let departments = [];
let roles = [];

for (let i = 0; i < btnsDptm.length; i++) {
    btnsDptm[i].addEventListener("click", () => {
        btnsDptm[i].classList.toggle("selected");
        toggleDepartments(btnsDptm[i]);
    })
}

for (let i = 0; i < btnsRoles.length; i++) {
    btnsRoles[i].addEventListener("click", () => {
        btnsRoles[i].classList.toggle("selected");
        toggleRoles(btnsRoles[i]);
    })
}

function toggleDepartments(selected) {
    let dupl = -1;
    for (let i = 0; i < departments.length; i++) {
        if (departments[i].textContent.trim() === selected.textContent.trim()) {
            dupl = i;
        }
    }
    console.log(dupl);
    if (dupl != -1) {
        departments.splice(dupl,1);
    } else {
        departments.push(selected);
    }
}

function toggleRoles(selected) {
    let dupl = -1;
    for (let i = 0; i < roles.length; i++) {
        if (roles[i].textContent.trim() === selected.textContent.trim()) {
            dupl = i;
        }
    }
    console.log(dupl);
    if (dupl != -1) {
        roles.splice(dupl,1);
    } else {
        roles.push(selected);
    }
}

function validateLists() {
    departments.forEach(e => {
        let newElement = document.createElement("input");
        newElement.type = "text";
        newElement.name = "Departments[]";
        newElement.value = e.textContent.trim();
        newElement.style.display = "none";
        depContainer.appendChild(newElement);
    });

    roles.forEach(e => {
        let newElement = document.createElement("input");
        newElement.type = "text";
        newElement.name = "Roles[]";
        newElement.value = e.textContent.trim();
        newElement.style.display = "none";
        roleContainer.appendChild(newElement);
    });

    let lockedVal = document.createElement("input");
    lockedVal.type = "text";
    lockedVal.name = "IsLocked";
    lockedVal.value = locked.classList.contains("checked") ? "true" : "false";
    lockedVal.style.display = "none";
    container.appendChild(lockedVal);
}

function toggleCheck(element) {
    element.classList.toggle('checked');
    document.getElementById('IsLocked').value = element.classList.contains('checked') ? "true" : "false";
}

// Initialise the roles and departments, so that you dont need the click on the old role und department buttons, when user still has these roles or remains in the departments

document.addEventListener("DOMContentLoaded", function () {
    initializeSelectedDepartments();
    initializeSelectedRoles();
    initializeLockStatus(); // Funktion um den Lock-Status zu setzen
});

function initializeSelectedDepartments() {
    for (let i = 0; i < btnsDptm.length; i++) {
        if (btnsDptm[i].classList.contains("selected")) {
            departments.push(btnsDptm[i]);
        }
    }
}

function initializeSelectedRoles() {
    for (let i = 0; i < btnsRoles.length; i++) {
        if (btnsRoles[i].classList.contains("selected")) {
            roles.push(btnsRoles[i]);
        }
    }
}


function initializeLockStatus() {
    const locked = document.getElementById("lock-toggle");
    // Überprüfen, ob die Checkbox für den Sperrstatus gesetzt ist
    if (locked.classList.contains("checked")) {
        locked.classList.add("checked");
    } else {
        locked.classList.remove("checked");
    }
}