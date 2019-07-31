// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(document).ready(function () {
    $('input[type=checkbox]').on('change', function (e) {
        if ($('input[type=checkbox]:checked').length > 2) {
            $(this).prop('checked', false);
            this.checked = false;
        }
    })
});


$(function () {
    $(".listitems li").sort(sort_li).appendTo('.listitems');
    function sort_li(a, b) {
        return ($(b).data('position')) > ($(a).data('position')) ? 1 : -1;
    }
});

$(function () {
    $("#fingerKnot")
        .mouseover(function () {
            $(this).attr("src", "/images/bluefinger.jpg");
        })
        .mouseout(function () {
            $(this).attr("src", "/images/finger.jpg");
        });
});

//$(function () {
//    $("#firstFade").load(function () {
//        $(this).fadeIn();
//    });
//});

$(function () {
    $("#firstFade").delay(100).fadeIn(1000);
});

$(function () {
    $("#secondFade").delay(1000).fadeIn("slow");
});

$(function () {
    $("#thirdFade").delay(2500).fadeIn("slow");
});