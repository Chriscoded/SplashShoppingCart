$(function () {
    //confirm notification by selecting yes in js alert
    if ($("a.confirmDeletion").length) {
        $("a.confirmDeletion").click(() => {
            if (!confirm("confirm deletion")) return false;
       });
    }

    //Change the time for display of success or error Temp messages
    if ($("div.alert.notification").length) {
        setTimeout(() => {
            $("div.alert.notification").fadeOut();
        }, 2000);
    }
})