import { searchParams } from "../search";
import { fixHeight } from "../general";
import tippy from "tippy.js";
import 'tippy.js/dist/tippy.css';
import 'tippy.js/themes/light.css';
import bootbox from "bootbox";
import {Collapse} from "bootstrap";

const copypastas = $(".copypasta");
const tagsRow = $(".tags-row");

tagsRow.on("mousedown", function (e) {
  const ele = this;
  
  const pos = {
      // The current scroll 
      left: ele.scrollLeft,
      top: ele.scrollTop,
      // Get the current mouse position
      x: e.clientX,
      y: e.clientY,
  };

  const jqDocument = $(document); 
  // Attach the listeners to `document`
  jqDocument.on('mouseup', function() {
      // Remove the handlers of `mousemove` and `mouseup`
      jqDocument.off('mousemove');
      jqDocument.off('mouseup');
  });
  
  jqDocument.on('mousemove', function (e) {
      // How far the mouse has been moved
      const dx = e.clientX - pos.x;
      const dy = e.clientY - pos.y;

      // Scroll the element
      ele.scrollTop = pos.top - dy;
      ele.scrollLeft = pos.left - dx;
  });
});

function changeShadowOpacity(edge, distToEdge) {
  const threshold = 80;
  if (distToEdge <= threshold) {
      $(this).siblings(`.${edge}-scroll-shadow`).css("opacity", distToEdge / threshold);
  }
  else {
      $(this).siblings(`.${edge}-scroll-shadow`).css("opacity", 1);
  }
}

tagsRow.on('scroll', function (event) {
  const tagsContainer = this;
  
  const distFromStart = tagsContainer.scrollLeft;
  changeShadowOpacity.call(tagsContainer, "left", distFromStart);
  const distToEnd = tagsContainer.scrollWidth - (tagsContainer.offsetWidth + tagsContainer.scrollLeft);
  changeShadowOpacity.call(tagsContainer, "right", distToEnd);
});

const isOverflown = ({ clientWidth, scrollWidth}) => {
  return scrollWidth > clientWidth;
}

function copyToClipboard(text) {
  navigator.clipboard.writeText(text);
}

function onClickCopyButton() {
  const button = $(this);
  const elemToCopy = button.closest(".copypasta").find(".copypasta__text");
  copyToClipboard(elemToCopy.val());
  button.find("i.bi").toggleClass("bi-clipboard bi-clipboard-check");

  setTimeout(function() {
    button.css("color", "unset");
    button.find("i.bi").toggleClass("bi-clipboard-check bi-clipboard");
  }, 1.5 * 1000);
}

copypastas.on('click', '.copy-button', onClickCopyButton);
let clickTimer = null;
copypastas.on('touchstart', '.copypasta__text', function () {
  if (clickTimer == null) {
    clickTimer = setTimeout(function () {
      clickTimer = null;
    }, 500)
  } else {
    clearTimeout(clickTimer);
    clickTimer = null;
    const copyButton = $(this).closest(".copypasta").find(".copy-button");
    onClickCopyButton.call(copyButton);
  }
});

$(".copypasta__text").each(fixHeight);

$("#filterTagsPopover").on('click', ".tags-container.includable .tag", function(e) {
  const tag = $(this);
  toggleFilterTagState(tag, "included", "excluded");
});

$("#filterTagsPopover").on('contextmenu', ".tag", function(e) {
  e.preventDefault();
  const tag = $(this);
  toggleFilterTagState(tag, "excluded", "included");
});

function getFilterTagName(state) {
  return `${state.substr(0, 2)}Tag`;
}

export function toggleFilterTagState(tag, stateToSet, oppositeState) {
  const tagId = tag.data("tag-id");
  const filterTagToSetParam = getFilterTagName(stateToSet);
  if (tag.hasClass(stateToSet)) {
    tag.removeClass(stateToSet);
    const index = searchParams[filterTagToSetParam].indexOf(tagId);
    if (index > -1) {
      searchParams[filterTagToSetParam].splice(index, 1);
    }
  }
  else {
    tag.addClass(stateToSet);
    searchParams[filterTagToSetParam].push(tagId);
  }
  if (tag.hasClass(oppositeState)) {
    tag.removeClass(oppositeState);
    const filterTagToUnsetParam = getFilterTagName(oppositeState);
    const index = searchParams[filterTagToUnsetParam].indexOf(tagId);
    if (index > -1) {
      searchParams[filterTagToUnsetParam].splice(index, 1);
    }
  }
}

$(() => {
  tippy('[data-tippy-content]', {
    theme: "simple-button-tip",
    delay: [1000, null],
  });
  
  $(".horizontal-tags-container").each(function() {
    if (isOverflown(this)) {
      $(this).siblings(".right-scroll-shadow").css("opacity", 1);
    }
  });

  const includedTagIds = [];
  const excludedTagIds = [];
  $("#filterTagsPopover .tag").each(function () {
    const tag = $(this);
    const tagId = tag.data("tag-id");
    if (tag.hasClass("included"))
      includedTagIds.push(tagId);
    if (tag.hasClass("excluded"))
      excludedTagIds.push(tagId);
  });
  searchParams["inTag"] = includedTagIds;
  searchParams["exTag"] = excludedTagIds;

  $(document).on('keydown', function(event) {
    if (event.altKey && event.key == 'F1' && !(event.ctrlKey || event.shiftKey || event.metaKey)) {
      
      bootbox.dialog({
        title: "А?",
        message: "<form action='/Home/Login' method='POST'> <input name='password' type='password'/><input type='submit' value='Тык'/> </form>",
        size: "small",
        centerVertical: true,
        onEscape: true,
        backdrop: true,
        closeButton: false
      });
    }
  });
});