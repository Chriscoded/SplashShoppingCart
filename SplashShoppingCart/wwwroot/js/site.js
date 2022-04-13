$(function () {
    //confirm notification by selecting yes in js alert
    if ($("a.confirmDeletion").length) {
        $("a.confirmDeletion").click(() => {
            if (!confirm("confirm deletion")) return false;
        });
    }

    //confirm notification by selecting yes in js alert (for removal)
    if ($("a.confirmRemoval").length) {
        $("a.confirmRemoval").click(() => {
            if (!confirm("confirm Item Removal")) return false;
        });
    }

    //confirm notification by selecting yes in js alert (for clearAll)
    if ($("a.clearAll").length) {
        $("a.clearAll").click(() => {
            if (!confirm("Are you sure you want to clear all items from cart ?")) return false;
        });
    }

    //Change the time for display of success or error Temp messages
    if ($("div.alert.notification").length) {
        setTimeout(() => {
            $("div.alert.notification").fadeOut();
        }, 2000);
    }
});

//helps us preview image we are uploading
function readURL(input) {
    if (input.files && input.files[0]) {
        let reader = new FileReader();

        reader.onload = function(e) {
            $("img#imgpreview").attr("src", e.target.result).width(200).height(200);
        };

        reader.readAsDataURL(input.files[0]);
    }
}