//author: Noah Engelschall

let selectedFilter = 'Alle'; // The currently selected filter
let currentSortOrder = ''; // The current sort order

// Toggles the view between "Meine" and "Alle" tasks
function toggleView(viewType) {
        window.location.href = `/Tasklist?viewType=${viewType}&filter=${document.getElementById('filter-input').value}&filterType=${selectedFilter}&first=false`;
}

// Shows or hides the filter dropdown menu
function toggleDropdown() {
        document.getElementById('filter-dropdown').classList.toggle('show');
}

// Sets the selected filter and updates the search field
function setFilter(filter) {
        selectedFilter = filter;
        document.getElementById('filter-icon').setAttribute('alt', filter); // Change the alt text to show current filter
        document.getElementById('filter-dropdown').classList.remove('show');

        const dropdownLinks = document.querySelectorAll('#filter-dropdown a');
        dropdownLinks.forEach(link => link.classList.remove('active'));

        const activeLink = Array.from(dropdownLinks).find(link => link.innerText === filter);
        if (activeLink) {
                activeLink.classList.add('active');
        }

        const filterInput = document.getElementById('filter-input');
        switch (filter) {
                case 'Aufgabe':
                        filterInput.placeholder = 'Suche nach Aufgabe';
                        break;
                case 'Vorgang':
                        filterInput.placeholder = 'Suche nach Prozess';
                        break;
                default:
                        filterInput.placeholder = 'Suche nach';
        }
}

// Handles the Enter key in the search field
function handleKeyPress(event) {
        if (event.key === 'Enter') {
                const query = document.getElementById('filter-input').value;
                window.location.href = `/Tasklist?viewType=${document.querySelector('.toggle-button .active').innerText}&filter=${query}&filterType=${selectedFilter}&first=false`;
        }
}

// Resets the view to show all tasks
function showAll() {
        window.location.href = `/Tasklist?viewType=${document.querySelector('.toggle-button .active').innerText}&first=${false}`;
}

// Shows or hides the sort dropdown menu
function toggleSortDropdown() {
        document.getElementById('sort-dropdown').classList.toggle('show');
}

function parseDate(dateString) {
        const formats = [
                'YYYY-MM-DD',  // ISO-Format
                'DD.MM.YYYY',   // Deutsches Format
                'M/D/YYYY'     // US-Format
        ];

        for (let format of formats) {
                const date = moment(dateString, format, true);
                if (date.isValid()) {
                        return date.toDate();
                }
        }
        return new Date(NaN);
}


// Sorts the table based on the due date
function sortTable(order) {
        currentSortOrder = order;
        const rows = Array.from(document.querySelectorAll('tbody tr'));
        rows.sort((a, b) => {
                const dateA = parseDate(a.querySelector('td:nth-child(3)').innerText);
                const dateB = parseDate(b.querySelector('td:nth-child(3)').innerText);
                return order === 'asc' ? dateA - dateB : dateB - dateA;
        });

        const tbody = document.querySelector('tbody');
        tbody.innerHTML = '';
        rows.forEach(row => tbody.appendChild(row));
        document.getElementById('sort-dropdown').classList.remove('show');

        // Updates the sort icon based on the sort order
        const sortIcon = document.getElementById('sort-icon');
        sortIcon.src = order === 'asc' ? '/public/arrow-up.png' : '/public/arrow-down.png';
}

// Groups the table based on the process
function groupByVorgang() {
        const rows = Array.from(document.querySelectorAll('tbody tr'));
        rows.sort((a, b) => {
                const vorgangA = a.querySelector('td:nth-child(2)').innerText.toLowerCase();
                const vorgangB = b.querySelector('td:nth-child(2)').innerText.toLowerCase();
                return vorgangA.localeCompare(vorgangB);
        });

        const tbody = document.querySelector('tbody');
        tbody.innerHTML = '';
        rows.forEach(row => tbody.appendChild(row));
}

// Saves the current sort order in local storage
function saveSortState() {
        localStorage.setItem('sortOrder', currentSortOrder);
}

// Applies the saved sort order when the page loads
window.onload = function () {
        const savedSortOrder = localStorage.getItem('sortOrder');
        if (savedSortOrder) {
                sortTable(savedSortOrder);
        }
        highlightOverdueTasks();
}

// Completes a task and refreshes the page
function completeTask(taskId) {
        fetch(`/TaskList/CompleteTask/${taskId}`, {
                method: 'POST',
                headers: {
                        'Content-Type': 'application/json',
                        'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
                }
        }).then(response => {
                if (response.ok) {
                        location.reload();
                } else {
                        alert('Error completing the task');
                }
        });
}

// Closes all dropdown menus when clicking outside
window.onclick = function (event) {
        if (!event.target.matches('#filter-icon') && !event.target.matches('#sort-icon') && !event.target.matches('#group-icon')) {
                var dropdowns = document.getElementsByClassName("dropdown-content");
                for (var i = 0; i < dropdowns.length; i++) {
                        var openDropdown = dropdowns[i];
                        if (openDropdown.classList.contains('show')) {
                                openDropdown.classList.remove('show');
                        }
                }
        }
}

// Adds hover and active filter logic to the filter icon
document.addEventListener('DOMContentLoaded', (event) => {
        const filterIcon = document.getElementById('filter-icon');
        const dropdownLinks = document.querySelectorAll('#filter-dropdown a');

        function updateFilterIcon() {
                const activeFilter = Array.from(dropdownLinks).some(link => link.classList.contains('active'));
                filterIcon.src = activeFilter ? '/public/filter-dark.png' : '/public/filter-bright.png';
        }

        filterIcon.addEventListener('mouseover', () => {
                filterIcon.src = '/public/filter-dark.png';
        });

        filterIcon.addEventListener('mouseout', () => {
                updateFilterIcon();
        });

        updateFilterIcon();
});

// Function to highlight overdue tasks
function highlightOverdueTasks() {
        console.log("highlightOverdueTasks called"); // Debugging output
        const rows = document.querySelectorAll('tbody tr');
        const today = new Date();

        const viewType = document.querySelector('.header-font').textContent.trim();
        if (viewType !== "MEINE AUFGABEN") {
                return;
        }

        rows.forEach(row => {
                const dueDateText = row.querySelector('td:nth-child(3)').innerText;
                const dueDate = parseDate(dueDateText);
                console.log(`Checking due date: ${dueDate}`);
                if (dueDate < today) {
                        console.log(`Overdue task found: ${dueDate}`);
                        row.style.backgroundColor = 'rgba(255, 0, 0, 0.2)';

                        const overdueTextCell = row.querySelector('td:nth-child(4)');
                        if (overdueTextCell) {
                                overdueTextCell.textContent = 'Aufgabe überfällig';
                                overdueTextCell.style.color = 'black';
                        }
                }
        });
}

//toggles the task description
function toggleInstruction(taskId) {
        var instructionElement = document.getElementById('instruction-' + taskId);
        if (instructionElement.style.display === 'none' || instructionElement.style.display === '') {
                instructionElement.style.display = 'block';
        } else {
                instructionElement.style.display = 'none';
        }
}