// login.js

function showLocal() {
    $(".local").removeClass("hidden");
    $(".windows").addClass("hidden");
    $("#localForm").attr("method", "post");
    $("#windowForm").removeAttr("method");
}

function showWindows() {
    $(".windows").removeClass("hidden");
    $(".local").addClass("hidden");
    $("#windowForm").attr("method", "post");
    $("#localForm").removeAttr("method");
}

// run local mode if server instructs
window.addEventListener("DOMContentLoaded", () => {
    if (window.mustShowLocal === true) {
        showLocal();
    }
});
