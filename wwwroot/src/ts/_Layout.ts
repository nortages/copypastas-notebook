// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

import { Collapse } from 'bootstrap';
import feather from 'feather-icons';
import { searchParams, search } from "./search";

let isDarkModeEnabled;
const DarkReaderOptions = {
    brightness: 100,
    contrast: 90,
    sepia: 10
};

const menuToggle = $('#navbarSupportedContent');
const bsCollapse = new Collapse(menuToggle[0], { toggle: false })

$(".theme-option").on('click', function () {
    const button = $(this);
    if (button.hasClass("active")) return;
    
    const themeOption = button.data("theme-option");
    let isOptionValid = true;
    switch (themeOption) {
        case "auto":
            // @ts-ignore
            DarkReader.auto(DarkReaderOptions);
            break;
        case "dark":
            // @ts-ignore
            DarkReader.enable(DarkReaderOptions);
            break;
        case "light":
            // @ts-ignore
            DarkReader.disable();
            break;
        default:
            isOptionValid = false;
            break;
    }
    
    if (isOptionValid) {
        localStorage.setItem("themeOption", themeOption);
    }
    
    $(".theme-option.active").removeClass("active");
    button.addClass("active");

    bsCollapse.hide();
});

let savedThemeOption = localStorage.getItem("themeOption");
if (savedThemeOption === null) {
    savedThemeOption = "auto";
}

$('.theme-option[data-theme-option="'+savedThemeOption+'"]').addClass("active");


$("#search-button").on("click", function (e) {
    e.preventDefault();
    searchParams["searchString"] = $("#search-input").val() as string;
    search();
});

$(() => {
    feather.replace();
});