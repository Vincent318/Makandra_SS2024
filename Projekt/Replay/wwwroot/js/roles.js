function toggleRoleDetails(element) {
    var row = element.closest('tr');
    var dropdown = row.querySelector('.description-container .dropdown');
    var icon = row.querySelector('.role-expand-icon');
   
    dropdown.classList.toggle('expanded');

    if (dropdown.classList.contains('expanded')) {
        icon.src = '/public/up-arrow.png';

    } else {
        icon.src = '/public/down-arrow-black.png';
    }
}

// before hashtag name of tabel, then . class of edit
$("body").on("click", "#tblRole .edit-role", function() {

    //  getting the line to edit 
    var row = $(this).closest("tr");

    //  the first td -> title 
    $("td", row).each(function () {

        //  if there is an input field then this case is looked at 
        if($(this).find("input").length > 0){

            //  shows input box, to enable editing 
            $(this).find("input").show();

            //  hides the text  
            $(this).find("span").hide();
        }
    });
    row.find(".Update").show();
    row.find(".Cancel").show();
    row.find(".Delete").show();
    $(this).hide();
});

$("body").on ("click", "#tblRole .Update", function () {
    var row = $(this).closest("tr");
    var id = row.data("role-id");

    $("td", row).each(function () {
        if ($(this).find("input").length > 0){
            var span = $(this).find("span");
            var input = $(this).find("input");
            span.html(input.val());
            span.show();
            input.hide();
        }
    });

    row.find(".edit-role").show();
    row.find(".delete-role").show();
    row.find(".Cancel").hide();
    $(this).hide();

    var roleViewModel = {
        Id: id,
        Title: row.find(".title-text").html(),
        Description: row.find(".description-container .description-text").html()
    };

    $.ajax({
        type: "POST",
        url: "/Roles/EditRole",
        data: { 
            Id: roleViewModel.Id,
            Title: roleViewModel.Title,
            Description: roleViewModel.Description 
        },
        success: function(response) {
            console.log("Update erfolgreich: ", response);
        },
        error: function(error) {
            console.error("Fehler beim Update: ", error);
        }
    });
});

// Event-Handler für "Cancel"
$("body").on("click", "#tblRole .Cancel", function() {
    var row = $(this).closest("tr");

    $("td", row).each(function() {
        if ($(this).find("input").length > 0) {
            var span = $(this).find("span");
            var input = $(this).find("input");
            span.show();
            input.hide();
        }
    });

    row.find(".edit-role").show();
    row.find(".delete-role").show();
    row.find(".Update").hide();
    $(this).hide();
});