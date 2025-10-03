
$(document).ready(function () {
    $('#FormVerifica').submit(function () {
 
        $('#FormVerifica').validate();
        if ($('#FormVerifica').valid() == true) {
            $("#submitBtn").prop("disabled", true);
            $("#spinner").removeClass("d-none");
            $("#spinner").focus();
        }
        else e.preventDefault();

    });
});
